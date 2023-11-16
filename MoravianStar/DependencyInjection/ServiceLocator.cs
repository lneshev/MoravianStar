using MoravianStar.Resources;
using System;
using System.Threading;

namespace MoravianStar.DependencyInjection
{
    public class ServiceLocator
    {
        private static readonly AsyncLocal<Func<IServiceProvider>> func = new AsyncLocal<Func<IServiceProvider>>();

        public ServiceLocator(Func<IServiceProvider> scopedServiceProviderFunc)
        {
            func.Value = scopedServiceProviderFunc;
        }

        public static IServiceProvider Container
        {
            get
            {
                if (func.Value == null)
                {
                    throw new InvalidOperationException(Strings.ServiceLocatorWasNotInitialized);
                }
                return func.Value();
            }
        }
    }
}