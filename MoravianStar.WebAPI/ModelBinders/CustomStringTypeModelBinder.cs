using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.ModelBinders
{
    /// <summary>
    /// Custom <see cref="string"/> type model binder, that is used for implementing various logics around deserialization of <see cref="string"/> properties.
    /// </summary>
    public class CustomStringTypeModelBinder : IModelBinder
    {
        private readonly IModelBinder modelBinder;

        public CustomStringTypeModelBinder(IModelBinder modelBinder)
        {
            this.modelBinder = modelBinder;
        }

        public Task BindModelAsync(ModelBindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var valueProviderResult = context.ValueProvider.GetValue(context.ModelName);

            if (valueProviderResult.FirstValue is string str && !string.IsNullOrEmpty(str))
            {
                context.Result = ModelBindingResult.Success(str.Trim());
                return Task.CompletedTask;
            }
            return modelBinder.BindModelAsync(context);
        }
    }
}