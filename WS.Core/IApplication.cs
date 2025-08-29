using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WS.Core.Config;

namespace WS.Core;

public interface IApplication
{
    void ConfigureApplication(WebApplication web_application);
    void ConfigureApplicationServices(IServiceCollection services);
    void OnEnter();
    Task OnShutDown();

    bool Fit(int serverType, Type type);
    bool LegalServerConfig(ServerConfig server_config);
}
public interface IApplicationConfiguration
{

    //bool Fit(int serverType);
    void ConfigureServices(IServiceCollection services);

    void Configure(WebApplication application);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ServiceAttribute : Attribute
{
    public readonly ServiceLifetime Lifetime;
    public readonly int ServerType;
    public const int AllServer = -1;
    public bool FitAllServer => ServerType == AllServer;
    public ServiceAttribute(ServiceLifetime lifetime, int serverType = AllServer)
    {
        Lifetime = lifetime;
        ServerType = serverType;
    }
}







