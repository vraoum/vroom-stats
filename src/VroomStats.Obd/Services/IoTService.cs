using System.Device.Gpio;
using Microsoft.Extensions.Logging;

namespace VroomStats.Obd.Services;

public class IoTService : IIoTService
{
    private readonly GpioController _controller;
    private readonly ILogger<IoTService> _logger;

    private const int RedLedPin = 18;
    private const int GreenLedPin = 23;
    private const int BlueLedPin = 24;

    public IoTService(GpioController controller, ILogger<IoTService> logger)
    {
        _controller = controller;
        _logger = logger;
    }

    public void Initialize()
    {
        _logger.LogDebug("Initializing pins");
        _controller.OpenPin(RedLedPin, PinMode.Output);
        _controller.OpenPin(GreenLedPin, PinMode.Output);
        _controller.OpenPin(BlueLedPin, PinMode.Output);
        _logger.LogDebug("Pins initialized");
    }
    
    public void UpdateLed(double rpm)
    {
        _logger.LogDebug("Changing led value: {Rpm}", rpm);
        switch (rpm)
        {
            case < 1500:
                _logger.LogDebug("GREEN");
                _controller.Write(RedLedPin, PinValue.Low);
                _controller.Write(GreenLedPin, PinValue.High);
                _controller.Write(BlueLedPin, PinValue.Low);
                _logger.LogDebug("DONE");
                break;
            case >= 1500 and < 2500:
                _logger.LogDebug("RED-GREEN");
                _controller.Write(RedLedPin, PinValue.High);
                _controller.Write(GreenLedPin, PinValue.High);
                _controller.Write(BlueLedPin, PinValue.Low);
                _logger.LogDebug("DONE");
                break;
            default:
                _logger.LogDebug("RED");
                _controller.Write(RedLedPin, PinValue.High);
                _controller.Write(GreenLedPin, PinValue.Low);
                _controller.Write(BlueLedPin, PinValue.Low);
                _logger.LogDebug("DONE");
                break;
        }
    }
}