using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    /// <inheritdoc cref="IModelRepository{TModel, TEntity, TDbContext}"/>
    public class ModelRepository<TModel, TEntity, TDbContext> : IModelRepository<TModel, TEntity, TDbContext>
        where TModel : class, IModelBase, new()
        where TEntity : class, IEntityBase, IProjectionBase, new()
        where TDbContext : DbContext
    {
        public IEntityRepository<TEntity, TDbContext> EntityRepository { get; }
        protected readonly IModelsMappingService<TModel, TEntity> modelsMappingService;

        public ModelRepository(IEntityRepository<TEntity, TDbContext> entityRepository, IModelsMappingService<TModel, TEntity> modelsMappingService)
        {
            EntityRepository = entityRepository;
            this.modelsMappingService = modelsMappingService;
        }

        public async Task<PageResult<TModel>> ReadAsync<TFilter>(TFilter filter = null, IEnumerable<Sort> sorts = null, Page page = null, bool trackable = false, bool getTotalCount = false)
            where TFilter : FilterSorterBase<TEntity>, new()
        {
            var entitiesPageResult = await EntityRepository.ReadAsync(filter, sorts, page, modelsMappingService.Project(), trackable, getTotalCount);
            var pairs = entitiesPageResult.Items.Select(projection => new ProjectionModelPair<IProjectionBase, TModel>() { Projection = projection, Model = new TModel() }).ToList();

            var models = (await modelsMappingService.ToModels(pairs)).Select(x => x.Model);

            return new PageResult<TModel>()
            {
                Items = models,
                TotalCount = entitiesPageResult.TotalCount,
                TotalCountGet = entitiesPageResult.TotalCountGet
            };
        }

        public async Task<int> CountAsync<TFilter>(TFilter filter = null)
            where TFilter : FilterSorterBase<TEntity>, new()
        {
            return await EntityRepository.CountAsync(filter);
        }

        public async Task<bool> ExistAsync<TFilter>(TFilter filter = null)
            where TFilter : FilterSorterBase<TEntity>, new()
        {
            return await EntityRepository.ExistAsync(filter);
        }

        public async Task<TModel> CreateAsync(TModel model, IDictionary<string, object> additionalParameters = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var entity = new TEntity();

            entity = (await modelsMappingService.ToEntities(new List<EntityModelPair<TEntity, TModel>>()
            {
                new EntityModelPair<TEntity, TModel>()
                {
                    Entity = entity,
                    Model = model
                }
            })).Single().Entity;

            await EntityRepository.SaveAsync(entity, additionalParameters);

            model = (await modelsMappingService.ToModels(new List<ProjectionModelPair<IProjectionBase, TModel>>()
            {
                new ProjectionModelPair<IProjectionBase, TModel>()
                {
                    Projection = entity,
                    Model = new TModel(),
                }
            })).Single().Model;

            return model;
        }
    }

    /// <inheritdoc cref="IModelRepository{TModel, TEntity, TId, TDbContext}"/>
    public class ModelRepository<TModel, TEntity, TId, TDbContext> : ModelRepository<TModel, TEntity, TDbContext>, IModelRepository<TModel, TEntity, TId, TDbContext>
        where TModel : class, IModelBase<TId>, new()
        where TEntity : class, IEntityBase<TId>, IProjectionBase, new()
        where TDbContext : DbContext
    {
        public new IEntityRepository<TEntity, TId, TDbContext> EntityRepository { get; }

        public ModelRepository(IEntityRepository<TEntity, TId, TDbContext> entityRepository, IModelsMappingService<TModel, TEntity> modelsMappingService) : base(entityRepository, modelsMappingService)
        {
            EntityRepository = entityRepository;
        }

        public async Task<TModel> GetAsync(TId id, bool trackable = false)
        {
            var projection = await EntityRepository.GetAsync(id, modelsMappingService.Project(), trackable);

            var model = (await modelsMappingService.ToModels(new List<ProjectionModelPair<IProjectionBase, TModel>>()
            {
                new ProjectionModelPair<IProjectionBase, TModel>()
                {
                    Projection = projection,
                    Model = new TModel()
                }
            })).Single().Model;

            return model;
        }

        public async Task<bool> ExistsAsync(TId id)
        {
            return await EntityRepository.ExistsAsync(id);
        }

        public async Task<TModel> UpdateAsync(TModel model, IDictionary<string, object> additionalParameters = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.IsNew())
            {
                throw new ArgumentNullException(nameof(model.Id));
            }

            var entity = await EntityRepository.GetAsync(model.Id, modelsMappingService.GetIncludes, true);

            entity = (await modelsMappingService.ToEntities(new List<EntityModelPair<TEntity, TModel>>()
            {
                new EntityModelPair<TEntity, TModel>()
                {
                    Entity = entity,
                    Model = model
                }
            })).Single().Entity;

            await EntityRepository.SaveAsync(entity, additionalParameters);

            var newModel = (await modelsMappingService.ToModels(new List<ProjectionModelPair<IProjectionBase, TModel>>()
            {
                new ProjectionModelPair<IProjectionBase, TModel>()
                {
                    Projection = entity,
                    Model = new TModel()
                }
            })).Single().Model;

            return newModel;
        }

        public async Task<TModel> DeleteAsync(TId id, IDictionary<string, object> additionalParameters = null)
        {
            var entity = await EntityRepository.GetAsync(id, modelsMappingService.GetIncludes);

            var model = (await modelsMappingService.ToModels(new List<ProjectionModelPair<IProjectionBase, TModel>>()
            {
                new ProjectionModelPair<IProjectionBase, TModel>()
                {
                    Projection = entity,
                    Model = new TModel()
                }
            })).Single().Model;

            await EntityRepository.DeleteAsync(entity, additionalParameters);

            return model;
        }
    }
}