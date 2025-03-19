using MoravianStar.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Extension methods that are related to operations with enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Generates a file (a .json, .js, .ts file or whatever is specified in parameter <paramref name="filePath"/>) containing all enums defined in Moravian Star together with enums from your project.<br />
        /// It is useful for client projects to have a file containing all enums in the server project.
        /// This method might be called upon application startup or when a new commit is pushed to the repository, so that the client project always has the latest enums.
        /// </summary>
        /// <param name="filePath">The destination path for the generated file.</param>
        /// <param name="modifyResult">A function that may additionally modify the result.</param>
        public static async Task GenerateEnumsFileAsync(string filePath, Func<string, string> modifyResult = null)
        {
            var enumsArray = AllEnumsAsJson();
            var enumsDict = enumsArray.ToDictionary(x => x.Name, x => x.Values);
            var enumsJSON = JsonConvert.SerializeObject(enumsDict, Formatting.Indented);
            var result = modifyResult != null ? modifyResult.Invoke(enumsJSON) : enumsJSON;
            await File.WriteAllTextAsync(filePath, result);
            Console.WriteLine(string.Format(Strings.EnumsFileGeneratedAt, Path.GetFullPath(filePath)));
        }

        /// <summary>
        /// Returns all enums defined in Moravian Star (like: "SortDirection" that is used to specify the sorting direction when using the read functionality) together with enums from your project.
        /// </summary>
        /// <returns>The enums as a list of <see cref="EnumNameValue"/>s, suitable for JSON format.</returns>
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

        /// <summary>
        /// Returns an enum's name and its values.
        /// </summary>
        /// <param name="type">A type that should be an enum.</param>
        /// <returns>The enum's name and its values, suitable for JSON format.</returns>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Returns the values of a single enum in multiple formats like its integer value, its string value and its translated string value.
        /// </summary>
        /// <param name="enumName">Enum's type as a string.</param>
        /// <param name="exactEnumValues">A list of int values. Acts like a filter. When set, only those values will be returned.</param>
        /// <param name="stringResourceType">The string resource type (the .resx file) from where the values will be taken.</param>
        /// <param name="sortByText">Specifies if the values in the result should be sorted by property "text" (when passed "true") or by property "value" (when passed "false").</param>
        /// <returns>The values as a list of <see cref="EnumTextValue"/>s.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Translates an enum value.
        /// In the string resource file, you should put the keys in the following pattern: "[EnumName]_[EnumValue]".
        /// </summary>
        /// <typeparam name="TEnum">Enum's type.</typeparam>
        /// <param name="enumValue">Enum's value.</param>
        /// <param name="stringResourceType">The string resource type (the .resx file) from where the values will be taken.</param>
        /// <returns>The translated value.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string Translate<TEnum>(this TEnum enumValue, Type stringResourceType)
            where TEnum : Enum
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException(Strings.TheSpecifiedTypeIsNotAnEnum, nameof(TEnum));
            }
            
            return Translate((object)enumValue, stringResourceType);
        }

        /// <summary>
        /// Checks if an enum value is in a collection of other enum values.
        /// </summary>
        /// <typeparam name="TEnum">Enum's type.</typeparam>
        /// <param name="enumValue">The value to check.</param>
        /// <param name="values">The collection of enums.</param>
        /// <returns>True if the enum value is in the collection of enum values.</returns>
        /// <exception cref="ArgumentException"></exception>
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
            var dependentProjectAssemblyForEnums = Settings.Settings.AssemblyForEnums;
            if (dependentProjectAssemblyForEnums == null)
            {
                throw new ArgumentNullException(nameof(Settings.Settings.AssemblyForEnums), Strings.AnAssemblyForEnumsWasNotSet);
            }

            var moravianStarEnumTypes = Assembly.GetExecutingAssembly()
                                   .GetTypes()
                                   .Where(x => x.IsEnum)
                                   .OrderBy(x => x.Name);

            var dependentProjectEnumTypes = dependentProjectAssemblyForEnums
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