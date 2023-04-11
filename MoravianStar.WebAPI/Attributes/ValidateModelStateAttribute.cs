using Microsoft.AspNetCore.Mvc.Filters;
using MoravianStar.Exceptions;
using System;
using System.Linq;

namespace MoravianStar.WebAPI.Attributes
{
    /// <summary>
    /// An attribute which collects all model state errors, creates a message from them and throws an <see cref="InvalidModelStateException"/> with that message.
    /// The generic exception handling catches the thrown exception in a next step.
    /// </summary>
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorList = context.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var errorText = string.Join(Environment.NewLine, errorList);

                throw new InvalidModelStateException(errorText);
            }
        }
    }
}