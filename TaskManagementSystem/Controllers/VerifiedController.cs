using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.Validators;

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
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

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

            return RedirectToAction("Nodes");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteNode(int id)
        {
            Node? node = await _nodedb.Nodes.FirstOrDefaultAsync(n => n.Id == id);

            if (node is null) return RedirectToRoute("Main");

            _nodedb.Nodes.Remove(node);

            await _nodedb.SaveChangesAsync();

            return RedirectToAction("Nodes");
        }
    }
}
