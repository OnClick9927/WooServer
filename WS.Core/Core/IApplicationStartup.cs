using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WS.Core;

public interface IApplicationStartup
{
    void FitConfigTypes(List<Type> serviceTypes, List<Type> configTypes);
    void BeforeRun(WebApplication web_application);
    void BeforeBuildWebApplication();
    void ConfigServices(IServiceCollection services);
}