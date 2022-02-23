using System.Net.WebSockets;

namespace VroomStats.Services;

/// <summary>
/// Defines a service that handles the WebSocket connections and dispatch the payloads to the other sessions.
/// </summary>
public interface IWsHandlerService
{
    /// <summary>
    /// Listens to the WebSocket connection.
    /// </summary>
    /// <param name="carId">Id of the Car to get and broadcast streamable data.</param>
    /// <param name="webSocket">Instance of the WebSocket connection.</param>
    Task ListenAsync(string carId, WebSocket webSocket);
}