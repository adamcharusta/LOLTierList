using LOLTierList.Infrastructure;
using LOLTierList.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
