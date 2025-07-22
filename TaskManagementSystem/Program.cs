using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
builder.Services.AddDbContext<NodeContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Nodes}/{id?}"
    );

app.MapControllerRoute(
    name: "Main",
    pattern: "Home/Nodes/"
    );

app.Run();
