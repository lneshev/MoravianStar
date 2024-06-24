using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoravianStar.DependencyInjection;
using MoravianStar.Exceptions;
using MoravianStar.Extensions;
using MoravianStar.Resources;
using MoravianStar.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    /// <inheritdoc cref="IEntityRepository"/>
    public class EntityRepository<TEntity, TDbContext> : IEntityRepository<TEntity, TDbContext>
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        protected readonly IDbTransaction<TDbContext> dbTransaction;

        public EntityRepository(IDbTransaction<TDbContext> dbTransaction)
        {
            this.dbTransaction = dbTransaction;
        }

        public IQueryable<TEntity> ReadQuery<TFilter>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
            bool trackable = true)
            where TFilter : FilterSorterBase<TEntity>, new()
        {
            var query = dbTransaction.DbContext.Set<TEntity>();
            return query.Include(includes).Filter(filter, this).Sort<TEntity, TFilter, TDbContext>(sorts, this).Page(page).Track(trackable);
        }

        public IQueryable<TProjection> ReadQuery<TFilter, TProjection>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Expression<Func<TEntity, TProjection>> projection = null,
            bool trackable = true)
            where TFilter : FilterSorterBase<TEntity>, new()
            where TProjection : class, IProjectionBase
        {
            return ReadQuery(filter, sorts, page, null, trackable).Projection(projection);
        }

        public async Task<PageResult<TEntity>> ReadAsync<TFilter>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
            bool trackable = true,
            bool getTotalCount = false)
            where TFilter : FilterSorterBase<TEntity>, new()
        {
            return new PageResult<TEntity>()
            {
                Items = await ReadQuery(filter, sorts, page, includes, trackable).ToListAsync(),
                TotalCount = getTotalCount ? await CountAsync(filter) : null,
                TotalCountGet = getTotalCount
            };
        }

        public async Task<PageResult<TProjection>> ReadAsync<TFilter, TProjection>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Expression<Func<TEntity, TProjection>> projection = null,
            bool trackable = true,
            bool getTotalCount = false)
            where TFilter : FilterSorterBase<TEntity>, new()
            where TProjection : class, IProjectionBase
        {
            return new PageResult<TProjection>()
            {
                Items = await ReadQuery(filter, sorts, page, projection, trackable).ToListAsync(),
                TotalCount = getTotalCount ? await CountAsync(filter) : null,
                TotalCountGet = getTotalCount
            };
        }

        public async Task<int> CountAsync<TFilter>(TFilter filter = null)
            where TFilter : FilterSorterBase<TEntity>, new()
        {
            return await dbTransaction.DbContext.Set<TEntity>().Filter(filter, this).CountAsync();
        }

        public async Task<bool> ExistAsync<TFilter>(TFilter filter = null)
            where TFilter : FilterSorterBase<TEntity>, new()
        {
            return await dbTransaction.DbContext.Set<TEntity>().Filter(filter, this).AnyAsync();
        }

        public async Task SaveAsync(TEntity entity, IDictionary<string, object> additionalParameters = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (additionalParameters == null)
            {
                additionalParameters = new Dictionary<string, object>();
            }

            TEntity originalEntity = await GetOriginalEntityAsync(entity, additionalParameters);

            await ValidateAsync(entity, originalEntity, additionalParameters);

            await ExecuteHandlersAsync<IEntitySaving<TEntity>>(x => x.SavingAsync(entity, originalEntity, additionalParameters));

            bool entityIsNew = entity.IsNew();
            if (entityIsNew)
            {
                entity = (await dbTransaction.DbContext.AddAsync(entity)).Entity;
            }
            else
            {
                dbTransaction.DbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbTransaction.DbContext.SaveChangesAsync();

            await ExecuteHandlersAsync<IEntitySaved<TEntity>>(x => x.SavedAsync(entity, originalEntity, entityIsNew, additionalParameters));
        }

        public async Task DeleteAsync(TEntity entity, IDictionary<string, object> additionalParameters = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (additionalParameters == null)
            {
                additionalParameters = new Dictionary<string, object>();
            }

            await ExecuteHandlersAsync<IEntityDeleting<TEntity>>(x => x.DeletingAsync(entity, additionalParameters));

            entity = dbTransaction.DbContext.Remove(entity).Entity;
            await dbTransaction.DbContext.SaveChangesAsync();

            await ExecuteHandlersAsync<IEntityDeleted<TEntity>>(x => x.DeletedAsync(entity, additionalParameters));
        }

        private async Task<TEntity> GetOriginalEntityAsync(TEntity entity, IDictionary<string, object> additionalParameters)
        {
            TEntity defaultRetreiver(TEntity entity)
            {
                return (TEntity)dbTransaction.DbContext.Entry(entity).OriginalValues.ToObject();
            }

            var service = ServiceLocator.Container.GetService<IGetOriginalEntity<TEntity>>();
            if (service != null)
            {
                return await service.GetAsync(entity, defaultRetreiver, additionalParameters);
            }
            else
            {
                return defaultRetreiver(entity);
            }
        }

        private async Task ValidateAsync(TEntity entity, TEntity originalEntity, IDictionary<string, object> additionalParameters)
        {
            await ExecuteHandlersAsync<IEntityValidating<TEntity>>(x => x.ValidatingAsync(entity, originalEntity, additionalParameters));

            var validationContext = new ValidationContext(entity);
            Validator.ValidateObject(entity, validationContext, true);

            await ExecuteHandlersAsync<IEntityValidated<TEntity>>(x => x.ValidatedAsync(entity, originalEntity, additionalParameters));
        }

        private async Task ExecuteHandlersAsync<THandler>(Func<THandler, Task> handlerFunc)
        {
            var handlers = ServiceLocator.Container.GetServices<THandler>().ToList();
            foreach (var handler in handlers)
            {
                await handlerFunc(handler);
            }
        }
    }

    /// <inheritdoc cref="IEntityRepository"/>
    public class EntityRepository<TEntity, TId, TDbContext> : EntityRepository<TEntity, TDbContext>, IEntityRepository<TEntity, TId, TDbContext>
        where TEntity : class, IEntityBase<TId>
        where TDbContext : DbContext
    {
        public EntityRepository(IDbTransaction<TDbContext> dbTransaction) : base(dbTransaction)
        {
        }

        public IQueryable<TEntity> GetQuery(TId id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null, bool trackable = true)
        {
            if (IdValidator.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var query = dbTransaction.DbContext.Set<TEntity>();
            return query.Include(includes).Where(x => x.Id.Equals(id)).Track(trackable);
        }

        public IQueryable<TProjection> GetQuery<TProjection>(TId id, Expression<Func<TEntity, TProjection>> projection = null, bool trackable = true)
            where TProjection : class, IProjectionBase
        {
            return GetQuery(id, null, trackable).Projection(projection);
        }

        public async Task<TEntity> GetAsync(TId id, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null, bool trackable = true)
        {
            var query = GetQuery(id, includes, trackable);

            var entity = await query.SingleOrDefaultAsync();

            if (entity == null)
            {
                throw new EntityNotFoundException(string.Format(Strings.AnEntityOfTypeWithIdWasNotFound, typeof(TEntity).Name, id));
            }

            return entity;
        }

        public async Task<TProjection> GetAsync<TProjection>(TId id, Expression<Func<TEntity, TProjection>> projection = null, bool trackable = true)
            where TProjection : class, IProjectionBase
        {
            var query = GetQuery(id, projection, trackable);

            var proj = await query.SingleOrDefaultAsync();

            if (proj == null)
            {
                throw new EntityNotFoundException(string.Format(Strings.AnEntityOfTypeWithIdWasNotFound, typeof(TEntity).Name, id));
            }

            return proj;
        }

        public async Task DeleteAsync(TId id, IDictionary<string, object> additionalParameters = null)
        {
            if (additionalParameters == null)
            {
                additionalParameters = new Dictionary<string, object>();
            }

            var entity = await dbTransaction.DbContext.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity, additionalParameters);
            }
        }
    }
}