using MoravianStar.Dao;
using MoravianStar.Extensions;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MoravianStar.MvcFramework.Controllers
{
    public abstract class EnumController<TEnum, TStringResourceType> : BaseController
        where TEnum : struct, IConvertible, IFormattable
        where TStringResourceType: class
    {
        [HttpGet]
        public virtual JsonResult List()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type.", nameof(TEnum));
            }
            var enumValues = Enum.GetValues(typeof(TEnum));
            List<ValueText> result = new List<ValueText>(enumValues.Length);
            foreach (TEnum enumValue in enumValues)
            {
                result.Add(new ValueText()
                {
                    Value = enumValue.ToString("d", null),
                    Text = enumValue.Translate(typeof(TStringResourceType))
                });
            }
            
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }
    }
}