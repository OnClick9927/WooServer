using Microsoft.Extensions.DependencyInjection;

namespace WS.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime;
    public ServiceAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}
