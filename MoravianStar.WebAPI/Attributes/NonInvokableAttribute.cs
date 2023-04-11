using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MoravianStar.WebAPI.Attributes
{
    /// <summary>
    /// This attribute specifies that an action cannot be invoked. It is similar to <see cref="Microsoft.AspNetCore.Mvc.NonActionAttribute"/>,
    /// but this attribute is not inherited, so overriding a non-invokable action will become invokable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NonInvokableAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            throw new MethodAccessException();
        }
    }
}