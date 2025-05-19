using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WS.Core;

public interface IApplication
{
    void ConfigureApplication(WebApplication web_application);
    void ConfigureApplicationServices(IServiceCollection services);
    void OnEnter();
    Task OnShutDown();
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

[Flags]
public enum ServerType
{
    Gate = 2,
    Game = 4,
    Timer = 8,
    Pay = 16,







    All = Gate | Game | Timer | Pay,
}
