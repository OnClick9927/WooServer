using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WS.Core.Config;

namespace WS.Core;

public interface IApplication
{
    void ConfigureApplication(WebApplication web_application);
    void ConfigureApplicationServices(IServiceCollection services);
    void OnEnter();
    void OnShutDown();
}
public interface IApplicationConfiguration
{

    ServerType Fit();
    void ConfigureServices(IServiceCollection services);

    void Configure(WebApplication application);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ServiceAttribute : Attribute
{
    public readonly ServiceLifetime Lifetime;
    public readonly ServerType ServerType;
    public ServiceAttribute(ServiceLifetime lifetime, ServerType serverType)
    {
        Lifetime = lifetime;
        ServerType = serverType;
    }
}
