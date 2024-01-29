using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    /// <summary>
    /// Provides methods for entity-to-model and model-to-entity transformations
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IModelsMappingService<TModel, TEntity>
        where TModel : class, IModelBase
        where TEntity : class, IEntityBase
    {
        /// <summary>
        /// Asynchronously maps many <see cref="IProjectionBase"/> to many <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="pairs">A structure that contains empty <typeparamref name="TModel"/> with its corresponding <see cref="IProjectionBase"/>.</param>
        /// <returns>The filled-in pairs.</returns>
        Task<List<ProjectionModelPair<IProjectionBase, TModel>>> ToModels(List<ProjectionModelPair<IProjectionBase, TModel>> pairs);

        /// <summary>
        /// Asynchronously maps many <typeparamref name="TModel"/> to many <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="pairs">A structure that contains empty <typeparamref name="TEntity"/> with its corresponding <typeparamref name="TModel"/>.</param>
        /// <returns>The filled-in pairs.</returns>
        Task<List<EntityModelPair<TEntity, TModel>>> ToEntities(List<EntityModelPair<TEntity, TModel>> pairs);

        /// <summary>
        /// Defines which navigation properties and collections of an entity should be included when an entity is loaded for updating or deleting.
        /// </summary>
        /// <param name="query">The query that will be executed to find an entity that is about to be updated or deleted.</param>
        /// <returns>The modified query with all needed includes.</returns>
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