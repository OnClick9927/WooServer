namespace WS.Core.HTTP;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class RpcControllerAttribute : Attribute
{
    public int serverType { get; private set; }

    /// <summary>
    /// 只用于手机方法
    /// </summary>
    public RpcControllerAttribute()
    {

    }
    /// <summary>
    /// 收集方法，以及通过Type，找到RpcUrl
    /// </summary>
    /// <param name="serverType"></param>
    public RpcControllerAttribute(int serverType)
    {
        this.serverType = serverType;
    }
}
