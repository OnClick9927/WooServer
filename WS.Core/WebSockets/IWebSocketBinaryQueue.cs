using Microsoft.Extensions.Logging;

namespace WS.Core.WebSockets;

public interface IWebSocketBinaryQueue
{
    void OnBinaryMessage(byte[] buffer, int offset, int len);
    void Init(int size);
    ArraySegment<byte> Unpack();
}
public interface IWebSocketMsgPacker
{
    (int id, int sid, object msg, bool succeed) Decode(ArraySegment<byte> buffers);
    ArraySegment<byte> Encode(int id, int sid, object msg);
    void Release(ArraySegment<byte> bytes);
}
public interface IWebSocketTextQueue
{
    void OnTextMessage(WebSocketToken token, bool endOfMessage, byte[] buffer, int offset, int len);
    void Init(int size);
}


interface IWebSocketTokenCollection
{
    IEnumerable<WebSocketToken> GetTokens();
    void Refresh(WebSocketToken token, DateTime time);
    void Remove(WebSocketToken token);

    void Bind(WebSocketToken token, long userData);
    WebSocketToken? Find(long userData);
}

class WebSocketTokenCollection : IWebSocketTokenCollection
{
    private List<WebSocketToken> tokens = new List<WebSocketToken>();

    public void Bind(WebSocketToken token, long userData)
    {
        token.userData = userData;
    }

    public WebSocketToken? Find(long userData)
    {
        return tokens.FirstOrDefault(x => x.userData == userData);
    }

    public IEnumerable<WebSocketToken> GetTokens()
    {
        return tokens;
    }

    public void Refresh(WebSocketToken token, DateTime time)
    {
        if (tokens.Contains(token)) return;
        tokens.Add(token);
    }

    public void Remove(WebSocketToken token)
    {
        tokens.Remove(token);
    }
}


public interface IWebSocketMessageErrHandler
{
    bool Handle(WebSocketToken token, int id, int sid, object msg, Exception exception);
}
public interface IWebSocketMessageLogLevel
{
    LogLevel GetMessageLogLevel(int id, int sid, object msg);
}