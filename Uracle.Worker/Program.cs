using Uracle.Application;
using Uracle.Infrastructure;
using Uracle.Worker;
using Uracle.Worker.ActivityWorker;

var builder = Host.CreateApplicationBuilder(args);

// Bind appsettings + environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Register Application and Infrastructure DI
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register background services
builder.Services.AddHostedService<ActivityWorker>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
