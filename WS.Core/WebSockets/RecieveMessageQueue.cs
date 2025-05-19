using System.Threading.Channels;

namespace WS.Core.WebSockets;

class RecieveMessageQueue
{
    struct MessageContext
    {
        public WebSocketToken token;
        public int id;
        public int sid;
        public object message;

        public MessageContext(WebSocketToken token, int id, int sid, object message)
        {
            this.token = token;
            this.id = id;
            this.sid = sid;
            this.message = message;
        }
    }
    private Channel<MessageContext> _channel;
    private CancellationTokenSource cts;
    public RecieveMessageQueue()
    {
        cts = new CancellationTokenSource();
        _channel = Channel.CreateUnbounded<MessageContext>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = true,
        });
        HandleMessage();
    }
    public async void Enqueue(WebSocketToken token, int id, int sid, object msg)
    {
        if (cts.IsCancellationRequested) return;

        MessageContext context = new MessageContext(token, id, sid, msg);
        await _channel.Writer.WriteAsync(context);
    }
    private async Task HandleMessage()
    {
        while (true)
        {
            if (cts.IsCancellationRequested) break;

           await _channel.Reader.WaitToReadAsync();
            //await _task;
            if (cts.IsCancellationRequested) break;
            if (_channel.Reader.TryRead(out MessageContext context))
            {
                Task task = WebSocketTool.ExecuteMsg(context.token, context.id, context.sid, context.message);
                if (task != null)
                    await task;
            }
        }



    }

    internal void Clear()
    {
       
        cts.Cancel();
    }

}

