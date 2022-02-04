using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OBD.NET.Common.Communication;
using OBD.NET.Common.Devices;
using OBD.NET.Desktop.Communication;
using VroomStats.Obd.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddSingleton<ISerialConnection>(_ => new SerialConnection(ctx.Configuration["Obd:SerialPort"]));
        services.AddSingleton(x => new ELM327(x.GetRequiredService<ISerialConnection>()));
        services.AddHostedService<ObdService>();
    });

var app= builder.Build();
await app.Services.GetRequiredService<ELM327>().InitializeAsync();
app.Run();