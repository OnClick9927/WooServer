namespace WS.WebSockets;

public interface IWebSocketTokenCollection
{
    void Refresh(WebSocketToken token, DateTime time);
    void Remove(WebSocketToken token);
}