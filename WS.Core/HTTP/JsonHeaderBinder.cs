using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
//using System.Text.Json;

namespace WS.Core.HTTP;

// JsonHeaderBinder.cs
class JsonHeaderBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        // 从头部获取 JSON 字符串
        var headerName = bindingContext.FieldName;
        var headerValue = bindingContext.HttpContext.Request.Headers[headerName];

        if (StringValues.IsNullOrEmpty(headerValue))
            return Task.CompletedTask;

        try
        {
            // 反序列化 JSON
            var json = headerValue.ToString();
            var result = JsonConvert.DeserializeObject(
                json,
                bindingContext.ModelType

            );
            //new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (JsonException ex)
        {
            bindingContext.ModelState.TryAddModelError(
                bindingContext.FieldName,
                $"Invalid JSON in header: {ex.Message}"
            );
        }

        return Task.CompletedTask;
    }
}


