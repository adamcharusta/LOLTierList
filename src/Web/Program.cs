using LOLTierList.Infrastructure;
using LOLTierList.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "web")
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetSection("Seq")["ServerUrl"] ?? "http://localhost:5341")
    .CreateLogger();

Log.Error("Starting web host");

try
{
    builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddWeb();

    var app = builder.Build();

    app
        .UseWeb()
        .Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
