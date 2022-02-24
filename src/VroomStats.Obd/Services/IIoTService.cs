namespace VroomStats.Obd.Services;

public interface IIoTService
{
    void Initialize();
    void UpdateLed(double rpm);
}