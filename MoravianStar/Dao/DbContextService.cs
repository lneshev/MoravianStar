using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoravianStar.DependencyInjection;

namespace MoravianStar.Dao
{
    public class DbContextService<TDbContext> : IDbContextService<TDbContext>
        where TDbContext : DbContext
    {
        public IDbTransaction<TDbContext> DbTransaction { get; }
        public TDbContext DbContext { get; }

        public DbContextService()
        {
            DbTransaction = ServiceLocator.Container.GetRequiredService<IDbTransaction<TDbContext>>();
            DbContext = DbTransaction.DbContext;
        }

        public IEntityRepository<TEntity, TDbContext> ForEntity<TEntity>()
            where TEntity : class, IEntityBase
        {
            return new EntityRepository<TEntity, TDbContext>(DbTransaction);
        }

        public IEntityRepository<TEntity, TId, TDbContext> ForEntity<TEntity, TId>()
            where TEntity : class, IEntityBase<TId>
        {
            return new EntityRepository<TEntity, TId, TDbContext>(DbTransaction);
        }

        public IModelRepository<TModel, TEntity, TDbContext> ForModel<TModel, TEntity>()
            where TModel : class, IModelBase, new()
            where TEntity : class, IEntityBase, IProjectionBase, new()
        {
            var modelsMappingService = ServiceLocator.Container.GetRequiredService<IModelsMappingService<TModel, TEntity>>();
            return new ModelRepository<TModel, TEntity, TDbContext>(ForEntity<TEntity>(), modelsMappingService);
        }

        public IModelRepository<TModel, TEntity, TId, TDbContext> ForModel<TModel, TEntity, TId>()
            where TModel : class, IModelBase<TId>, new()
            where TEntity : class, IEntityBase<TId>, IProjectionBase, new()
        {
            var modelsMappingService = ServiceLocator.Container.GetRequiredService<IModelsMappingService<TModel, TEntity>>();
            return new ModelRepository<TModel, TEntity, TId, TDbContext>(ForEntity<TEntity, TId>(), modelsMappingService);
        }
    }
}