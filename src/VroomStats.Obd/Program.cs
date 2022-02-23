using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OBD.NET.Common.Communication;
using OBD.NET.Desktop.Communication;
using Serilog;
using Serilog.Events;
using VroomStats.Obd.Services;

var loggerTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog((x, y) => y
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .WriteTo.Conditional(
            logEvent => x.HostingEnvironment.IsProduction(),
            logConfig => logConfig.File(@"./logs/", retainedFileCountLimit: 31, rollingInterval: RollingInterval.Day, outputTemplate: loggerTemplate))
        .WriteTo.Console(outputTemplate: loggerTemplate))
    .ConfigureServices((ctx, services) =>
    {
        services.AddTransient<ClientWebSocket>();
        services.AddSingleton<IWsHandlerService, WsHandlerService>();
        services.AddSingleton<ISerialConnection>(_ => new SerialConnection(ctx.Configuration["Obd:SerialPort"]));
        services.AddSingleton<ExtendedElm327>();
        services.AddHostedService<ObdService>();
    });

var app = builder.Build();
app.Run();