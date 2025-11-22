using MoravianStar.Extensions;
using MoravianStar.Resources;
using System;
using System.Collections.Generic;

namespace MoravianStar.WebAPI.Helpers
{
    public class EnumsControllerHelper
    {
        public virtual List<EnumNameValue> Get()
        {
            var enumsAsJson = EnumExtensions.AllEnumsAsJson();
            return enumsAsJson;
        }

        public virtual List<EnumTextValue> Get(string enumName, List<int> exactEnumValues, bool sortByText = false)
        {
            var stringResourceTypeForEnums = Settings.Settings.StringResourceTypeForEnums;
            if (stringResourceTypeForEnums == null)
            {
                throw new ArgumentNullException(nameof(Settings.Settings.StringResourceTypeForEnums), Strings.AStringsResourceTypeForEnumsWasNotSet);
            }

            var enumValues = EnumExtensions.GetEnumValues(enumName, exactEnumValues, stringResourceTypeForEnums, sortByText);
            return enumValues;
        }
    }
}