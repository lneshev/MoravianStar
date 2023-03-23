using MoravianStar.Dao.NHibernate;
using MoravianStar.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoravianStar.Dao
{
    public class Persister<TEntity> : IPersister<TEntity>
        where TEntity : class, IIdentifier
    {
        public Persister(int id)
        {
            Id = id;
        }

        public Persister(TEntity entity)
        {
            Entity = entity;
            Id = Entity.Id;
        }

        public TEntity Load()
        {
            Entity = Persistence.GetLoader<TEntity, EntityFilter>(new EntityFilter() { Ids = new List<int>() { Id } }).List().SingleOrDefault();
            if (Entity == null)
            {
                throw new Exception("Entity not found.");
            }
            
            return Entity;
        }

        public void Save()
        {
            var domainObject = Entity as DomainObject;
            if (domainObject == null)
            {
                throw new Exception("The given entity in not a domain object.");
            }

            bool entityWasTransient = Entity.IsTransient();
            NHibernateSession.CurrentSession.Evict(domainObject); // Fix for violation of unique constraints in DB and business logic
            DependencyInjectionContext.Container.TryResolveAll<IEntitySaving<TEntity>>().ForEach((IEntitySaving<TEntity> x) => x.Saving(Entity));

            NHibernateSession.CurrentSession.SaveOrUpdate(domainObject);
            Id = domainObject.Id;

            DependencyInjectionContext.Container.TryResolveAll<IEntitySaved<TEntity>>().ForEach((IEntitySaved<TEntity> x) => x.Saved(Entity, entityWasTransient));
        }

        public void Delete()
        {
            bool suppressDelete = false;
            DependencyInjectionContext.Container.TryResolveAll<IEntityDeleting<TEntity>>().ForEach((IEntityDeleting<TEntity> x) => x.Deleting(Entity, ref suppressDelete));
            if (!suppressDelete)
            {
                NHibernateSession.CurrentSession.Delete(Entity);
                DependencyInjectionContext.Container.TryResolveAll<IEntityDeleted<TEntity>>().ForEach((IEntityDeleted<TEntity> x) => x.Deleted(Entity));
            }
        }

        #region Protected members
        protected TEntity Entity { get; set; }
        protected int Id { get; set; }
        #endregion
    }

    public class Persister<TEntity, TModel> : Persister<TEntity>, IPersister<TModel>
        where TEntity : class, IIdentifier, new()
        where TModel : class, IIdentifier
    {
        public Persister(int id) : base(id) { }

        public Persister(TModel model) : base(new TEntity())
        {
            Model = model;
            Id = model.Id;
        }

        TModel IPersister<TModel>.Load()
        {
            Model = Persistence.GetLoader<TModel, EntityFilter>(new EntityFilter() { Ids = new List<int>() { Id } }).List().SingleOrDefault();
            if (Model == null)
            {
                throw new Exception("Entity not found.");
            }
            return Model;
        }

        public new void Save()
        {
            if (!Model.IsTransient())
            {
                Entity = Load();
            }

            DependencyInjectionContext.Container.TryResolveAll<IEntityFilling<TEntity, TModel>>().ForEach((IEntityFilling<TEntity, TModel> x) => x.Filling(Entity, Model));

            FillEntity(Entity, Model);

            DependencyInjectionContext.Container.TryResolveAll<IEntityFilled<TEntity, TModel>>().ForEach((IEntityFilled<TEntity, TModel> x) => x.Filled(Entity, Model));

            base.Save();

            Model.Id = Id;
        }

        public new void Delete()
        {
            if (!Model.IsTransient())
            {
                Entity = Load();
            }

            base.Delete();
        }

        #region Private members
        private void FillEntity(TEntity entity, TModel model)
        {
            
        }

        private TModel Model { get; set; }
        #endregion
    }
}