using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.Validators;
using Serilog;
using System.Xml.Linq;

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

            var nodes = _nodedb.Nodes.ToList();

            return View(nodes);
        }
        [HttpGet]
        public IActionResult AddNode()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNode(Node node)
        {
            NodeValidator validator = new NodeValidator();

            if (!validator.Validate(node).IsValid)
            {
                ViewData["Errors"] = validator.Validate(node).Errors;
                return View();
            }

            node.CreateDate = DateTime.Now;

            await _nodedb.Nodes.AddAsync(node);
            await _nodedb.SaveChangesAsync();

            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == HttpContext.User.Identity.Name);
            Log.Information("[NODE CREATED] Node ID: {Id}, Title: {Title}, Description: {Desc}\n" +
                "By User ID: {Id}, Name: {Name}", 
                node.Id, node.Title, node.Description, 
                user.Id, user.Name);

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

            oldNode.Title = newNode.Title;
            oldNode.Description = newNode.Description;

            await _nodedb.SaveChangesAsync();

            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == HttpContext.User.Identity.Name);

            Log.Information("[NODE EDITED] Node ID: {Id}, Title: {oldTitle}, Description: {oldDesc} -> Title: {newTitle}, Descrition: {newDesc}\n" +
                "By User ID: {Id}, Name: {Name}",
            oldNode.Id, oldNode.Title, oldNode.Description, newNode.Title, newNode.Description, 
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
                "By User ID: {Id}, Name: {Name}",
            node.Id, node.Title, node.Description,
            user.Id, user.Name);

            return RedirectToAction("Nodes");
        }
    }
}
