using System;
using System.Resources;

namespace MoravianStar.Extensions
{
    public static class EnumExtensions
    {
        public static string Translate<TEnum>(this TEnum enumValue, Type stringResourceType)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enumerated type.", nameof(TEnum));
            }
            string key = enumValue.GetType().Name + '_' + enumValue;
            var rm = new ResourceManager(stringResourceType);
            string result = rm.GetString(key);
            if (string.IsNullOrEmpty(result))
            {
                result = enumValue.ToString();
            }
            return result;
        }
    }
}