using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OBD.NET.Common.Communication;
using OBD.NET.Desktop.Communication;
using VroomStats.Obd.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddTransient<ClientWebSocket>();
        services.AddSingleton<IWsHandlerService, WsHandlerService>();
        services.AddSingleton<ISerialConnection>(_ => new SerialConnection(ctx.Configuration["Obd:SerialPort"]));
        services.AddSingleton<ExtendedElm327>();
        services.AddHostedService<ObdService>();
    });

var app= builder.Build();
app.Services.GetRequiredService<ExtendedElm327>().Initialize();
app.Run();