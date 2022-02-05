using MongoDB.Driver;
using VroomStats.Services;

var builder = WebApplication.CreateBuilder(args);

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