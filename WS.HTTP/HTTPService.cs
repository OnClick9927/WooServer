using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WS.Core;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.HTTP;

 class HTTPService : IApplicationConfig
{
    private ILogger logger = LogTools.CreateLogger<HTTPService>();
    private readonly IOptionsSnapshot<RootConfig> root;

    public HTTPService(IOptionsSnapshot<RootConfig> root)
    {
        this.root = root;
    }

    void IApplicationConfig.Config(WebApplication application)
    {
        HTTPTool.InitRoute(application.Services, application, root.Value.ServerType);

    }


}
