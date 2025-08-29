using Microsoft.AspNetCore.Mvc;

namespace WS.Core.HTTP;

// JsonHeaderBinderAttribute.cs
public class JsonHeaderBinderAttribute : ModelBinderAttribute
{
    public JsonHeaderBinderAttribute() : base(typeof(JsonHeaderBinder))
    {
    }
}


