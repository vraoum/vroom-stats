using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using VroomStats.Extensions;
using VroomStats.Payloads;

namespace VroomStats.Services;

/// <inheritdoc cref="IWsHandlerService"/>
public class WsHandlerService : IWsHandlerService
{
    private readonly ILogger<WsHandlerService> _logger;
    private readonly IDatabaseService _database;
    
    private readonly ConcurrentDictionary<string, List<WebSocket>> _sessionsPerCar;

    public WsHandlerService(ILogger<WsHandlerService> logger, IDatabaseService database)
    {
        _logger = logger;
        _database = database;
        _sessionsPerCar = new ConcurrentDictionary<string, List<WebSocket>>();
    }
    
    /// <inheritdoc cref="IWsHandlerService.ListenAsync"/>
    public async Task ListenAsync(string carId, WebSocket webSocket)
    {
        if (!TryAddCarSession(carId, webSocket))
        {
            _logger.LogCritical("An unexpected error occured when trying to cache the WebSocket session. Aborting current connection");
            webSocket.Abort();
        }
        
        var (result, content) = await webSocket.ReceiveUtf8StringAsync().ConfigureAwait(false);
        while (!result.CloseStatus.HasValue)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<BasePayload>(content)!;
                _logger.LogDebug("Received WS payload {OpCode} with {Count} data", 
                    payload.OpCode, payload.Data.Count);

                if (payload.OpCode != OpCode.Data)
                {
                    (result, content) = await webSocket.ReceiveUtf8StringAsync();
                    continue;
                }
                
                await _database.AppendDataAsync(carId, payload);
                await DispatchAsync(carId, webSocket, payload);
                
                (result, content) = await webSocket.ReceiveUtf8StringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured when handling a WS payload");
            }
        }

        await CloseCarSessionAsync(carId, webSocket);
    }

    /// <summary>
    /// Dispatches the received payload to the other connected sessions for the specified car id.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="webSocket">Instance of the WebSocket session that sent the payload.</param>
    /// <param name="payload">Instance of the Payload to dispatch.</param>
    private async Task DispatchAsync(string carId, WebSocket webSocket, BasePayload payload)
    {
        var sessions = _sessionsPerCar.GetValueOrDefault(carId);
        if (sessions is null or {Count: <= 0})
        {
            return;
        }

        payload = new BasePayload(OpCode.Dispatch, payload.Data);
        
        foreach (var session in sessions.Where(x => x != webSocket))
        {
            await session.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
    
    /// <summary>
    /// Attempts to add the WebSocket session to the session manager for the specified car.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="webSocket">Instance of the WebSocket connection.</param>
    /// <returns>A boolean indicating whether the operation was successful or not.</returns>
    private bool TryAddCarSession(string carId, WebSocket webSocket)
    {
        if (!_sessionsPerCar.TryGetValue(carId, out var webSockets))
        {
            webSockets = new List<WebSocket>();
            _sessionsPerCar.TryAdd(carId, webSockets);
        }

        if (webSockets.Contains(webSocket))
        {
            return false;
        }
        
        webSockets.Add(webSocket);
        return true;
    }

    /// <summary>
    /// Removes the WebSocket session from the cache.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    /// <param name="webSocket">Instance of the WebSocket connection to close.</param>
    private async Task CloseCarSessionAsync(string carId, WebSocket webSocket)
    {
        if (_sessionsPerCar.TryGetValue(carId, out var webSockets))
        {
            webSockets.Remove(webSocket);
        }
        
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "CLOSE", CancellationToken.None);
    }
}