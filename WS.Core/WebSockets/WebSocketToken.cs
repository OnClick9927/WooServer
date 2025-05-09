using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace WS.Core.WebSockets;

public class WebSocketToken
{
    public WebSocket socket;
    public HttpContext context;
    internal WebSocketChannel channel;
    internal DateTime LastTime;
    public object userData { get; internal set; }

    public WebSocketToken(WebSocket socket, HttpContext context)
    {
        this.socket = socket;
        this.context = context;
    }
    public Task Send(int id, int sid, object msg)
    {
        return channel.Send(id, sid, msg);
    }
    public Task Send(ArraySegment<byte> bytes, WebSocketMessageType messageType, bool endOfMessage)
    {
        return channel.Send(bytes, messageType, endOfMessage);
    }
    public Task Close(string? statusDescription, WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure)
    {
        return channel.Close(statusDescription, status);
    }

}
