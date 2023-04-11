using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoravianStar.Dao
{
    /// <summary>
    /// Provides access to the data layer via the repository pattern.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    public interface IEntityRepository<TEntity, TDbContext>
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        /// <summary>
        /// Creates and returns a query that can get entities, optionally by a filter, sortings, paging and includes (aka SQL joins).
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter being used for querying.</typeparam>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <param name="includes">Allows modifying the query to add the necessary includes (aka the SQL joins). See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager"/>.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that allows querying entities.</returns>
        IQueryable<TEntity> ReadQuery<TFilter>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
            bool trackable = true)
            where TFilter : FilterSorterBase<TEntity>, new();

        /// <summary>
        /// Creates and returns a query that can get entities, optionally by a filter, sortings and paging, via a projection.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter being used for querying.</typeparam>
        /// <typeparam name="TProjection">The type of the resulting projection.</typeparam>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <param name="projection">Allows setting the resulting projection.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>An <see cref="IQueryable{TProjection}"/> that allows querying entities.</returns>
        IQueryable<TProjection> ReadQuery<TFilter, TProjection>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Expression<Func<TEntity, TProjection>> projection = null,
            bool trackable = true)
            where TFilter : FilterSorterBase<TEntity>, new()
            where TProjection : class, IProjectionBase;

        /// <summary>
        /// Creates and asynchronously executes a query that can get entities, optionally by a filter, sortings, paging and includes (aka SQL joins).
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter being used for querying.</typeparam>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <param name="includes">Allows modifying the query to add the necessary includes (aka the SQL joins). See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager"/>.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>An <see cref="IEnumerable{TEntity}"/> of the found entities.</returns>
        Task<PageResult<TEntity>> ReadAsync<TFilter>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
            bool trackable = true,
            bool getTotalCount = false)
            where TFilter : FilterSorterBase<TEntity>, new();

        /// <summary>
        /// Creates and asynchronously executes a query that can get entities, optionally by a filter, sortings and paging, via a projection.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter being used for querying.</typeparam>
        /// <typeparam name="TProjection">The type of the resulting projection.</typeparam>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <param name="projection">Allows setting the resulting projection.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>An <see cref="IEnumerable{TProjection}"/> of the found entities.</returns>
        Task<PageResult<TProjection>> ReadAsync<TFilter, TProjection>(
            TFilter filter = null,
            IEnumerable<Sort> sorts = null,
            Page page = null,
            Expression<Func<TEntity, TProjection>> projection = null,
            bool trackable = true,
            bool getTotalCount = false)
            where TFilter : FilterSorterBase<TEntity>, new()
            where TProjection : class, IProjectionBase;

        /// <summary>
        /// Asynchronously counts entities, optionally by a filter.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity being count.</typeparam>
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

        Task SaveAsync(TEntity entity, IDictionary<string, object> additionalParameters = null);

        /// <summary>
        /// Begins tracking the given entity such that it will be removed from the database when <see cref="SaveChangesAsync"/> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity being deleted.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        Task<TEntity> DeleteAsync(TEntity entity, IDictionary<string, object> additionalParameters = null);
    }

    /// <inheritdoc cref="IEntityRepository"/>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    public interface IEntityRepository<TEntity, TId, TDbContext> : IEntityRepository<TEntity, TDbContext>
        where TEntity: class, IEntityBase<TId>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Creates and returns a query that can get entity by Id.
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <param name="includes">Allows modifying the query to add the necessary includes (aka the SQL joins). See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager"/>.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>An <see cref="IQueryable{TEntity}"/> that allows getting entity by Id.</returns>
        IQueryable<TEntity> GetQuery(
            TId id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
            bool trackable = true);

        /// <summary>
        /// Creates and returns a query that can get entity by Id via a projection.
        /// </summary>
        /// <typeparam name="TProjection">The type of the resulting projection.</typeparam>
        /// <param name="id">The target Id.</param>
        /// <param name="projection">Allows setting the resulting projection.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>An <see cref="IQueryable{TProjection}"/> that allows getting entity by Id.</returns>
        IQueryable<TProjection> GetQuery<TProjection>(
            TId id,
            Expression<Func<TEntity, TProjection>> projection = null,
            bool trackable = true)
            where TProjection : class, IProjectionBase;

        /// <summary>
        /// Creates and asynchronously executes a query that can get entity by Id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
        /// <typeparam name="TId">The type of the Id of the entity being queried.</typeparam>
        /// <param name="id">The target Id.</param>
        /// <param name="includes">Allows modifying the query to add the necessary includes (aka the SQL joins). See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager"/>.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>The found <typeparamref name="TEntity"/> by Id.</returns>
        Task<TEntity> GetAsync(
            TId id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> includes = null,
            bool trackable = true);

        /// <summary>
        /// Creates and asynchronously executes a query that can get entity by Id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
        /// <typeparam name="TId">The type of the Id of the entity being queried.</typeparam>
        /// <typeparam name="TProjection">The type of the resulting projection.</typeparam>
        /// <param name="id">The target Id.</param>
        /// <param name="projection">Allows setting the resulting projection.</param>
        /// <param name="trackable">Specifies whether the query should be trackable or not. See: <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/>.</param>
        /// <returns>The <typeparamref name="TProjection"/> of the found <typeparamref name="TEntity"/> by Id.</returns>
        Task<TProjection> GetAsync<TProjection>(
            TId id,
            Expression<Func<TEntity, TProjection>> projection = null,
            bool trackable = true)
            where TProjection : class, IProjectionBase;

        /// <summary>
        /// Finds an entity of type <typeparamref name="TEntity"/> by Id and begins tracking it such that it will be removed from the database when <see cref="SaveChangesAsync"/> is called.
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(TId id, IDictionary<string, object> additionalParameters = null);
    }
}