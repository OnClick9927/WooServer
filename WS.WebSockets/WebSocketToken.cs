using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace WS.WebSockets;

public class WebSocketToken
{
    public WebSocket socket;
    public HttpContext context;
    internal WebSocketChannel channel;
    public WebSocketToken(WebSocket socket, HttpContext context)
    {
        this.socket = socket;
        this.context = context;
    }
    public void Send(int id, int sid, object msg)
    {
        channel.Send(id, sid, msg);
    }
    public void Send(ArraySegment<byte> bytes, WebSocketMessageType messageType, bool endOfMessage)
    {
        channel.Send(bytes, messageType, endOfMessage);
    }
    public Task Close(string? statusDescription, WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure)
    {
        return channel.Close(statusDescription, status);
    }

}
