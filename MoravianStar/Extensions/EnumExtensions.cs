using MoravianStar.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace MoravianStar.Extensions
{
    public static class EnumExtensions
    {
        public static List<EnumNameValue> AllEnumsAsJson()
        {
            var results = new List<EnumNameValue>();

            var enumTypes = GetAllEnumTypes();

            foreach (var enumType in enumTypes)
            {
                results.Add(EnumToJson(enumType));
            }

            return results;
        }

        public static EnumNameValue EnumToJson(this Type type)
        {
            if (!type.IsEnum)
            {
                throw new InvalidOperationException(Strings.TheSpecifiedTypeIsNotAnEnum);
            }

            var results = Enum.GetValues(type)
                              .Cast<object>()
                              .ToDictionary(enumValue => enumValue.ToString(), enumValue => (int)enumValue);

            return new EnumNameValue()
            {
                Name = type.Name,
                Values = results
            };
        }

        public static List<EnumTextValue> GetEnumValues(string enumName, List<int> exactEnumValues, Type stringResourceType, bool sortByText = false)
        {
            if (string.IsNullOrWhiteSpace(enumName))
            {
                throw new ArgumentNullException(nameof(enumName));
            }

            var result = new List<EnumTextValue>();

            var enumType = GetAllEnumTypes().SingleOrDefault(x => x.Name == enumName);
            if (enumType == null)
            {
                throw new InvalidOperationException(string.Format(Strings.AnEnumWithNameDoesNotExist, enumName));
            }

            var values = Enum.GetValues(enumType);

            foreach (var value in values)
            {
                if (exactEnumValues != null && exactEnumValues.Count > 0)
                {
                    if (!exactEnumValues.Contains((int)value))
                    {
                        continue;
                    }
                }
                result.Add(new EnumTextValue()
                {
                    Value = (int)value,
                    StringValue = value.ToString(),
                    Text = Translate(value, stringResourceType)
                });
            }

            if (sortByText)
            {
                result = result.OrderBy(x => x.Text).ToList();
            }

            return result;
        }

        public static string Translate<TEnum>(this TEnum enumValue, Type stringResourceType)
            where TEnum : Enum
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException(Strings.TheSpecifiedTypeIsNotAnEnum, nameof(TEnum));
            }
            
            return Translate((object)enumValue, stringResourceType);
        }

        public static bool IsIn<TEnum>(this TEnum enumValue, params TEnum[] values)
            where TEnum : Enum
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException(Strings.TheSpecifiedTypeIsNotAnEnum, nameof(TEnum));
            }

            return values.Length > 0 && values.Contains(enumValue);
        }

        private static IEnumerable<Type> GetAllEnumTypes()
        {
            var moravianStarEnumTypes = Assembly.GetExecutingAssembly()
                                   .GetTypes()
                                   .Where(x => x.IsEnum)
                                   .OrderBy(x => x.Name);

            var dependentProjectEnumTypes = Assembly.GetAssembly(Settings.Settings.AssemblyForEnums.GetType())
                .GetTypes()
                .Where(x => x.IsEnum)
                .OrderBy(x => x.Name);

            return moravianStarEnumTypes.Concat(dependentProjectEnumTypes);
        }

        private static string Translate(object enumValue, Type stringResourceType)
        {
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