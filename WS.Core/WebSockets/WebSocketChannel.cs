using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using WS.Core.Tool;

namespace WS.WebSockets;

class WebSocketChannel
{
    private WebSocketToken token;
    public byte[] buffer;
    private IWebSocketBinaryQueue queue_binary;
    private IWebSocketTextQueue text_queue;
    private static ILogger logger = LogTools.CreateLogger<WebSocketChannel>();

    public WebSocketChannel(WebSocketToken token, int size)
    {
        token.channel = this;
        this.token = token;
        buffer = new byte[size];
        queue_binary = WebSocketTool.CreateNewBinaryQueue(size);
        text_queue = WebSocketTool.CreateNewTextQueue(size);
        WebSocketTool.RefreshToken(token);
    }

    public async Task BeginRec()
    {
        var receiveResult = await token.socket.ReceiveAsync(buffer, CancellationToken.None);
        if (!receiveResult.CloseStatus.HasValue)
        {
            WebSocketTool.RefreshToken(token);
            OnRec(receiveResult.MessageType,
                receiveResult.EndOfMessage, receiveResult.Count);
            await BeginRec();
        }
        else
            await Close(receiveResult.CloseStatusDescription, receiveResult.CloseStatus.Value);
    }
    private void OnRec(WebSocketMessageType messageType, bool endOfMessage, int len)
    {

        if (messageType == WebSocketMessageType.Binary)
        {
            queue_binary.OnBinaryMessage(buffer, 0, len);

        UNPACK:
            var unpack = queue_binary.Unpack();
            if (unpack != null)
            {
                var result = WebSocketTool.Decode(unpack);
                logger.AlertLog(!result.succeed, "can't  decode message", LogLevel.Critical);
                if (result.succeed)
                    WebSocketTool.ExecuteMsg(token, result.id, result.sid, result.msg);
                goto UNPACK;
            }
        }
        else
        {
            text_queue.OnTextMessage(token, endOfMessage, buffer, 0, len);
        }
    }
    public void Send(ArraySegment<byte> bytes, WebSocketMessageType messageType, bool endOfMessage)
    {
        if (token.socket.State == WebSocketState.Open)
            token.socket.SendAsync(bytes, messageType, true, CancellationToken.None);
    }
    public void Send(int id, int sid, object msg)
    {
        var bytes = WebSocketTool.Encode(id, sid, msg);
        Send(bytes, WebSocketMessageType.Binary, true);
    }

    public async Task Close(string? statusDescription, WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure)
    {
        await token.socket.CloseAsync(status, statusDescription, CancellationToken.None);
        WebSocketTool.RemoveToken(token);

    }
}
