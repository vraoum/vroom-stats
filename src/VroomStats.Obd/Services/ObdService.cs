using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OBD.NET.Common.Devices;
using OBD.NET.Common.OBDData;

namespace VroomStats.Obd.Services;

public class ObdService : BackgroundService
{
    private readonly ELM327 _device;
    private readonly ILogger<ObdService> _logger;

    public ObdService(ELM327 device, ILogger<ObdService> logger)
    {
        _device = device;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _device.InitializeAsync();
        
        var fuelType = await _device.RequestDataAsync<FuelType>();
        var fuelStatus = await _device.RequestDataAsync<FuelSystemStatus>();
        
        _logger.LogInformation("Fuel type: {Type}; Status 1: {Status1}; Status 2: {Status2}", 
            fuelType, fuelStatus.StatusSystem1, fuelStatus.StatusSystem2);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var speed = await _device.RequestDataAsync<VehicleSpeed>();
            var rpm = await _device.RequestDataAsync<EngineRPM>();
            
            _logger.LogInformation("Car speed: {Speed}; Rpm: {Rpm}", 
                speed, rpm);

            // pull every 500ms
            await Task.Delay(500, stoppingToken);
        }
    }
}