using MoravianStar.Resources;
using System;
using System.Threading;

namespace MoravianStar.DependencyInjection
{
    public class ServiceLocator
    {
        private static AsyncLocal<IServiceProvider> container { get; set; }

        public ServiceLocator(IServiceProvider scopedServiceProvider)
        {
            container = new AsyncLocal<IServiceProvider>()
            {
                Value = scopedServiceProvider
            };
        }

        public static IServiceProvider Container
        {
            get
            {
                if (container == null || container.Value == null)
                {
                    throw new InvalidOperationException(Strings.ServiceLocatorWasNotInitialized);
                }
                return container.Value;
            }
        }
    }
}