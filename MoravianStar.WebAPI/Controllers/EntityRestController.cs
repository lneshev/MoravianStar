using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using MoravianStar.Exceptions;
using MoravianStar.WebAPI.Attributes;
using MoravianStar.WebAPI.Constants;
using MoravianStar.WebAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Controllers
{
    /// <summary>
    /// The base WebAPI controller for the most common operations over an entity (like CRUD, count, exist, etc.), defined in the REST standard.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the Id of the entity.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    [ApiController]
    [Route(RoutingConstants.ApiController)]
    public abstract class EntityRestController<TEntity, TId, TModel, TFilter, TDbContext> : ControllerBase
        where TEntity : class, IEntityBase<TId>, IProjectionBase, new()
        where TModel : class, IModelBase<TId>, IProjectionBase, new()
        where TFilter : FilterSorterBase<TEntity>, new()
        where TDbContext : DbContext
    {
        protected readonly EntityRestControllerHelper<TEntity, TId, TModel, TFilter, TDbContext> helper;

        public EntityRestController()
        {
            helper = new EntityRestControllerHelper<TEntity, TId, TModel, TFilter, TDbContext>();
        }

        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <returns>The found <typeparamref name="TEntity"/> by Id, transformed into <typeparamref name="TModel"/></returns>
        [NonInvokable]
        [HttpGet(RoutingConstants.Id)]
        public virtual async Task<ActionResult<TModel>> Get([FromRoute] TId id)
        {
            TModel result = await helper.GetAsync(id);
            return result;
        }

        /// <summary>
        /// Get entities, optionally by a filter, sortings and paging, and transforms each entity to a given model.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <returns>The found entities, transformed into <see cref="IEnumerable{TModel}"/>, and their total count (excluding the paging), wrapped in <see cref="PageResult{TModel}"/> object.</returns>
        /// <exception cref="SecurityException"></exception>
        [NonInvokable]
        [HttpGet]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<PageResult<TModel>>> Read([FromQuery] TFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            PageResult<TModel> result = await helper.ReadAsync(filter, sorts, page);
            return result;
        }

        /// <summary>
        /// Counts entities, optionally by a filter.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns>The number of the found entities.</returns>
        [NonInvokable]
        [HttpGet(RoutingConstants.Action)]
        public virtual async Task<ActionResult<int>> Count([FromQuery] TFilter filter)
        {
            int result = await helper.CountAsync(filter);
            return result;
        }

        /// <summary>
        /// Checks if entities exist, optionally by a filter.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns><see langword="True"/> if the entities exist, otherwise <see langword="false"/>.</returns>
        [NonInvokable]
        [HttpGet(RoutingConstants.Action)]
        public virtual async Task<ActionResult<bool>> Exist([FromQuery] TFilter filter)
        {
            bool result = await helper.ExistAsync(filter);
            return result;
        }

        /// <summary>
        /// Creates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="model">The model containing the input data of the entity, that will be created.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [NonInvokable]
        [HttpPost]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<TModel>> Post([FromBody] TModel model)
        {
            TModel result = await helper.CreateAsync(model);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="id">The Id of the entity that will be updated.</param>
        /// <param name="model">The model containing the input data of the entity, that will be updated.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidModelStateException"></exception>
        [NonInvokable]
        [HttpPut(RoutingConstants.Id)]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<TModel>> Put([FromRoute] TId id, [FromBody] TModel model)
        {
            TModel result = await helper.UpdateAsync(id, model);
            return result;
        }

        /// <summary>
        /// Deletes an entity, based on an Id.
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <returns>A model of the found entity in a state before the deletion.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [NonInvokable]
        [HttpDelete(RoutingConstants.Id)]
        [ExecuteInTransactionAsync]
        public virtual async Task<ActionResult<TModel>> Delete([FromRoute] TId id)
        {
            TModel result = await helper.DeleteAsync(id);
            return result;
        }
    }
}