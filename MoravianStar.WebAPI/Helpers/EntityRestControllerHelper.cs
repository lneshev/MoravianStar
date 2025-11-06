using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using MoravianStar.Exceptions;
using MoravianStar.Resources;
using MoravianStar.Utilities;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace MoravianStar.WebAPI.Helpers
{
    /// <summary>
    /// The base WebAPI controller helper class for the most common operations over an entity (like CRUD, count, exist, etc.).
    /// This class is intended to be used by WebAPI controllers. It allows the controller which uses it to return the data from each method in a custom way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the Id of the entity.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    public class EntityRestControllerHelper<TEntity, TId, TModel, TFilter, TDbContext>
        where TEntity : class, IEntityBase<TId>, IProjectionBase, new()
        where TModel : class, IModelBase<TId>, IProjectionBase, new()
        where TFilter : FilterSorterBase<TEntity>, new()
        where TDbContext : DbContext
    {
        protected readonly IModelRepository<TModel, TEntity, TId, TDbContext> modelRepository;

        public EntityRestControllerHelper()
        {
            modelRepository = Persistence.ForDbContext<TDbContext>().ForModel<TModel, TEntity, TId>();
        }

        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <returns>The found <typeparamref name="TEntity"/> by Id, transformed into <typeparamref name="TModel"/></returns>
        public virtual async Task<TModel> GetAsync(TId id)
        {
            return await modelRepository.GetAsync(id, false);
        }

        /// <summary>
        /// Get entities, optionally by a filter, sortings and paging, and transforms each entity to a given model.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <param name="sorts">The collection of sorts used for sorting.</param>
        /// <param name="page">The page object used for paging.</param>
        /// <returns>The found entities, transformed into <see cref="IEnumerable{TModel}"/>, and their total count (excluding the paging), wrapped in <see cref="PageResult{TModel}"/> object.</returns>
        /// <exception cref="SecurityException"></exception>
        public virtual async Task<PageResult<TModel>> ReadAsync(TFilter filter, List<Sort> sorts, Page page)
        {
            if (filter is ISecurityFilter securityFilter && !securityFilter.IsSecurityEnabled)
            {
                throw new SecurityException(Strings.DisablingTheSecurityIsNotAllowed);
            }

            return await modelRepository.ReadAsync(filter, sorts, page, false, true);
        }

        /// <summary>
        /// Counts entities, optionally by a filter.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns>The number of the found entities.</returns>
        public virtual async Task<int> CountAsync(TFilter filter)
        {
            return await modelRepository.CountAsync(filter);
        }

        /// <summary>
        /// Checks if entities exist, optionally by a filter.
        /// </summary>
        /// <param name="filter">The <see cref="FilterSorterBase{TEntity}"/> instance used for filtering.</param>
        /// <returns><see langword="True"/> if the entities exist, otherwise <see langword="false"/>.</returns>
        public virtual async Task<bool> ExistAsync(TFilter filter)
        {
            return await modelRepository.ExistAsync(filter);
        }

        /// <summary>
        /// Creates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="model">The model containing the input data of the entity, that will be created.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<TModel> CreateAsync(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return await modelRepository.CreateAsync(model);
        }

        /// <summary>
        /// Updates and saves an entity, based on a <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="id">The Id of the entity that will be updated.</param>
        /// <param name="model">The model containing the input data of the entity, that will be updated.</param>
        /// <returns>The model containing the input data. The model might be modified by the logic.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidModelStateException"></exception>
        public virtual async Task<TModel> UpdateAsync(TId id, TModel model)
        {
            if (IdValidator.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (IdValidator.IsNullOrEmpty(model.Id))
            {
                throw new ArgumentNullException(nameof(model.Id));
            }
            if (!id.Equals(model.Id))
            {
                throw new InvalidModelStateException(Strings.ParametersIdAndModelIdShouldBeSame);
            }

            return await modelRepository.UpdateAsync(model);
        }

        /// <summary>
        /// Deletes an entity, based on an Id.
        /// </summary>
        /// <param name="id">The target Id.</param>
        /// <returns>A model of the found entity in a state before the deletion.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<TModel> DeleteAsync(TId id)
        {
            if (IdValidator.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await modelRepository.DeleteAsync(id);
        }
    }
}