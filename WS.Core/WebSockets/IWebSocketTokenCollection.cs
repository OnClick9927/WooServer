namespace WS.Core.WebSockets;

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
