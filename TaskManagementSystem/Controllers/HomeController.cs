using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagementSystem.Models;
using TaskManagementSystem.Models.Validators;
using TaskManagementSystem.Attributes;
using Serilog;

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
            UserValidator validator = new UserValidator();
            if (!validator.Validate(user).IsValid)
            {
                ViewData["Errors"] = validator.Validate(user).Errors;
                return View();
            }

            if ((await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == user.Name)) is not null)
            {
                ViewData["MultiAccoutError"] = "User with this Name is already registered";
                Log.Information("Attempt to register name that already in use ({Name})", user.Name);
                return View();
            }

            await _nodedb.Users.AddAsync(user);

            await _nodedb.SaveChangesAsync();

            Log.Information("[USER CREATED] User ID: {Id} Name: {Name} successful!", user.Id, user.Name);

            return RedirectToRoute("Main");
        }
        [HttpGet]
        public IActionResult Authorization() => View();
        [HttpPost]
        public async Task<IActionResult> Authorization(User inputUser)
        {
            User? user = await _nodedb.Users.FirstOrDefaultAsync(u => u.Name == inputUser.Name && u.Password == inputUser.Password);

            if (user == null) 
            {
                ViewData["Errors"] = "Name or Password don't match";
                return View();
            }

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

            Log.Information("[JWT LOGIN] User ID: {Id} Name: {Name} authorized successful!", user.Id, user.Name);

            return RedirectToAction("Nodes", "Verified");
        }
    }
}
