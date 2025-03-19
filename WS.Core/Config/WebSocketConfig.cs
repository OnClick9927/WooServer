namespace WS.Core.Config;

public class WebSocketConfig
{
    public int QueueSize { get; set; } = 4 * 1024;
    public int AutoDisconnectTime { get; set; } = 4;
}
