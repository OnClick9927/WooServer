using Microsoft.Extensions.DependencyInjection;
using WS.Core;

namespace WS.WebSockets;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class WebSocketHandlerAttribute : ServiceAttribute
{
    public WebSocketHandlerAttribute() : base(ServiceLifetime.Singleton)
    {
    }
}
