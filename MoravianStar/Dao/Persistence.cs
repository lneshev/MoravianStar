using System;
using System.Linq;

namespace MoravianStar.Dao
{
    public static class Persistence
    {
        public static IPersister<TContract> GetPersister<TContract>(int id)
            where TContract : class, IIdentifier
        {
            if (typeof(PresentationModel).IsAssignableFrom(typeof(TContract)))
            {
                var entityType = typeof(TContract);
                var mapClassAttribute = (typeof(TContract).GetCustomAttributes(typeof(MapClassAttribute), false).FirstOrDefault() as MapClassAttribute);
                if (mapClassAttribute != null)
                {
                    entityType = mapClassAttribute.EntityType;
                    var persisterType = typeof(Persister<,>).MakeGenericType(entityType, typeof(TContract));
                    var persisterInstance = persisterType.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { id });

                    return (IPersister<TContract>)persisterInstance;
                }
                else
                {
                    throw new Exception(string.Format("'MapClass' attribute is not specified on class: '{0}'", entityType.FullName));
                }
            }
            else
            {
                return new Persister<TContract>(id);
            }
        }

        public static IPersister<TContract> GetPersister<TContract>(TContract contract)
            where TContract : class, IIdentifier
        {
            if (typeof(PresentationModel).IsAssignableFrom(typeof(TContract)))
            {
                var entityType = typeof(TContract);
                var mapClassAttribute = (typeof(TContract).GetCustomAttributes(typeof(MapClassAttribute), false).FirstOrDefault() as MapClassAttribute);
                if (mapClassAttribute != null)
                {
                    entityType = mapClassAttribute.EntityType;
                    var persisterType = typeof(Persister<,>).MakeGenericType(entityType, typeof(TContract));
                    var persisterInstance = persisterType.GetConstructor(new Type[] { typeof(TContract) }).Invoke(new[] { contract });

                    return (IPersister<TContract>)persisterInstance;
                }
                else
                {
                    throw new Exception(string.Format("'MapClass' attribute is not specified on class: '{0}'", entityType.FullName));
                }
            }
            else
            {
                return new Persister<TContract>(contract);
            }
        }

        public static ILoader<TContract, TFilter> GetLoader<TContract, TFilter>(TFilter filter)
            where TContract : class, IIdentifier
            where TFilter : EntityFilter
        {
            if (typeof(PresentationModel).IsAssignableFrom(typeof(TContract)))
            {
                var entityType = typeof(TContract);
                var mapClassAttribute = (typeof(TContract).GetCustomAttributes(typeof(MapClassAttribute), false).FirstOrDefault() as MapClassAttribute);
                if (mapClassAttribute != null)
                {
                    entityType = mapClassAttribute.EntityType;
                    var loaderType = typeof(Loader<,,>).MakeGenericType(entityType, typeof(TContract), typeof(TFilter));
                    var loaderInstance = loaderType.GetConstructor(new Type[] { typeof(TFilter) }).Invoke(new[] { filter });

                    return (ILoader<TContract, TFilter>)loaderInstance;
                }
                else
                {
                    throw new Exception(string.Format("'MapClass' attribute is not specified on class: '{0}'", entityType.FullName));
                }
            }
            else
            {
                return new Loader<TContract, TFilter>(filter);
            }
        }
    }
}
