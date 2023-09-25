using MoravianStar.Resources;
using System;

namespace MoravianStar.DependencyInjection
{
    public class ServiceLocator : IDisposable
    {
        private static IServiceProvider container { get; set; }

        public ServiceLocator(IServiceProvider serviceProvider)
        {
            container = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            container = null;
        }

        public static IServiceProvider Container
        {
            get
            {
                if (container == null)
                {
                    throw new InvalidOperationException(Strings.ServiceLocatorWasNotInitialized);
                }
                return container;
            }
        }
    }
}