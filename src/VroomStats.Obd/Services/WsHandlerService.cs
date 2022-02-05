using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VroomStats.Extensions;
using VroomStats.Payloads;

namespace VroomStats.Obd.Services;

public class WsHandlerService : IWsHandlerService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WsHandlerService> _logger;
    private readonly ClientWebSocket _webSocket;

    public WsHandlerService(IConfiguration configuration, ILogger<WsHandlerService> logger, ClientWebSocket webSocket)
    {
        _configuration = configuration;
        _logger = logger;
        _webSocket = webSocket;
    }

    public async Task ConnectAsync(string carId)
    {
        await _webSocket.ConnectAsync(
            new Uri($"ws://{_configuration["WebSocket:Host"]}:{_configuration["WebSocket:Port"]}/api/v1/ws/{carId}", UriKind.Absolute),
            CancellationToken.None);

        _ = ListenAsync();
    }

    public async Task SendPayloadAsync(BasePayload payload)
    {
        await _webSocket.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)),
            WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task ListenAsync()
    {
        var (result, content) = await _webSocket.ReceiveUtf8StringAsync().ConfigureAwait(false);
        while (!result.CloseStatus.HasValue)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<BasePayload>(content)!;
                _logger.LogDebug("Received WS payload {OpCode} with {Count} data", 
                    payload.OpCode, payload.Data.Count);

                if (payload.OpCode != OpCode.Event)
                {
                    (result, content) = await _webSocket.ReceiveUtf8StringAsync();
                    continue;
                }

                // todo: IoT things~
                (result, content) = await _webSocket.ReceiveUtf8StringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured when handling a WS payload");
            }
        }
    }
}