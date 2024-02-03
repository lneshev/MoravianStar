using System;
using System.Resources;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with booleans.
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Translates a boolean value.
        /// In the string resource file, you should put the following two keys: "Boolean_True" and "Boolean_False".
        /// </summary>
        /// <param name="boolValue">The value.</param>
        /// <param name="stringResourceType">The string resource type (the .resx file) from where the values will be taken.</param>
        /// <returns>The translated value.</returns>
        public static string Translate(this bool? boolValue, Type stringResourceType)
        {
            return boolValue.HasValue ? Translate(boolValue.Value, stringResourceType) : string.Empty;
        }

        /// <summary>
        /// Translates a boolean value.
        /// In the string resource file, you should put the following two keys: "Boolean_True" and "Boolean_False".
        /// </summary>
        /// <param name="boolValue">The value.</param>
        /// <param name="stringResourceType">The string resource type (the .resx file) from where the values will be taken.</param>
        /// <returns>The translated value.</returns>
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