using MoravianStar.Resources;
using System;

namespace MoravianStar.DependencyInjection
{
    public class ServiceLocator
    {
        public static IServiceProvider Container
        {
            get
            {
                var serviceProvider = Settings.Settings.ServiceProvider;
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException(nameof(Settings.Settings.ServiceProvider), Strings.AServiceProviderWasNotSet);
                }

                return serviceProvider;
            }
        }
    }
}