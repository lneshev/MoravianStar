using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using MoravianStar.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MoravianStar.Extensions
{
    /// <summary>
    /// Queryable extension methods that are related to repository operations.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Specifies related entities to include in the query results.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="query">The source query.</param>
        /// <param name="includes">Lambda expression with the includes.</param>
        /// <returns>The query modified with the related data included. If <paramref name="includes"/> is <see langword="null"/>, the source query is returned without any modification.</returns>
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes)
            where TEntity : class, IEntityBase
        {
            if (includes != null)
            {
                query = includes(query);
            }

            return query;
        }

        /// <summary>
        /// Applies a filtering, defined for a <typeparamref name="TEntity"/>, over a <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <typeparam name="TFilter">The type of filter being used for filtering.</typeparam>
        /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
        /// <param name="query">The source query.</param>
        /// <param name="filter">The filter that will be used for filtering.</param>
        /// <param name="entityRepository">The <see cref="IEntityRepository{TEntity, TDbContext}"/> that can be used for creating subqueries in the filter.</param>
        /// <returns>The query modified with the applied filtering.</returns>
        public static IQueryable<TEntity> Filter<TEntity, TFilter, TDbContext>(this IQueryable<TEntity> query, TFilter filter, IEntityRepository<TEntity, TDbContext> entityRepository)
            where TEntity : class, IEntityBase
            where TFilter : FilterSorterBase<TEntity>, new()
            where TDbContext : DbContext
        {
            return (filter ?? new()).Filter(query, entityRepository);
        }

        /// <summary>
        /// Applies a sorting, defined for a <typeparamref name="TEntity"/>, over a <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <typeparam name="TSorter">The type of sorter being used for sorting. <typeparamref name="TSorter"/> should be of type: <see cref="FilterSorterBase{TEntity}"/>.</typeparam>
        /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
        /// <param name="query">The source query.</param>
        /// <param name="sorts">The collection of sorts that will be used for sorting.</param>
        /// <param name="entityRepository">The <see cref="IEntityRepository{TEntity, TDbContext}"/> that can be used for creating subqueries in the sorter.</param>
        /// <returns>The query modified with the applied sorting.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IQueryable<TEntity> Sort<TEntity, TSorter, TDbContext>(this IQueryable<TEntity> query, IEnumerable<Sort> sorts, IEntityRepository<TEntity, TDbContext> entityRepository)
            where TEntity : class, IEntityBase
            where TSorter : FilterSorterBase<TEntity>, new()
            where TDbContext : DbContext
        {
            if (sorts == null)
            {
                sorts = new List<Sort>();
            }
            if (sorts.Any(x => string.IsNullOrWhiteSpace(x.Field)))
            {
                throw new ArgumentException(Strings.ASortingFieldIsEitherNullOrEmpty, nameof(sorts));
            }

            var entitySorts = new TSorter().Sort(sorts, entityRepository);
            query = query.Order(entitySorts);

            return query;
        }

        /// <summary>
        /// Applies a paging over a <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="query">The source query.</param>
        /// <param name="page">The page object that will be used for paging.</param>
        /// <returns>The query modified with the applied paging.</returns>
        public static IQueryable<TEntity> Page<TEntity>(this IQueryable<TEntity> query, Page page)
            where TEntity : class, IEntityBase
        {
            if (page == null)
            {
                page = new Page();
            }
            if (!page.PageNumber.HasValue)
            {
                page.PageNumber = 1;
            }
            if (!page.PageSize.HasValue)
            {
                page.PageSize = int.MaxValue;
            }

            return query.Skip((page.PageNumber.Value - 1) * page.PageSize.Value).Take(page.PageSize.Value);
        }

        /// <summary>
        /// Applies a projection over a <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <typeparam name="TProjection">The type of the resulting projection.</typeparam>
        /// <param name="query">The source query.</param>
        /// <param name="projection">The expression that will be used to construct the projection.</param>
        /// <returns>An <see cref="IQueryable{TProjection}"/> whose elements are the result of invoking the projection expression.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<TProjection> Projection<TEntity, TProjection>(this IQueryable<TEntity> query, Expression<Func<TEntity, TProjection>> projection)
            where TEntity : class, IEntityBase
            where TProjection : class, IProjectionBase
        {
            if (projection == null)
            {
                throw new ArgumentNullException(nameof(projection));
            }

            return query.Select(projection);
        }

        /// <summary>
        /// Turns on or off the tracking mechanism.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="query">The source query.</param>
        /// <param name="trackable">Indicates if the query should be trackable or not.</param>
        /// <returns>A new query where the result set will be or will not be tracked by the context.</returns>
        public static IQueryable<TEntity> Track<TEntity>(this IQueryable<TEntity> query, bool trackable)
            where TEntity : class, IEntityBase
        {
            if (!trackable)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        private static IQueryable<TEntity> Order<TEntity>(this IQueryable<TEntity> query, IEnumerable<(Expression<Func<TEntity, object>> expression, SortDirection direction)> orders)
            where TEntity : class, IEntityBase
        {
            IQueryable<TEntity> result = query;

            if (orders != null && orders.Any())
            {
                IOrderedQueryable<TEntity> orderedResult = null;
                var ordersList = orders.ToList();

                var orderBy = ordersList[0];
                switch (orderBy.direction)
                {
                    case SortDirection.Asc:
                        orderedResult = query.OrderBy(orderBy.expression);
                        break;
                    case SortDirection.Desc:
                        orderedResult = query.OrderByDescending(orderBy.expression);
                        break;
                }

                for (int i = 1; i < ordersList.Count; i++)
                {
                    orderBy = ordersList[i];
                    switch (orderBy.direction)
                    {
                        case SortDirection.Asc:
                            orderedResult = orderedResult.ThenBy(orderBy.expression);
                            break;
                        case SortDirection.Desc:
                            orderedResult = orderedResult.ThenByDescending(orderBy.expression);
                            break;
                    }
                }

                result = orderedResult;
            }

            return result;
        }
    }
}