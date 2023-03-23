using System;
using System.Resources;

namespace MoravianStar.Extensions
{
    public static class BoolExtensions
    {
        public static string Translate(this bool? boolValue, Type stringResourceType)
        {
            return boolValue.HasValue ? Translate(boolValue.Value, stringResourceType) : string.Empty;
        }

        public static string Translate(this bool boolValue, Type stringResourceType)
        {
            string key = nameof(Boolean) + '_' + boolValue.ToString();
            var rm = new ResourceManager(stringResourceType);
            string result = rm.GetString(key);
            if (string.IsNullOrEmpty(result))
            {
                result = boolValue.ToString();
            }
            return result;
        }
    }
}