using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace MoravianStar.WebAPI.ModelBinders
{
    /// <summary>
    /// Custom model binder for simple types like <see cref="string"/>, <see cref="DateTime"/>, etc.
    /// </summary>
    public class CustomSimpleTypeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            IModelBinder result = null;

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.Metadata.IsComplexType)
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                if (context.Metadata.ModelType == typeof(string))
                {
                    result = new CustomStringTypeModelBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory));
                }
                else if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
                {
                    // TODO ? : Handle UTC
                    //result = new CustomDateTimeTypeModelBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory));
                }
            }

            return result;
        }
    }
}