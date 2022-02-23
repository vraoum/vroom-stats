using Microsoft.AspNetCore.Mvc;
using VroomStats.Services;

namespace VroomStats.Controllers;

/// <summary>
/// Represents the controller to interact with the API through WebSockets.
/// </summary>
[ApiController]
[Route("api/v1/ws")]
public class WsController : ControllerBase
{
    private readonly IWsHandlerService _wsHandler;

    /// <summary>
    /// Creates a new instance of the <see cref="WsController"/>.
    /// </summary>
    /// <param name="wsHandler">Instance of <see cref="IWsHandlerService"/>.</param>
    public WsController(IWsHandlerService wsHandler)
    {
        _wsHandler = wsHandler;
    }
    
    /// <summary>
    /// Gets a WebSocket connection asynchronously.
    /// </summary>
    /// <param name="carId">Id of the car.</param>
    [HttpGet("{carId}")]
    public async Task GetWebSocketAsync(string carId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _wsHandler.ListenAsync(carId, webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}