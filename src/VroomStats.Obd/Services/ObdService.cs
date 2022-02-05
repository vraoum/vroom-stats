using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OBD.NET.Common.OBDData;

namespace VroomStats.Obd.Services;

public class ObdService : BackgroundService
{
    private readonly ExtendedElm327 _device;
    private readonly ILogger<ObdService> _logger;

    public ObdService(ExtendedElm327 device, ILogger<ObdService> logger)
    {
        _device = device;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var vin = await _device.RequestVinAsync();
        _logger.LogInformation("Vehicle VIN: {Vin}", vin);

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

            // pull every 2000ms
            await Task.Delay(2000, stoppingToken);
        }
    }
}