using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    public abstract class ModelsMappingService<TModel, TEntity> : IModelsMappingService<TModel, TEntity>
        where TModel : class, IModelBase
        where TEntity : class, IEntityBase
    {
        public async Task<List<ProjectionModelPair<TProjection, TModel>>> ToModels<TProjection>(List<ProjectionModelPair<TProjection, TModel>> pairs)
            where TProjection : class, IProjectionBase
        {
            foreach (var pair in pairs)
            {
                pair.Model = await MapAsync(pair.Projection);
            }

            return pairs;
        }

        public async Task<List<EntityModelPair<TEntity, TModel>>> ToEntities(List<EntityModelPair<TEntity, TModel>> pairs)
        {
            return await Task.FromResult(pairs);
        }

        public IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query)
        {
            return query;
        }

        public async Task<TModel> MapAsync(IProjectionBase projection)
        {
            return await Task.FromResult((TModel)projection);
        }

        public async Task<TModel> MapAsync(TEntity entity)
        {
            var projection = Project().Compile()(entity);
            var model = await MapAsync(projection);
            return model;
        }

        public abstract Expression<Func<TEntity, IProjectionBase>> Project();
    }
}