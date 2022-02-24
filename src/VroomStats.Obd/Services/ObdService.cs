using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly IIoTService? _ioTService;

    public ObdService(ExtendedElm327 device, ILogger<ObdService> logger, 
        IWsHandlerService ws, IHostApplicationLifetime host, IServiceProvider provider)
    {
        _device = device;
        _logger = logger;
        _ws = ws;
        _host = host;
        _ioTService = provider.GetService<IIoTService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ioTService?.Initialize();

        if (!await TryConnectElmAsync(5, stoppingToken))
        {
            _logger.LogCritical("Couldn't connect to the ELM");
            _host.StopApplication();
            return;
        }

        var vin = await _device.RequestVinAsync();
        _logger.LogInformation("Vehicle VIN: {Vin}", vin);

        if (!await TryConnectWsAsync(5, vin.Vin, stoppingToken))
        {
            _logger.LogCritical("Couldn't connect to the websocket server");
            _host.StopApplication();
            return;
        }
        
        _logger.LogInformation("Start requesting data and sending payloads");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var speed = await _device.RequestDataAsync<VehicleSpeed>();
                await Task.Delay(20, stoppingToken);
                var rpm = await _device.RequestDataAsync<EngineRPM>();
                await Task.Delay(20, stoppingToken);
                var throttle = await _device.RequestDataAsync<ThrottlePosition>();
                await Task.Delay(20, stoppingToken);
                var runTime = await _device.RequestDataAsync<RunTimeSinceEngineStart>();
                await Task.Delay(20, stoppingToken);
                var fuel = await _device.RequestDataAsync<FuelTankLevelInput>();
                await Task.Delay(20, stoppingToken);
                var airTemperature = await _device.RequestDataAsync<IntakeAirTemperature>();
                await Task.Delay(20, stoppingToken);
                var ambientAirTemperature = await _device.RequestDataAsync<AmbientAirTemperature>();
                await Task.Delay(20, stoppingToken);
                var engineOilTemperature = await _device.RequestDataAsync<EngineOilTemperature>();
                await Task.Delay(20, stoppingToken);
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

                if (rpm is not null)
                {
                    _ioTService?.UpdateLed(rpm.Rpm.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured when trying to either retrieve OBD values or send them through websockets");
            }
            finally
            {
                // wait 1 second before starting pulling again.
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task<bool> TryConnectWsAsync(int count, string vin, CancellationToken token)
    {
        while (count > 0)
        {
            try
            {
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
            finally
            {
                await Task.Delay(5000, token);
            }
        }

        _logger.LogInformation("Connected with the WebSocket");
        return true;
    }

    private async Task<bool> TryConnectElmAsync(int count, CancellationToken token)
    {
        while (count > 0)
        {
            try
            {
                // ReSharper disable once MethodHasAsyncOverload [Reason: Async Overload is not supported]
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
            finally
            {
                await Task.Delay(5000, token);
            }
        }

        _logger.LogInformation("Connected with the ELM327 device");
        return true;
    }
}