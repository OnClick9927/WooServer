namespace WS.Core.Config
{
    public class WebSocketConfig
    {
        public int queueSize { get; set; } = 4 * 1024;
        public int autoDisconnectTime { get; set; } = 4;
    }
}
