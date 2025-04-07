namespace WS.Core.WebSockets;

class RecieveMessageQueue
{
    private class MessageContext
    {
        public WebSocketToken? token;
        public int id;
        public int sid;
        public object? message;

        public MessageContext(WebSocketToken? token, int id, int sid, object? message)
        {
            this.token = token;
            this.id = id;
            this.sid = sid;
            this.message = message;
        }
    }
    private Queue<MessageContext> _messages = new Queue<MessageContext>();
    private enum State
    {
        Busy, Free
    }
    private State state = State.Free;
    public void Enqueue(WebSocketToken token, int id, int sid, object msg)
    {
        if (_clear) return;
        MessageContext context = new MessageContext(token, id, sid, msg);

        lock (_messages)
            _messages.Enqueue(context);
        if (state == State.Busy) return;
        HandleMessage();
    }
    private bool _clear;
    private async void HandleMessage()
    {
        if (_clear) return;
        MessageContext context = null;
        lock (_messages)
        {
            var count = _messages.Count;
            state = count == 0 ? State.Free : State.Busy;
            if (state == State.Free) return;
            context = _messages.Dequeue();
        }

        Task task = WebSocketTool.ExecuteMsg(context.token, context.id, context.sid, context.message);
        if (task != null)
            await task;
        HandleMessage();
    }

    internal void Clear()
    {
        _clear = true;
        lock (_messages)
        {
            _messages.Clear();
        }
    }
}

