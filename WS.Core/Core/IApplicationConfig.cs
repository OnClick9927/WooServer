using Microsoft.AspNetCore.Builder;

namespace WS.Core;

public interface IApplicationConfig
{
    void Config(WebApplication application);
}
