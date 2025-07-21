var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

var app = builder.Build();

app.MapControllerRoute(
    name: "Main",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
