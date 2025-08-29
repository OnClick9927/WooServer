using Microsoft.Extensions.DependencyInjection;

namespace WS.Core.WebSockets;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class WebSocketServiceAttribute : ServiceAttribute
{
    public WebSocketServiceAttribute(int serverType) : base(ServiceLifetime.Singleton, serverType)
    {
    }
}


