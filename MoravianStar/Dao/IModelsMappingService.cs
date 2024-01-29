using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public interface IModelsMappingService<TModel, TEntity>
        where TModel : class, IModelBase
        where TEntity : class, IEntityBase
    {
        Task<List<ProjectionModelPair<IProjectionBase, TModel>>> ToModels(List<ProjectionModelPair<IProjectionBase, TModel>> pairs);

        Task<List<EntityModelPair<TEntity, TModel>>> ToEntities(List<EntityModelPair<TEntity, TModel>> pairs);

        IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query);

        /// <summary>
        /// Asynchronously maps <see cref="IProjectionBase"/> to <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="projection">The source projection.</param>
        /// <returns>An instance of <typeparamref name="TModel"/> that is filled with data from the projection and optionally from other sources.</returns>
        Task<TModel> MapAsync(IProjectionBase projection);

        /// <summary>
        /// Asynchronously maps <typeparamref name="TEntity"/> to <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="entity">The source entity.</param>
        /// <returns>An instance of <typeparamref name="TModel"/> that is filled with data from the entity and optionally from other sources.</returns>
        Task<TModel> MapAsync(TEntity entity);

        /// <summary>
        /// Defines a projection of an entity, that is used in the transformation process (from entity to model).
        /// </summary>
        Expression<Func<TEntity, IProjectionBase>> Project();
    }
}