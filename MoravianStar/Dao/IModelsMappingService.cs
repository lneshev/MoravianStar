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
        async Task<List<ProjectionModelPair<TProjection, TModel>>> ToModels<TProjection>(List<ProjectionModelPair<TProjection, TModel>> pairs)
            where TProjection : class, IProjectionBase
        {
            foreach (var pair in pairs)
            {
                pair.Model = await MapAsync(pair.Projection);
            }

            return pairs;
        }

        async Task<List<EntityModelPair<TEntity, TModel>>> ToEntities(List<EntityModelPair<TEntity, TModel>> pairs)
        {
            return await Task.FromResult(pairs);
        }

        IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query)
        {
            return query;
        }

        /// <summary>
        /// Asynchronously maps <see cref="IProjectionBase"/> to <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="projection">The source projection.</param>
        /// <returns>An instance of <typeparamref name="TModel"/> that is filled with data from the projection and optionally from other sources.</returns>
        async Task<TModel> MapAsync(IProjectionBase projection)
        {
            return await Task.FromResult((TModel)projection);
        }

        /// <summary>
        /// Asynchronously maps <typeparamref name="TEntity"/> to <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="entity">The source entity.</param>
        /// <returns>An instance of <typeparamref name="TModel"/> that is filled with data from the entity and optionally from other sources.</returns>
        async Task<TModel> MapAsync(TEntity entity)
        {
            var projection = Project().Compile()(entity);
            var model = await MapAsync(projection);
            return model;
        }

        /// <summary>
        /// Defines a projection of an entity, that is used in the transformation process (from entity to model).
        /// </summary>
        Expression<Func<TEntity, IProjectionBase>> Project();
    }
}