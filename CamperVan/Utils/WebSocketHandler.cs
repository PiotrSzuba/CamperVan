using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CamperVan.Services.BackgroundWorkers;
using Newtonsoft.Json.Linq;

namespace CamperVan.Utils;

public static class WebSocketHandler
{
    public static async Task Echo<T>(WebSocket webSocket, IHostedServiceWorker<T> hostedService)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult? result = null;
        T? OldData = default;
        while (true)
        {
            var data = await Task.Run(() => hostedService.GetData());
            if (data == null)
            {
                continue;
            }
            if (EqualityComparer<T>.Default.Equals(OldData, default(T)))
            {
                OldData = data;
                continue;
            }
            if(EqualityComparer<T>.Default.Equals(OldData, data))
            {
                continue;
            }
            OldData = data;
            var serverMsg = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
            await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), WebSocketMessageType.Text, true, CancellationToken.None);

            var task = webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (await Task.WhenAny(task, Task.Delay(1000)) == task)
            {
                result = await task;
                if (!result.CloseStatus.HasValue)
                {
                    break;
                }
            }
        }
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }
}
