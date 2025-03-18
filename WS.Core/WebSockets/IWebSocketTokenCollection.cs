namespace WS.WebSockets;

public interface IWebSocketTokenCollection
{
    IEnumerable<WebSocketToken> GetTokens();
    void Refresh(WebSocketToken token, DateTime time);
    void Remove(WebSocketToken token);
}