using Microsoft.EntityFrameworkCore;

namespace MoravianStar.Dao
{
    public interface IDbContextService<TDbContext>
        where TDbContext : DbContext
    {
        IDbTransaction<TDbContext> DbTransaction { get; }
        TDbContext DbContext { get; }

        IEntityRepository<TEntity, TDbContext> ForEntity<TEntity>()
            where TEntity : class, IEntityBase;

        IEntityRepository<TEntity, TId, TDbContext> ForEntity<TEntity, TId>()
            where TEntity : class, IEntityBase<TId>;

        IModelRepository<TModel, TEntity, TDbContext> ForModel<TModel, TEntity>()
            where TModel : class, IModelBase, new()
            where TEntity : class, IEntityBase, IProjectionBase, new();

        IModelRepository<TModel, TEntity, TId, TDbContext> ForModel<TModel, TEntity, TId>()
            where TModel : class, IModelBase<TId>, new()
            where TEntity : class, IEntityBase<TId>, IProjectionBase, new();
    }
}