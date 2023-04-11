using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Linq;

namespace MoravianStar.WebAPI.ModelBinders
{
    public static class ModelBinderExtensions
    {
        public static void AddCustomSimpleTypeModelBinderProvider(this MvcOptions option)
        {
            var binderToFind = option.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(SimpleTypeModelBinderProvider));
            if (binderToFind == null)
            {
                return;
            }
            var index = option.ModelBinderProviders.IndexOf(binderToFind);
            option.ModelBinderProviders.Insert(index, new CustomSimpleTypeModelBinderProvider());
        }
    }
}