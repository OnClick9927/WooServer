namespace WS.Core.WebSockets;

public interface IWebSocketMsgPacker
{
    (int id, int sid, object msg, bool succeed) Decode(ArraySegment<byte> buffers);
    ArraySegment<byte> Encode(int id, int sid, object msg);
    void Release(ArraySegment<byte> bytes);
}
