using MoravianStar.MvcFramework.JsonNet;
using Newtonsoft.Json;
using System.Text;
using System.Web.Mvc;

namespace MoravianStar.MvcFramework.Controllers
{
    public abstract class BaseController : Controller
    {
        public JsonResult JsonNet(object data)
        {
            var result = new JsonNetResult
            {
                Data = data,
                Settings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            };
            return result;
        }

        public JsonResult JsonNet(object data, JsonRequestBehavior behavior)
        {
            var result = new JsonNetResult
            {
                Data = data,
                JsonRequestBehavior = behavior,
                Settings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            };
            return result;
        }

        public JsonResult JsonNet(object data, string contentType)
        {
            var result = new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                Settings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            };
            return result;
        }

        public JsonResult JsonNet(object data, JsonRequestBehavior behavior, string contentType)
        {
            var result = new JsonNetResult
            {
                Data = data,
                JsonRequestBehavior = behavior,
                ContentType = contentType,
                Settings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            };
            return result;
        }

        public JsonResult JsonNet(object data, string contentType, Encoding contentEncoding)
        {
            var result = new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                Settings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            };
            return result;
        }

        public JsonResult JsonNet(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            var result = new JsonNetResult
            {
                Data = data,
                JsonRequestBehavior = behavior,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                Settings = { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            };
            return result;
        }
    }
}