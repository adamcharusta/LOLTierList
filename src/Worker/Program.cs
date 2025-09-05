using LOLTierList.Infrastructure;
using LOLTierList.Worker;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "worker")
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetSection("Seq")["ServerUrl"] ?? "http://localhost:5341")
    .CreateLogger();

Log.Information("Starting worker host");

try
{
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.Information("Shutting down worker host");
    Log.CloseAndFlush();
}

public partial class Program;
