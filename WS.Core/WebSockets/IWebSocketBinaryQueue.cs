namespace WS.Core.WebSockets;

public interface IWebSocketBinaryQueue
{
    void OnBinaryMessage(byte[] buffer, int offset, int len);
    void Init(int size);
    ArraySegment<byte> Unpack();
}
