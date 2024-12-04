namespace WS.HTTP;

public class HttpMethodAttribute : Attribute
{
    public enum MethodType
    {
        GET, Post, Put, Delete,
    }
    public MethodType type { get; private set; }

    public HttpMethodAttribute(MethodType type, string name = "", bool rpc = false)
    {
        this.type = type; this.name = name; this.rpc = rpc;
    }
    public bool rpc { get; private set; }
    public string name { get; private set; }
}
