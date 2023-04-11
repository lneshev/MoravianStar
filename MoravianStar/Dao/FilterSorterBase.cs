using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MoravianStar.Dao
{
    /// <summary>
    /// An abstract base class for defining filtering and sorting rules for a given entity.
    /// </summary>
    /// <remarks>In the derived class you may add public properties related to filtering of the entity. Properties for sorting are not supported!</remarks>
    /// <typeparam name="TEntity">The type of the entity for which filtering and sorting rules are defined.</typeparam>
    public abstract class FilterSorterBase<TEntity>
        where TEntity : class, IEntityBase
    {
        /// <summary>
        /// Defines how the filtering for <typeparamref name="TEntity"/> should happen.
        /// </summary>
        /// <remarks>
        /// In general, all filtering rules should be applied with "AND" operator. But there are some cases, where "OR" operator should be used.
        /// In such cases the <see cref="LinqKit.PredicateBuilder"/> can be used to easily concatenate lambda expressions with "OR" operator.
        /// <para>
        /// One typical case is when there is a dropdown list in the UI which lists only some items of an entity, for example - only the active entities.
        /// A user has selected such active entity and saved the state. Later this entity becomes inactive and in the dropdown there will be no selected value, despite that in
        /// background (in the DB) everything will be fine. To fix this issue, a filter of kind: "Additional ID" may be introduced, that will can be attached to the main criteria with
        /// "OR" operator. This way in the dropdown list will be shown all active entities and additionally the currently selected item (entity).
        /// </para>
        /// <para>
        /// The previous case can be extended with adding a security layer (filtering by tenancy) over both "Main" criteria and "Additional ID" criteria. In this case, the "Security" criteria
        /// should be applied on top of both criterias, otherwise the security will be compromised.
        /// </para>
        /// <example>
        /// Example for concatenating criterias in the most advanced scenario:
        /// <code>
        /// >    "Security" critiria
        /// >    AND
        /// >    (
        /// >        "Main" criteria
        /// >        OR
        /// >        "Additional ID" criteria
        /// >    )
        /// </code>
        /// </example>
        /// </remarks>
        /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
        /// <param name="query">The source query.</param>
        /// <param name="dbContext">The DbContext that can be used for creating subqueries.</param>
        /// <returns>The query modified with the applied filtering.</returns>
        public virtual IQueryable<TEntity> Filter<TDbContext>(IQueryable<TEntity> query, IEntityRepository<TEntity, TDbContext> entityRepository)
            where TDbContext : DbContext
        {
            return query;
        }

        /// <summary>
        /// Defines how the sorting for <typeparamref name="TEntity"/> should happen.
        /// </summary>
        /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
        /// <param name="sorts">The collection of sorts that will be used for defining the sorting rules.</param>
        /// <param name="dbContext">The DbContext that can be used for creating subqueries.</param>
        /// <returns>A collection of tuples, where the first tuple's item is a sorting expression and the second tuple's item is a sorting direction.</returns>
        public virtual List<(Expression<Func<TEntity, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<TEntity, TDbContext> entityRepository)
            where TDbContext : DbContext
        {
            return new List<(Expression<Func<TEntity, object>>, SortDirection)>();
        }
    }
}