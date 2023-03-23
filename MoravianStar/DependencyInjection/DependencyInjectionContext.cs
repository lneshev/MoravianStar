using Castle.Windsor;

namespace MoravianStar.DependencyInjection
{
    public static class DependencyInjectionContext
    {
        public static void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
        }

        public static IWindsorContainer Container { get { return CastleWindsorContainerProvider.Container; } }
    }
}