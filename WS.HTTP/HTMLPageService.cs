using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using WS.Core.Tool;

namespace WS.Core.Service;

class HTMLPageService : IApplicationConfig
{
    private ILogger logger = LogTools.CreateLogger<HTMLPageService>();

    void IApplicationConfig.Config(WebApplication application)
    {

        application.UseDefaultFiles();
        application.UseStaticFiles();

    }

}
