using Microsoft.AspNetCore.Mvc.Filters;

namespace Qikig.App.Extension
{
    public class ActionFilter:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 在执行操作之前执行的代码
            // 例如，记录日志、验证参数等
            // context.HttpContext.Response.Headers.Add("X-Action-Filter", "Executed");
            base.OnActionExecuting(context);
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // 在执行操作之后执行的代码
            // 例如，记录日志、修改响应等
            // context.HttpContext.Response.Headers.Add("X-Action-Filter", "Executed After");
            base.OnActionExecuted(context);
        }
    }
}
