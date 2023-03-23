using MoravianStar.Dao;
using MoravianStar.MvcFramework.Filters;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MoravianStar.MvcFramework.Controllers
{
    public abstract class EntityController<TModel, TFilter> : BaseController
        where TModel : class, IIdentifier
        where TFilter : EntityFilter
    {
        [HttpGet]
        [NHibernateActionFilter]
        public virtual JsonResult Get(int id)
        {
            TModel model = Persistence.GetPersister<TModel>(id).Load();
            return JsonNet(model, JsonRequestBehavior.AllowGet);
        }

        [NHibernateActionFilter]
        public virtual JsonResult List(TFilter filter, IList<KendoSort> sort = null, int? startIndex = null, int? maxCount = null)
        {
            IList<TModel> models = ListEntities(filter, sort, startIndex, maxCount);
            return JsonNet(models, JsonRequestBehavior.AllowGet);
        }

        [NHibernateActionFilter]
        public virtual JsonResult Read(KendoDataSourceRequest request, TFilter filter)
        {
            int? startIndex = null;
            int? maxCount = null;

            if (request != null)
            {
                startIndex = (request.Page - 1) * request.PageSize;
                maxCount = request.PageSize != 0 ? request.PageSize : (int?)null;
            }

            IList<TModel> models = ListEntities(filter, request.Sort, startIndex, maxCount);
            int count = Persistence.GetLoader<TModel, TFilter>(filter).Count();
            return JsonNet(new { Data = models, Total = count }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NHibernateActionFilter]
        public virtual JsonResult Create(TModel model)
        {
            var persister = Persistence.GetPersister<TModel>(model);
            persister.Save();
            model = persister.Load();
            return JsonNet(new { Data = new TModel[] { model }, Total = 1 });
        }

        [HttpPost]
        [NHibernateActionFilter]
        public virtual JsonResult Update(TModel model)
        {
            var persister = Persistence.GetPersister<TModel>(model);
            persister.Save();
            model = persister.Load();
            return JsonNet(new { Data = new TModel[] { model }, Total = 1 });
        }

        [HttpPost]
        [NHibernateActionFilter]
        public virtual JsonResult Delete(TModel model)
        {
            var persister = Persistence.GetPersister<TModel>(model);
            persister.Delete();
            return JsonNet(new { Data = new TModel[] { model }, Total = 1 });
        }

        #region Protected members
        protected virtual IList<TModel> ListEntities(TFilter filter, IList<KendoSort> sort, int? startIndex = null, int? maxCount = null)
        {
            return Persistence.GetLoader<TModel, TFilter>(filter).List(sort, startIndex, maxCount);
        }
        #endregion
    }
}