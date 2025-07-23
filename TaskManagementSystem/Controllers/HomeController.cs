using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private NodeContext _nodedb;
        public HomeController(NodeContext noteContext)
        {
            _nodedb = noteContext;
        }
        [HttpGet]
        public IActionResult Nodes()
        {
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
            await _nodedb.Nodes.AddAsync(node);
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

            return RedirectToRoute("Main");
        }
    }
}
