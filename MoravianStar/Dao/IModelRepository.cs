using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    /// <summary>
    /// Provides methods for CRUD operations over an entity and transforming the results into a model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    public interface IModelRepository<TModel, TEntity, TDbContext>
        where TModel : class, IModelBase
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        /// <summary>
        /// The EntityRepository instance that will be called internally
        /// </summary>
        public IEntityRepository<TEntity, TDbContext> EntityRepository { get; }

        /// <summary>
        /// Asynchronously gets entities, optionally by a filter, sortings and paging, and transforms each entity to a given model.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter being used for querying.</typeparam>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <param name="getTotalCount">Specifies whether the total count of the entities should be calculated and returned or not.</param>
        /// <returns>The found entities, transformed into <see cref="IEnumerable{TModel}"/></returns>
        Task<PageResult<TModel>> ReadAsync<TFilter>(TFilter filter = null, IEnumerable<Sort> sorts = null, Page page = null, bool trackable = false, bool getTotalCount = false)
            where TFilter : FilterSorterBase<TEntity>, new();

        /// <summary>
        /// Asynchronously counts entities, optionally by a filter.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter being used for counting.</typeparam>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns>The number of the found entities.</returns>
        Task<int> CountAsync<TFilter>(TFilter filter = null)
            where TFilter : FilterSorterBase<TEntity>, new();

        /// <summary>
        /// Asynchronously checks if entities exist, optionally by a filter.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter being used for checking.</typeparam>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns><see langword="True"/> if the entities exist, otherwise <see langword="false"/>.</returns>
        Task<bool> ExistAsync<TFilter>(TFilter filter = null)
           where TFilter : FilterSorterBase<TEntity>, new();

        /// <summary>
        /// Asynchronously creates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="model">The model containing the input data of the entity, that will be created.</param>
        /// <param name="additionalParameters">Additional parameters that can be passed to the event handlers, like <see cref="IEntitySaving{TEntity}"/> and so on.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        Task<TModel> CreateAsync(TModel model, IDictionary<string, object> additionalParameters = null);
    }

    /// <summary>
    /// Provides methods for CRUD operations over an entity and transforming the results into a model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the Id of the entity.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    public interface IModelRepository<TModel, TEntity, TId, TDbContext> : IModelRepository<TModel, TEntity, TDbContext>
        where TModel : class, IModelBase
        where TEntity : class, IEntityBase<TId>
        where TDbContext : DbContext
    {
        public new IEntityRepository<TEntity, TId, TDbContext> EntityRepository { get; }

        /// <summary>
        /// Asynchronously gets an entity by Id and transforms it to a given model.
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>The found <typeparamref name="TEntity"/> by Id, transformed into <typeparamref name="TModel"/></returns>
        Task<TModel> GetAsync(TId id, bool trackable = false);

        /// <summary>
        /// Asynchronously updates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="model">The model containing the input data of the entity, that will be updated.</param>
        /// <param name="additionalParameters">Additional parameters that can be passed to the event handlers, like <see cref="IEntitySaving{TEntity}"/> and so on.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        Task<TModel> UpdateAsync(TModel model, IDictionary<string, object> additionalParameters = null);

        /// <summary>
        /// Asynchronously deletes an entity, based on an Id.
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <param name="additionalParameters">Additional parameters that can be passed to the event handlers, like <see cref="IEntityDeleting{TEntity}"/> and so on.</param>
        /// <returns>A model of the found entity in a state before the deletion.</returns>
        Task<TModel> DeleteAsync(TId id, IDictionary<string, object> additionalParameters = null);
    }
}