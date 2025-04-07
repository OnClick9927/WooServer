using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.Core.WebSockets;


class WebSocketConfiguration : IApplicationConfiguration
{
    private ILogger logger = LogTools.CreateLogger<WebSocketConfiguration>();
    private readonly IOptionsSnapshot<RootConfig> root;

    public WebSocketConfiguration(IOptionsSnapshot<RootConfig> root)
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
                var context_ws = new WebSocketChannel(new WebSocketToken(webSocket, context), root.Value.WebSocket.QueueSize, root.Value.WebSocket.AutoDisconnectTime);

                try
                {
                    await context_ws.BeginRec();
                }
                catch (Exception e)
                {
                    if (e is System.OperationCanceledException)
                        await next(context);
                    else
                    {
                        logger.LogCritical($" Server ERR {e.GetType().FullName} \n {e.Message}\n {e}");
                        throw;
                    }
                }
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
    void IApplicationConfiguration.Configure(WebApplication application)
    {
        application.UseWebSockets(new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromDays(5)
        });
        application.Use(Accept);
        WebSocketTool.CreateMessageHandlers(application.Services);
    }
    void IApplicationConfiguration.ConfigureServices(IServiceCollection services)
    {
        WebSocketTool.ConfigService(services);
    }
    ServerType IApplicationConfiguration.Fit()
    {
        return ServerType.Game;
    }
}
