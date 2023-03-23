using MoravianStar.MvcFramework.JsonNet;
using System;
using System.Net;
using System.Web.Mvc;

namespace MoravianStar.MvcFramework.Filters
{
    public class FrameworkHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
            {
                filterContext.HttpContext.Response.StatusCode = (int)GetHttpStatusCode(filterContext.Exception);
                filterContext.Result = new JsonNetResult()
                {
                    Data = new
                    {
                        Message = filterContext.Exception.Message,
                        ExceptionType = filterContext.Exception.GetType().FullName,
                        Exception = filterContext.HttpContext.IsCustomErrorEnabled ? filterContext.Exception : null
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        #region Private members
        private HttpStatusCode GetHttpStatusCode(Exception exception)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

            switch (exception.GetType().Name)
            {
                case nameof(NotImplementedException):
                    httpStatusCode = HttpStatusCode.NotImplemented;
                    break;
                default:
                    break;
            }

            return httpStatusCode;
        }
        #endregion
    }
}