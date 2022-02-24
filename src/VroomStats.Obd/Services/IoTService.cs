using System.Device.Gpio;

namespace VroomStats.Obd.Services;

public class IoTService : IIoTService
{
    private readonly GpioController _controller;
    
    private const int RedLedPin = 5;
    private const int GreenLedPin = 13;
    private const int BlueLedPin = 6;

    public IoTService(GpioController controller)
    {
        _controller = controller;
    }

    public void Initialize()
    {
        _controller.OpenPin(RedLedPin);
        _controller.OpenPin(GreenLedPin);
        _controller.OpenPin(BlueLedPin);
    }
    
    public void UpdateLed(double rpm)
    {
        switch (rpm)
        {
            case < 1500:
                _controller.Write(RedLedPin, PinValue.Low);
                _controller.Write(GreenLedPin, PinValue.High);
                _controller.Write(BlueLedPin, PinValue.Low);
                break;
            case >= 1500 and < 2500:
                _controller.Write(RedLedPin, PinValue.High);
                _controller.Write(GreenLedPin, PinValue.High);
                _controller.Write(BlueLedPin, PinValue.Low);
                break;
            default:
                _controller.Write(RedLedPin, PinValue.High);
                _controller.Write(GreenLedPin, PinValue.Low);
                _controller.Write(BlueLedPin, PinValue.Low);
                break;
        }
    }
}