using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OBD.NET.Common.OBDData;
using VroomStats.Payloads;

namespace VroomStats.Obd.Services;

public class ObdService : BackgroundService
{
    private readonly ExtendedElm327 _device;
    private readonly ILogger<ObdService> _logger;
    private readonly IWsHandlerService _ws;
    private readonly IHostApplicationLifetime _host;

    public ObdService(ExtendedElm327 device, ILogger<ObdService> logger, IWsHandlerService ws, IHostApplicationLifetime host)
    {
        _device = device;
        _logger = logger;
        _ws = ws;
        _host = host;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!TryConnectElm(5))
        {
            _host.StopApplication();
            return;
        }
        
        var vin = await _device.RequestVinAsync();
        _logger.LogInformation("Vehicle VIN: {Vin}", vin);

        if (!await TryConnectWsAsync(5, vin.Vin))
        {
            _host.StopApplication();
            return;
        }
        
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

            await _ws.SendPayloadAsync(new BasePayload(OpCode.Data, new Dictionary<string, string>
            {
                ["speed"] = speed.Speed.Value.ToString(CultureInfo.InvariantCulture),
                ["rpm"] = rpm.Rpm.Value.ToString(CultureInfo.InvariantCulture)
            }));
            
            // pull every 200ms
            await Task.Delay(200, stoppingToken);
        }
    }

    private async Task<bool> TryConnectWsAsync(int count, string vin)
    {
        while (count > 0)
        {
            try
            {
                await Task.Delay(5000);
                await _ws.ConnectAsync(vin);
                break;
            }
            catch (Exception ex)
            {
                count--;
                
                if (count == 0)
                {
                    _logger.LogError(ex, "Unable to connect to remote WebSocket server. Exiting");
                    return false;
                }
                
                _logger.LogError(ex, "Unable to connect to remote WebSocket server. Retrying {Count} times", count);
            }
        }

        return true;
    }
    
    private bool TryConnectElm(int count)
    {
        while (count > 0)
        {
            try
            {
                Thread.Sleep(5000);
                _device.Initialize();
                break;
            }
            catch (Exception ex)
            {
                count--;
                
                if (count == 0)
                {
                    _logger.LogError(ex, "Unable to initialize ELM327. Exiting");
                    return false;
                }
                
                _logger.LogError("Unable to initialize ELM327. Retrying {Count} times", count);
            }
        }

        return true;
    }
}