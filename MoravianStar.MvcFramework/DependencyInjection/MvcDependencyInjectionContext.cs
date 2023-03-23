using MoravianStar.DependencyInjection;
using System.Web.Mvc;

namespace MoravianStar.MvcFramework.DependencyInjection
{
    public static class MvcDependencyInjectionContext
    {
        public static void SetupForMvc(ControllerBuilder controllerBuilder)
        {
            var controllerFactory = new WindsorControllerFactory(DependencyInjectionContext.Container.Kernel);
            controllerBuilder.SetControllerFactory(controllerFactory);
        }
    }
}