
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace StreamMaster.API.Services;
public class SMWebSocketManager
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();

    public SMWebSocketManager()
    {
        // Constructor for DI
    }

    // Handle WebSocket connections
    public async Task HandleWebSocketAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            Guid clientId = Guid.NewGuid();
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            _clients.TryAdd(clientId, webSocket);
            Console.WriteLine($"Client connected: {clientId}");

            await ListenToClientAsync(webSocket, clientId);
        }
        else
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("WebSocket connection expected.");
        }
    }

    // Broadcast "reload" message to all clients
    public async Task BroadcastReloadAsync()
    {
        byte[] message = Encoding.UTF8.GetBytes("reload");
        List<Task> tasks = [];

        foreach (WebSocket client in _clients.Values)
        {
            if (client.State == WebSocketState.Open)
            {
                tasks.Add(client.SendAsync(
                    new ArraySegment<byte>(message),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None));
            }
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("Broadcasted 'reload' to all clients.");
    }

    private async Task ListenToClientAsync(WebSocket webSocket, Guid clientId)
    {
        byte[] buffer = new byte[1024 * 4];
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"Client disconnected: {clientId}");
                    _clients.TryRemove(clientId, out _);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error with client {clientId}: {ex.Message}");
            _clients.TryRemove(clientId, out _);
        }
    }
}
