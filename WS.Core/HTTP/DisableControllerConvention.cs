using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;


namespace WS.Core.HTTP;

class DisableControllerConvention : IControllerModelConvention
{
    class HttpResponseExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // 直接返回404，不执行控制器代码
            context.Result = new NotFoundResult();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // 无需实现
        }
    }
    public void Apply(ControllerModel controller)
    {
        // 根据条件禁用特定控制器
        //if (controller.ControllerType == typeof(ExperimentalController) &&
        //    !ShouldEnableExperimentalController())
        if (!ShouldEnableExperimentalController(controller.ControllerType))
        {
            // 方法1: 添加阻止访问的筛选器
            controller.Filters.Add(new HttpResponseExceptionFilter());

            // 方法2: 移除所有动作方法
            controller.Actions.Clear();

      

            // 方法3: 标记为不可见（对API Explorer有效）
            controller.ApiExplorer.IsVisible = false;
        }
    }

    private bool ShouldEnableExperimentalController(Type type)
    {
        var ServerType = Context.ServerType;
        var find = type.GetCustomAttributes(typeof(ApiControllerFitterAttribute))
               .Select(x => x as ApiControllerFitterAttribute)
               .FirstOrDefault(x => x.ServerType == ServerType || x.ServerType == ServiceAttribute.AllServer);
        return find != null;


    }
}

