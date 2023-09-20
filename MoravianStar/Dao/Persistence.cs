using Microsoft.EntityFrameworkCore;
using MoravianStar.DependencyInjection;
using MoravianStar.Resources;
using System;

namespace MoravianStar.Dao
{
    public static class Persistence
    {
        public static IDbTransaction<DbContext> DefaultDbTransaction
        {
            get
            {
                if (DefaultDbContextType == null)
                {
                    throw new ArgumentNullException(nameof(DefaultDbContextType), Strings.ADefaultDbContextTypeWasNotSet);
                }

                var serviceType = typeof(IDbTransaction<>).MakeGenericType(DefaultDbContextType);

                return (IDbTransaction<DbContext>)DependencyInjectionContext.Container.Resolve(serviceType);
            }
        }

        public static Type DefaultDbContextType { get { return Settings.Settings.DefaultDbContextType; } }

        public static IDbContextService<TDbContext> ForDbContext<TDbContext>()
            where TDbContext : DbContext
        {
            return new DbContextService<TDbContext>();
        }

        public static IEntityRepository<TEntity, DbContext> ForEntity<TEntity>()
            where TEntity : class, IEntityBase
        {
            return new EntityRepository<TEntity, DbContext>(DefaultDbTransaction);
        }

        public static IEntityRepository<TEntity, TId, DbContext> ForEntity<TEntity, TId>()
            where TEntity : class, IEntityBase<TId>
        {
            return new EntityRepository<TEntity, TId, DbContext>(DefaultDbTransaction);
        }

        public static IModelRepository<TModel, TEntity, DbContext> ForModel<TModel, TEntity>()
            where TModel : class, IModelBase, new()
            where TEntity : class, IEntityBase, IProjectionBase, new()
        {
            var modelsMappingService = DependencyInjectionContext.Container.Resolve<IModelsMappingService<TModel, TEntity>>();
            if (modelsMappingService == null)
            {
                throw new NotImplementedException(string.Format(Strings.AnInstanceForServiceWasNotFound, nameof(IModelsMappingService<TModel, TEntity>)));
            }

            return new ModelRepository<TModel, TEntity, DbContext>(ForEntity<TEntity>(), modelsMappingService);
        }

        public static IModelRepository<TModel, TEntity, TId, DbContext> ForModel<TModel, TEntity, TId>()
            where TModel : class, IModelBase<TId>, new()
            where TEntity : class, IEntityBase<TId>, IProjectionBase, new()
        {
            var modelsMappingService = DependencyInjectionContext.Container.Resolve<IModelsMappingService<TModel, TEntity>>();
            if (modelsMappingService == null)
            {
                throw new NotImplementedException(string.Format(Strings.AnInstanceForServiceWasNotFound, nameof(IModelsMappingService<TModel, TEntity>)));
            }

            return new ModelRepository<TModel, TEntity, TId, DbContext>(ForEntity<TEntity, TId>(), modelsMappingService);
        }
    }

    internal static class Test
    {
        public static void TestMethod()
        {
            var a = Persistence.ForDbContext<TestContext>().DbContext;
            var b = Persistence.ForDbContext<TestContext>().DbTransaction;
            //var c = Persistence.ForDbContext<TestContext>().ForEntity<Address>().DbContext;
            //var d = Persistence.ForDbContext<TestContext>().ForEntity<Address, int>().DbContext;
            //var e = Persistence.ForDbContext<TestContext>().ForModel<AddressModel, Address>().DbContext;
            //var f = Persistence.ForDbContext<TestContext>().ForModel<AddressModel, Address, int>().DbContext;

            Persistence.ForDbContext<TestContext>().ForEntity<Address>().ReadAsync<AddressFilter>();
            Persistence.ForDbContext<TestContext>().ForEntity<Address, int>().GetAsync(1);
            Persistence.ForDbContext<TestContext>().ForModel<AddressModel, Address>().ReadAsync<AddressFilter>();
            Persistence.ForDbContext<TestContext>().ForModel<AddressModel, Address, int>().GetAsync(1);

            var g = Persistence.DefaultDbContextType;
            var h = Persistence.DefaultDbTransaction;
            //var i = Persistence.ForEntity<Address>().DbContext;
            //var j = Persistence.ForEntity<Address, int>().DbContext;
            //var k = Persistence.ForModel<AddressModel, Address>().DbContext;
            //var l = Persistence.ForModel<AddressModel, Address, int>().DbContext;

            Persistence.ForEntity<Address>().ReadAsync<AddressFilter>();
            Persistence.ForEntity<Address, int>().GetAsync(1);
            Persistence.ForModel<AddressModel, Address>().ReadAsync<AddressFilter>();
            Persistence.ForModel<AddressModel, Address, int>().GetAsync(1);
        }
    }

    internal class TestContext : DbContext
    {
        public TestContext(DbContextOptions options) : base(options)
        {
        }

        protected TestContext()
        {
        }
    }

    internal class Address : EntityBase<int>
    {
    }

    internal class AddressModel : ModelBase<int>
    {
    }

    internal class AddressFilter : FilterSorterBase<Address>
    {
    }
}

// Persistence.GetLoader<Address>().GetAsync();
// Persistence.GetLoader<AddressModel>().GetAsync();

// Persistence.GetPersister<Address>().Save();
// Persistence.GetPersister<AddressModel>().Save();

// Persistence.ForDBContext<SystemContext>().ForEntity<Address>().GetAsync(id);
// Persistence.ForEntity<Address>().GetAsync(id);

// Persistence.ForDBContext<SystemContext>().ForModel<AddressModel>().GetAsync(id);
// Persistence.ForModel<AddressModel>().GetAsync(id);

// Persistence.ForDBContext<SystemContext>().For<Address>().GetAsync(id);
// Persistence.ForDBContext<SystemContext>().For<AddressModel>().GetAsync(id);
// Persistence.For<Address>().GetAsync(id);
// Persistence.For<AddressModel>().GetAsync(id);