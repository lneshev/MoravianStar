using Microsoft.AspNetCore.Mvc.Filters;
using MoravianStar.WebAPI.Extensions;
using System;

namespace MoravianStar.WebAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ElmahRaiseLogAttribute : ActionFilterAttribute
    {
        public ElmahRaiseLogAttribute(string message = null, bool raiseOnError = false)
        {
            Message = message;
            RaiseOnError = raiseOnError;
        }

        public string Message { get; }
        public bool RaiseOnError { get; }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);

            if (context.Exception == null || RaiseOnError)
            {
                ElmahExtensions.RaiseLog(Message);
            }
        }
    }
}