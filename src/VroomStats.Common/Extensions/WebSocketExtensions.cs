using System.Net.WebSockets;
using System.Text;

namespace VroomStats.Extensions;

public static class WebSocketExtensions
{
    /// <summary>
    /// Receives an entire UTF8 string payload from the WebSocket.
    /// </summary>
    /// <param name="webSocket">Instance of the WebSocket connection.</param>
    /// <returns>The last WebSocket result along with the payload.</returns>
    public static async Task<(WebSocketReceiveResult, string)> ReceiveUtf8StringAsync(this WebSocket webSocket)
    {
        var messageBuffer = Array.Empty<byte>();
        var buffer = new byte[1024 * 4];

        WebSocketReceiveResult result;
        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.CloseStatus.HasValue)
            {
                return (result, null)!;
            }

            Array.Resize(ref messageBuffer, messageBuffer.Length + result.Count);
            Array.Copy(buffer, 0, messageBuffer, messageBuffer.Length - result.Count, result.Count);
        } while (!result.EndOfMessage);

        return (result, Encoding.UTF8.GetString(messageBuffer));
    }   
}