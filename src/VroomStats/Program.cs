using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using VroomStats.Services;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Your mom is a fucking whore");

var loggerTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
builder.Host.UseSerilog((x, y) => y
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Conditional(
        logEvent => x.HostingEnvironment.IsProduction(), 
        logConfig => logConfig.File(@"./logs/", retainedFileCountLimit: 31, rollingInterval: RollingInterval.Day, outputTemplate: loggerTemplate))
    .WriteTo.Console(outputTemplate: loggerTemplate));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IWsHandlerService, WsHandlerService>();
builder.Services.AddSingleton(_ => new MongoClient($"mongodb://{builder.Configuration["Database:Host"] ?? "local-mongo"}:{builder.Configuration["Database:Port"] ?? "27017"}"));
builder.Services.AddSingleton(x => x.GetRequiredService<MongoClient>().GetDatabase("vroom-stats"));
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

var app = builder.Build();

app.UseWebSockets();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();