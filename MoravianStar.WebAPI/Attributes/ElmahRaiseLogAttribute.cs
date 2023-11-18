using Microsoft.AspNetCore.Mvc.Filters;
using MoravianStar.WebAPI.Extensions;
using System;

namespace MoravianStar.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ElmahRaiseLogAttribute : ActionFilterAttribute
    {
        public ElmahRaiseLogAttribute(string message = null)
        {
            Message = message;
        }

        public string Message { get; }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);

            if (context.Exception == null)
            {
                ElmahExtensions.RaiseLog(Message);
            }
        }
    }
}