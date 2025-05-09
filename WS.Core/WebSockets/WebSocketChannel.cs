using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using WS.Core.Tool;
using static WS.Core.Tool.TimeTool;

namespace WS.Core.WebSockets;

class WebSocketChannel : ITimeEntityContext
{


    private WebSocketToken token;
    public byte[] buffer;
    private IWebSocketBinaryQueue queue_binary;
    private IWebSocketTextQueue text_queue;
    private static IWebSocketMsgPacker Packer;


    private static ILogger logger = LogTools.CreateLogger<WebSocketChannel>();

    private RecieveMessageQueue messageQueue;
    private TimeTool.TimeEntity entity;
    private double AutoDisconnectTime;

    public WebSocketChannel(WebSocketToken token, int size, double AutoDisconnectTime)
    {
        this.AutoDisconnectTime = AutoDisconnectTime;
        token.channel = this;
        this.token = token;
        buffer = new byte[size];
        queue_binary = WebSocketTool.CreateNewBinaryQueue(size);
        text_queue = WebSocketTool.CreateNewTextQueue(size);
        Packer = WebSocketTool.CreateMsagPacker();
        WebSocketTool.RefreshToken(token);
        entity = TimeTool.Add(this);
        messageQueue = new RecieveMessageQueue();

  
    }

    CancellationTokenSource tokenSource = new CancellationTokenSource();
    public async Task BeginRec()
    {
        var task = token.socket.ReceiveAsync(buffer, tokenSource.Token);
        var awaiter = task.GetAwaiter();
        while (!awaiter.IsCompleted)
        {

            await Task.Delay(1);

            if (task.Exception != null)
            {
                break;

            }

        }

        if (task.Exception != null)
        {
            Exception e = task.Exception;
            if (e is System.OperationCanceledException || e is AggregateException)
                await Close(e.Message, WebSocketCloseStatus.NormalClosure);
            else
            {
                logger.LogCritical($" Server ERR {e.GetType().FullName} \n {e.Message}\n {e}");

                throw e;
            }

        }
        else
        {

            var receiveResult = awaiter.GetResult();
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
                var result = Packer.Decode(unpack);
                logger.AlertLog(!result.succeed, "can't  decode message", LogLevel.Critical);
                if (result.succeed)
                    messageQueue.Enqueue(token, result.id, result.sid, result.msg);
                goto UNPACK;
            }
        }
        else
        {
            text_queue.OnTextMessage(token, endOfMessage, buffer, 0, len);
        }
    }
    public async Task Send(ArraySegment<byte> bytes, WebSocketMessageType messageType, bool endOfMessage)
    {
        if (token.socket.State == WebSocketState.Open)
            await token.socket.SendAsync(bytes, messageType, true, CancellationToken.None);
    }
    public async Task Send(int id, int sid, object msg)
    {

        var lev = WebSocketTool.GetMessageLogLevel(id, sid, msg);
        logger.Log(lev, $"Send Msg {id}:sid:{sid}msg:{msg.GetType()}\t{msg}");
        var bytes = Packer.Encode(id, sid, msg);
        await Send(bytes, WebSocketMessageType.Binary, true);
        Packer.Release(bytes);
    }
    public async Task Close(string? statusDescription, WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure)
    {
        if (token.socket.State == WebSocketState.Closed) return;
        messageQueue.Clear();
        tokenSource.Cancel();
        entity.InvokeComplete();
        WebSocketTool.RemoveToken(token);
        if (token.socket.State != WebSocketState.Aborted)
            await token.socket.CloseAsync(status, statusDescription, CancellationToken.None);
        else
            token.socket.Dispose();
    }

    void ITimeEntityContext.Invoke()
    {
        if ((DateTime.Now - token.LastTime).TotalSeconds > AutoDisconnectTime)
        {
            Close("Time out", WebSocketCloseStatus.NormalClosure);
        }
    }
}
