using Microsoft.Extensions.DependencyInjection;

namespace WS.Core;

public interface IApplicationServiceConfig
{
    void ConfigureServices(IServiceCollection services);

}
