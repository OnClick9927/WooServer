using WS.Core.Config;

namespace WS.Core.HTTP;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class RpcControllerAttribute : Attribute
{
    public ServerType serverType { get; private set; }

    public RpcControllerAttribute(ServerType serverType)
    {
        this.serverType = serverType;
    }
}
