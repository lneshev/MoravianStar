using Microsoft.AspNetCore.Mvc;
using MoravianStar.Extensions;
using MoravianStar.Resources;
using MoravianStar.WebAPI.Constants;
using System;
using System.Collections.Generic;

namespace MoravianStar.WebAPI.Controllers
{
    [ApiController]
    [Route(RoutingConstants.ApiController)]
    public class EnumsController : ControllerBase
    {
        [HttpGet]
        public virtual ActionResult<List<EnumNameValue>> Get()
        {
            var enumsAsJson = EnumExtensions.AllEnumsAsJson();
            return enumsAsJson;
        }

        [HttpGet("{enumName}")]
        public virtual ActionResult<List<EnumTextValue>> Get([FromRoute] string enumName, [FromQuery] List<int> exactEnumValues, [FromQuery] bool sortByText = false)
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