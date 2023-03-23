using Castle.Windsor;
using System.Collections.Generic;
using System.Linq;

namespace MoravianStar.DependencyInjection
{
    public static class CastleWindsorExtensions
    {
        public static List<T> TryResolveAll<T>(this IWindsorContainer container)
            where T : class
        {
            var result = new List<T>();

            if (container != null && container.Kernel.HasComponent(typeof(T)))
            {
                result = container.ResolveAll<T>().ToList();
            }

            return result;
        }
    }
}