using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WS.Core;

public interface IApplicationStartup
{
    void ConfigApplication(WebApplication web_application);
    void ConfigApplicationServices(IServiceCollection services);
    void OnEnter();
    void OnShutDown();
}