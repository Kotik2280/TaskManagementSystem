using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagementSystem.Models;
using TaskManagementSystem.Attributes;

namespace TaskManagementSystem.Controllers
{
    [AutorizeRedirect]
    public class HomeController : Controller
    {
        private NodeContext _nodedb;
        public HomeController(NodeContext noteContext)
        {
            _nodedb = noteContext;
        }
        [HttpGet]
        public IActionResult Index() => View();
        [HttpGet]
        public IActionResult Registration() => View();
        [HttpPost]
        public async Task<IActionResult> Registration(User user)
        {
            if ((await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == user.Name)) is not null)
                return BadRequest();

            await _nodedb.Users.AddAsync(user);

            await _nodedb.SaveChangesAsync();

            return RedirectToRoute("Main");
        }
        [HttpGet]
        public IActionResult Authorization() => View();
        [HttpPost]
        public async Task<IActionResult> Authorization(User inputUser)
        {
            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == inputUser.Name && u.Password == inputUser.Password);

            if (user == null) return BadRequest();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };

            var jwtToken = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(120)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            string stringToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            Response.Cookies.Append("jwt", stringToken, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(120)
            });

            return RedirectToAction("Nodes", "Verified");
        }
    }
}
