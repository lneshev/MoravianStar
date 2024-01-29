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
        public virtual async Task<List<ProjectionModelPair<IProjectionBase, TModel>>> ToModels(List<ProjectionModelPair<IProjectionBase, TModel>> pairs)
        {
            foreach (var pair in pairs)
            {
                pair.Model = pair.Projection is TEntity ? await MapAsync(pair.Projection as TEntity) : await MapAsync(pair.Projection);
            }

            return pairs;
        }

        public virtual async Task<List<EntityModelPair<TEntity, TModel>>> ToEntities(List<EntityModelPair<TEntity, TModel>> pairs)
        {
            return await Task.FromResult(pairs);
        }

        public virtual IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query)
        {
            return query;
        }

        public virtual async Task<TModel> MapAsync(IProjectionBase projection)
        {
            return await Task.FromResult((TModel)projection);
        }

        public virtual async Task<TModel> MapAsync(TEntity entity)
        {
            var projection = Project().Compile()(entity);
            var model = await MapAsync(projection);
            return model;
        }

        public abstract Expression<Func<TEntity, IProjectionBase>> Project();
    }
}