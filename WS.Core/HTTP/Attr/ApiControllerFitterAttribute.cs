namespace WS.Core.HTTP;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ApiControllerFitterAttribute : Attribute
{
    public int ServerType { get; set; }

    public ApiControllerFitterAttribute(int serverType)
    {
        ServerType = serverType;
    }
}

