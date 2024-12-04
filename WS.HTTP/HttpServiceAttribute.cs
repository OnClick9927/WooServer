using Microsoft.Extensions.DependencyInjection;
using WS.Core;
using WS.Core.Config;

namespace WS.HTTP;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]

public class HttpServiceAttribute : ServiceAttribute
{
    public string route { get; private set; }
    public ServerType rpcServerType { get; private set; }

    public HttpServiceAttribute(ServerType type, string route = "") : base(ServiceLifetime.Singleton)
    {
        rpcServerType = type;
        this.route = route;
    }
}
