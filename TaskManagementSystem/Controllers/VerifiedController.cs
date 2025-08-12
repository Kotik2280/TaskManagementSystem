using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.Validators;
using Serilog;
using TaskManagementSystem.Models.ViewModels;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class VerifiedController : Controller
    {
        private NodeContext _nodedb;
        public VerifiedController(NodeContext context)
        {
            _nodedb = context;
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == HttpContext.User.Identity.Name);

            if (user is null) BadRequest();
            
            Response.Cookies.Delete("jwt");

            Log.Information("[JWT LOGOUT] User ID: {Id}, Name: {Name} loguot successful!", user.Id, user.Name);

            return RedirectToRoute("Main");
        }

        [HttpGet]
        public IActionResult Nodes()
        {
            ViewData["UserName"] = HttpContext.User.Identity.Name;

            var nodes = _nodedb.Nodes.OrderByDescending(n => n.Id).ToList();

            return View(new NodesAndSortingSettings(nodes, new SortingSettings()));
        }
        [HttpPost]
        public IActionResult Nodes(SortingSettings sortingSettings)
        {
            ViewData["UserName"] = HttpContext.User.Identity.Name;

            List<Node> nodes = null;

            if (sortingSettings.Field is null || sortingSettings.Vector is null)
                nodes = _nodedb.Nodes.OrderByDescending(n => n.Id).ToList();
            else
            {
                nodes = NodeSorter.Sort(_nodedb.Nodes, sortingSettings);
            }

            return View(new NodesAndSortingSettings(nodes, sortingSettings));
        }
        [HttpGet]
        public IActionResult AddNode()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNode(NodeCreation nodeCreation)
        {
            NodeCreationValidator validator = new NodeCreationValidator(_nodedb);

            if (!validator.Validate(nodeCreation).IsValid)
            {
                ViewData["Errors"] = validator.Validate(nodeCreation).Errors;
                return View();
            }

            Node node = nodeCreation.Node;
            var (days, hours, minutes) = nodeCreation.DeadLineTime;
            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == HttpContext.User.Identity.Name);

            node.CreateDate = DateTime.Now;
            if (days != 0 || hours != 0 || minutes != 0)
                node.DeadLine = node.CreateDate + new TimeSpan(days, hours, minutes, 0);
            node.AuthorName = user.Name;

            node.Responsible = nodeCreation.Responsible;

            await _nodedb.Nodes.AddAsync(node);
            await _nodedb.SaveChangesAsync();

            Log.Information("[NODE CREATED] {@node}\nby [{id}]{name}", node, user.Id, user.Name);

            return RedirectToAction("Nodes");
        }
        [HttpGet]
        public async Task<IActionResult> EditNode(int id)
        {
            Node? node = await _nodedb.Nodes.FirstOrDefaultAsync(n => n.Id == id);

            return View(node);
        }
        [HttpPost]
        public async Task<IActionResult> EditNode(Node newNode)
        {
            Node? oldNode = await _nodedb.Nodes.FirstOrDefaultAsync(n => n.Id == newNode.Id);

            if (oldNode is null) return RedirectToRoute("Main");

            NodeValidator validator = new NodeValidator();
            ValidationResult validationResult = validator.Validate(newNode);

            if (!validationResult.IsValid)
            {
                if (validationResult.Errors.Any(e => e.PropertyName == "Title"))
                    ModelState.Remove("Title");
                if (validationResult.Errors.Any(e => e.PropertyName == "Description"))
                    ModelState.Remove("Description");
                ViewData["Errors"] = validationResult.Errors;
                return View(oldNode);
            }

            string oldNodeTitle = oldNode.Title;
            string oldNodeDescription = oldNode.Description;

            oldNode.Title = newNode.Title;
            oldNode.Description = newNode.Description;

            await _nodedb.SaveChangesAsync();

            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == HttpContext.User.Identity.Name);

            Log.Information("[NODE EDITED] Node ID: {Id}, Title: {oldTitle}, Description: {oldDesc} -> Title: {newTitle}, Descrition: {newDesc}\n" +
                "By User ID: {UserId}, Name: {Name}",
            oldNode.Id, oldNodeTitle, oldNodeDescription, newNode.Title, newNode.Description, 
            user.Id, user.Name);

            return RedirectToAction("Nodes");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteNode(int id)
        {
            Node? node = await _nodedb.Nodes.FirstOrDefaultAsync(n => n.Id == id);

            if (node is null) return RedirectToRoute("Main");

            _nodedb.Nodes.Remove(node);

            await _nodedb.SaveChangesAsync();

            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == HttpContext.User.Identity.Name);

            Log.Information("[NODE DELETED] Node ID: {Id}, Title: {Title}, Description: {Desc}\n" +
                "By User ID: {UserId}, Name: {Name}",
            node.Id, node.Title, node.Description,
            user.Id, user.Name);

            return RedirectToAction("Nodes");
        }
        [HttpGet]
        public async Task<IActionResult> Profile(string name)
        {
            if (name == HttpContext.User.Identity.Name)
                return RedirectToAction("MyProfile");

            List<Node> nodes = await _nodedb.Nodes.Where(n => n.AuthorName == name).ToListAsync();

            ViewData["AuthorName"] = name;

            return View(nodes);
        }
        [HttpGet]
        public async Task<IActionResult> ProfileResponsible(string name)
        {
            List<Node> nodes = await _nodedb.Nodes.Where(n => n.Responsible == name).ToListAsync();

            ViewData["AuthorName"] = name;

            int complited = (await _nodedb.Nodes
                        .Where(n => n.Responsible == name && (NodeStatus)n.Status == NodeStatus.Accepted)
                        .ToListAsync()).Count;
            int failed = (await _nodedb.Nodes
                        .Where(n => n.Responsible == name && (NodeStatus)n.Status == NodeStatus.Ejected)
                        .ToListAsync()).Count;

            ViewData["Complited"] = complited;
            ViewData["Failed"] = failed;
            ViewData["Percent"] = complited == 0 && failed == 0 ? "-" : Math.Round((complited / (double)(complited + failed) * 100), 2);

            return View(nodes);
        }
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            string name = HttpContext.User.Identity.Name;

            List<Node> nodes = await _nodedb.Nodes.Where(n => n.AuthorName == name).ToListAsync();
            ViewData["AuthorName"] = name;

            return View(nodes);
        }

        [HttpPost]
        public async Task<IActionResult> SetNodeStatus(int id, NodeStatus status)
        {
            Node? node = await _nodedb.Nodes.FirstOrDefaultAsync(n => n.Id == id);

            NodeStatus oldStatus = (NodeStatus)node.Status;

            node.Status = (int)status;

            await _nodedb.SaveChangesAsync();

            string userName = HttpContext.User.Identity.Name;

            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == HttpContext.User.Identity.Name);

            Log.Information("[NODE {rate}] Node ID: {Id}, Title: {Title}, Description: {Desc}, Status: {status} -> {newStatus}\n" +
                "by [{userId}]{name}", status, node.Id, node.Title, node.Description, oldStatus, status, user.Id, user.Name);

            return RedirectToAction("MyProfile");
        }
    }
}
