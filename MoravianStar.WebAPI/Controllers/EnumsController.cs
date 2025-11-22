using Microsoft.AspNetCore.Mvc;
using MoravianStar.Extensions;
using MoravianStar.WebAPI.Constants;
using MoravianStar.WebAPI.Helpers;
using System.Collections.Generic;

namespace MoravianStar.WebAPI.Controllers
{
    [ApiController]
    [Route(RoutingConstants.ApiController)]
    public class EnumsController : ControllerBase
    {
        protected readonly EnumsControllerHelper helper;

        public EnumsController()
        {
            helper = new EnumsControllerHelper();
        }

        [HttpGet]
        public virtual ActionResult<List<EnumNameValue>> Get()
        {
            List<EnumNameValue> result = helper.Get();
            return result;
        }

        [HttpGet("{enumName}")]
        public virtual ActionResult<List<EnumTextValue>> Get([FromRoute] string enumName, [FromQuery] List<int> exactEnumValues, [FromQuery] bool sortByText = false)
        {
            List<EnumTextValue> result = helper.Get(enumName, exactEnumValues, sortByText);
            return result;
        }
    }
}