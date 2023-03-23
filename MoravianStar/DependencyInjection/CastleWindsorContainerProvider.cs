using Castle.Windsor;
using Castle.Windsor.Installer;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace MoravianStar.DependencyInjection
{
    public class CastleWindsorContainerProvider
    {
        static CastleWindsorContainerProvider()
        {
            container = new Lazy<WindsorContainer>(() =>
            {
                var result = new WindsorContainer();
                result.Kernel.ComponentModelBuilder.AddContributor(new TransientEqualizer());

                string applicationName = ConfigurationManager.AppSettings["ApplicationName"];
                string prefix = applicationName + ".";
                var ass = AppDomain.CurrentDomain.GetAssemblies();
                var appAssemblies = ass.Cast<Assembly>().Where(x => x.FullName.StartsWith(prefix) || x.FullName.StartsWith("MoravianStar")).ToList();
                foreach (var assembly in appAssemblies)
                {
                    result.Install(FromAssembly.Named(assembly.FullName));
                }

                return result;
            });
        }

        public static IWindsorContainer Container
        {
            get
            {
                return container.Value;
            }
        }

        #region Private members
        private static readonly Lazy<WindsorContainer> container;
        #endregion
    }
}