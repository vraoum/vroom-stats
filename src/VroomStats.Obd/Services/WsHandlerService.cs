using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VroomStats.Extensions;
using VroomStats.Payloads;

namespace VroomStats.Obd.Services;

public class WsHandlerService : IWsHandlerService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WsHandlerService> _logger;
    private readonly IServiceProvider _services;
    
    private ClientWebSocket _webSocket;

    public WsHandlerService(IConfiguration configuration, ILogger<WsHandlerService> logger, IServiceProvider services, ClientWebSocket webSocket)
    {
        _configuration = configuration;
        _logger = logger;
        _services = services;
        _webSocket = webSocket;
    }

    public async Task ConnectAsync(string carId)
    {
        _webSocket = _services.GetRequiredService<ClientWebSocket>();
        
        var uri =
            $"ws{(_configuration["WebApi:UseSSL"] == "true" ? "s" : "")}://{_configuration["WebApi:Host"]}:{_configuration["WebApi:Port"]}/api/v1/ws/{carId}";
        
        _logger.LogInformation("Connecting to {Uri}", uri);
        await _webSocket.ConnectAsync(
            new Uri(uri),
            CancellationToken.None);
        _logger.LogInformation("Done");
        
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
                    continue;
                }

                // todo: IoT things~
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured when handling a WS payload");
                _logger.LogDebug("Bad payload received:\n{Payload}", content);
            }
            finally
            {
                (result, content) = await _webSocket.ReceiveUtf8StringAsync();
            }
        }
    }
}