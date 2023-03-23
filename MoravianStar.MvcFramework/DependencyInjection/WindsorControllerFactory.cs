using Castle.MicroKernel;
using MoravianStar.MvcFramework;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MoravianStar.MvcFramework.DependencyInjection
{
    internal class WindsorControllerFactory : DefaultControllerFactory
    {
        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            kernel.ReleaseComponent(controller);
        }

        #region Protected members
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, string.Format(Strings.TheControllerForPathCouldNotBeFound, requestContext.HttpContext.Request.Path));
            }
            return kernel.HasComponent(controllerType) ? (IController)kernel.Resolve(controllerType) : base.GetControllerInstance(requestContext, controllerType);
        }
        #endregion

        #region Private members
        private readonly IKernel kernel;
        #endregion
    }
}