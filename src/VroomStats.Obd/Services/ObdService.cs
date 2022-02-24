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

        _logger.LogInformation("Fuel type: {Type}", fuelType);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var speed = await _device.RequestDataAsync<VehicleSpeed>();
                var rpm = await _device.RequestDataAsync<EngineRPM>();
                var throttle = await _device.RequestDataAsync<ThrottlePosition>();
                var runTime = await _device.RequestDataAsync<RunTimeSinceEngineStart>();
                var fuel = await _device.RequestDataAsync<FuelTankLevelInput>();
                var airTemperature = await _device.RequestDataAsync<IntakeAirTemperature>();
                var ambientAirTemperature = await _device.RequestDataAsync<AmbientAirTemperature>();
                var engineOilTemperature = await _device.RequestDataAsync<EngineOilTemperature>();
                var odometer = await _device.RequestDataAsync<Odometer>();
                
                await _ws.SendPayloadAsync(new BasePayload(OpCode.Data, new Dictionary<string, string?>
                {
                    ["speed"] = speed?.Speed.Value.ToString(CultureInfo.InvariantCulture),
                    ["rpm"] = rpm?.Rpm.Value.ToString(CultureInfo.InvariantCulture),
                    ["throttle"] = throttle?.Position.Value.ToString(CultureInfo.InvariantCulture),
                    ["fuel"] = fuel?.Level.Value.ToString(CultureInfo.InvariantCulture),
                    ["runTime"] = TimeSpan.FromSeconds(runTime?.Runtime.Value ?? 0).ToString("g", CultureInfo.InvariantCulture),
                    ["airTemperature"] = airTemperature?.Temperature.Value.ToString(CultureInfo.InvariantCulture),
                    ["ambientAirTemperature"] = ambientAirTemperature?.Temperature.Value.ToString(CultureInfo.InvariantCulture),
                    ["engineOilTemperature"] = engineOilTemperature?.Temperature.Value.ToString(CultureInfo.InvariantCulture),
                    ["odometer"] = odometer?.Odom.Value.ToString(CultureInfo.InvariantCulture)
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured when trying to either retrieve OBD values or send them through websockets");
            }
            finally
            {
                // pull every 200ms
                await Task.Delay(200, stoppingToken);
            }
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