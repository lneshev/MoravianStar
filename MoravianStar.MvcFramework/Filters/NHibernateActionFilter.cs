using MoravianStar.Dao.NHibernate;
using System.Web.Mvc;

namespace MoravianStar.MvcFramework.Filters
{
    public class NHibernateActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            NHibernateSession.SessionStart();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            NHibernateSession.SessionEnd(filterContext.Exception);
        }
    }
}