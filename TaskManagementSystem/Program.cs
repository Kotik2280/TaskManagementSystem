using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) //Отключение подробных логов
    .Filter.ByExcluding(e =>
        e.MessageTemplate.Text.Contains("Unable to configure Browser Link") ||
        e.MessageTemplate.Text.Contains("Unable to configure browser refresh") ||
        e.MessageTemplate.Text.Contains("Executed DbCommand"))
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) //Запись логов в файл
    .CreateLogger();

static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');

        return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true;";
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException("Invalid DATABASE_URL format", ex);
    }
}

try
{
    Log.Information("Starting web application...");

    builder.Host.UseSerilog();

    builder.Services.AddMvc();

    string? connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string not found");
        }
    }
    else
    {
        connectionString = ConvertDatabaseUrlToConnectionString(connectionString);
    }

    connectionString = "postgresql://postgres:ukJxeEzgnwgDrYOpSAvWwcNNKVMIWWIt@postgres.railway.internal:5432/railway"; //temp

    builder.Services.AddDbContext<NodeContext>(
        options => options.UseNpgsql(connectionString));

    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddCookie(options => options.LoginPath = "/Home/Index")
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = AuthOptions.AUDIENCE,
                ValidateLifetime = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["jwt"];
                    return Task.CompletedTask;
                }
            };
        });

    var app = builder.Build();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "Default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
        );

    app.MapControllerRoute(
        name: "Main",
        pattern: "Home/Index/"
        );

    await app.StartAsync();

    Log.Information("====== Web application started successful! ======");
    Log.Information("Urls: {Urls}", string.Join(", ", app.Urls));
    Log.Information("Environment: {Env}", app.Environment.EnvironmentName);
    Log.Information("Database connection string: ", connectionString);

    await app.WaitForShutdownAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "Application starting error");
}
finally
{
    Log.CloseAndFlush();
}
public class AuthOptions
{
    public const string ISSUER = "EpiccipE";
    public const string AUDIENCE = "TaskManager";
    private const string secretKey = "328qrnwq98nvqwf9kqw0eufrnvw0qeuxfwe0mqucfvwqneyf9wq8ynbcffamqm";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); 
}