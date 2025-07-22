using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private NodeContext _notedb;
        public HomeController(NodeContext noteContext)
        {
            _notedb = noteContext;
        }
        [HttpGet]
        public IActionResult Nodes()
        {
            var nodes = _notedb.Nodes.ToList();

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
            await _notedb.Nodes.AddAsync(node);
            await _notedb.SaveChangesAsync();

            return RedirectToAction("Nodes");
        }
    }
}
