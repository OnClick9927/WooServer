using Microsoft.Extensions.DependencyInjection;

namespace WS.Core.WebSockets;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class WebSocketHandlerAttribute : ServiceAttribute
{
    public WebSocketHandlerAttribute() : base(ServiceLifetime.Singleton, Config.ServerType.Game)
    {
    }
}
