namespace WS.Core.WebSockets;

public interface IWebSocketTextQueue
{
    void OnTextMessage(WebSocketToken token, bool endOfMessage, byte[] buffer, int offset, int len);
    void Init(int size);
}