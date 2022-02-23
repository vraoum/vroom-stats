using VroomStats.Payloads;

namespace VroomStats.Obd.Services;

public interface IWsHandlerService
{
    Task ConnectAsync(string carId);
    Task SendPayloadAsync(BasePayload payload);
}