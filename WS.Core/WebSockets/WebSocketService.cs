using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WS.Core;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.WebSockets;


public class WebSocketService : IApplicationServiceConfig, IApplicationConfig
{
    private ILogger logger = LogTools.CreateLogger<WebSocketService>();
    private readonly IOptionsSnapshot<RootConfig> root;

    public WebSocketService(IOptionsSnapshot<RootConfig> root)
    {
        this.root = root;
    }

    private async Task Accept(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var context_ws = new WebSocketChannel(new WebSocketToken(webSocket, context), root.Value.Current.webSocketQueueSize);
                await context_ws.BeginRec();
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await next(context);
        }
    }
    void IApplicationConfig.Config(WebApplication application)
    {
        application.UseWebSockets(new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromDays(5)
        });
        application.Use(Accept);
        WebSocketTool.Init(application.Services);
    }

    void IApplicationServiceConfig.ConfigureServices(IServiceCollection services)
    {
        WebSocketTool.ConfigService(services);
    }
}
