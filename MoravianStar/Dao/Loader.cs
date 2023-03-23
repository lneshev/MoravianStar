using MoravianStar.Dao.NHibernate;
using MoravianStar.DependencyInjection;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoravianStar.Dao
{
    public class Loader<TEntity, TFilter> : ILoader<TEntity, TFilter>
        where TEntity : class, IIdentifier
        where TFilter : EntityFilter
    {
        public Loader(TFilter filter)
        {
            Criteria = NHibernateSession.CurrentSession.CreateCriteria<TEntity>();

            if (filter != null)
            {
                FillCriteria(filter);

                DependencyInjectionContext.Container.TryResolveAll<ICriteriaFilled<TEntity, TFilter>>().ForEach((ICriteriaFilled<TEntity, TFilter> x) => x.Filled(Criteria, filter));
            }
        }

        public IList<TEntity> List(IList<KendoSort> sort = null, int? startIndex = null, int? maxCount = null)
        {
            ICriteria criteria = Criteria;

            AddStartIndex(criteria, startIndex);
            AddMaxCount(criteria, maxCount);
            AddOrder(criteria, sort);

            return criteria.List<TEntity>();
        }

        public int Count()
        {
            ICriteria criteria = Criteria;
            criteria.SetProjection(Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        #region Protected members
        protected ICriteria Criteria { get; set; }
        #endregion

        #region Private members
        private void FillCriteria(TFilter filter)
        {
            var queryOver = Criteria.CreateQueryOver<TEntity>();

            if (filter.Ids != null && filter.Ids.Count > 0)
            {
                queryOver.WhereRestrictionOn(x => x.Id).IsIn(filter.Ids);
            }

            if (filter.ExcludeIds != null && filter.ExcludeIds.Count > 0)
            {
                queryOver.WhereRestrictionOn(x => x.Id).Not.IsIn(filter.ExcludeIds);
            }
        }

        private void AddStartIndex(ICriteria criteria, int? startIndex)
        {
            if (startIndex.HasValue)
            {
                criteria.SetFirstResult(startIndex.Value);
            }
        }
        private void AddMaxCount(ICriteria criteria, int? maxCount)
        {
            if (maxCount.HasValue)
            {
                criteria.SetMaxResults(maxCount.Value);
            }
        }
        private void AddOrder(ICriteria criteria, IList<KendoSort> sort)
        {
            if (sort != null && sort.Count > 0)
            {
                foreach (var sortItem in sort)
                {
                    if (string.IsNullOrEmpty(sortItem.Field))
                    {
                        throw new Exception("A sorting field is missing.");
                    }

                    var nhOrder = new Order(sortItem.Field, sortItem.Dir == KendoSortDirection.Asc);
                    criteria.AddOrder(nhOrder);
                }
            }
        }
        #endregion
    }

    public class Loader<TEntity, TModel, TFilter> : Loader<TEntity, TFilter>, ILoader<TModel, TFilter>
        where TEntity : class, IIdentifier
        where TModel : class, IIdentifier, new()
        where TFilter : EntityFilter
    {
        public Loader(TFilter filter) : base(filter) { }

        public new IList<TModel> List(IList<KendoSort> sort = null, int? startIndex = null, int? maxCount = null)
        {
            var entities = base.List(sort, startIndex, maxCount).ToList();
            var contracts = new List<TModel>(entities.Count);
            var dataPairs = new List<DataPair<TEntity, TModel>>(entities.Count);

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                contracts.Add(new TModel());
                var model = contracts[i];
                dataPairs.Add(new DataPair<TEntity, TModel>()
                {
                    Entity = entity,
                    Model = model
                });

                DependencyInjectionContext.Container.TryResolveAll<IModelFilling<TEntity, TModel>>().ForEach((IModelFilling<TEntity, TModel> x) => x.Filling(entity, model));

                FillModel(entity, model);

                DependencyInjectionContext.Container.TryResolveAll<IModelFilled<TEntity, TModel>>().ForEach((IModelFilled<TEntity, TModel> x) => x.Filled(entity, model));
            }

            DependencyInjectionContext.Container.TryResolveAll<IModelListLoaded<TEntity, TModel>>().ForEach((IModelListLoaded<TEntity, TModel> x) => x.ListLoaded(dataPairs));

            return contracts;
        }

        #region Private members
        private void FillModel(TEntity entity, TModel model)
        {
            model.Id = entity.Id;
        }
        #endregion
    }
}