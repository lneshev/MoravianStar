using Microsoft.AspNetCore.Mvc.Filters;
using MoravianStar.WebAPI.Extensions;
using System;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ElmahRaiseLogAsyncAttribute : Attribute, IAsyncActionFilter
    {
        public ElmahRaiseLogAsyncAttribute(string message = null, bool raiseOnError = false)
        {
            Message = message;
            RaiseOnError = raiseOnError;
        }

        public string Message { get; }
        public bool RaiseOnError { get; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();
            if (executedContext.Exception == null || RaiseOnError)
            {
                ElmahExtensions.RaiseLog(Message);
            }
        }
    }
}