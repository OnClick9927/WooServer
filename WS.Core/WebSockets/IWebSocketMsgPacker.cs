namespace WS.WebSockets;

public interface IWebSocketMsgPacker
{
    (int id, int sid, object msg, bool succeed) Decode(byte[] unpack);
    byte[] Encode(int id, int sid, object msg);
}
