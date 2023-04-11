namespace MoravianStar.Dao
{
    public class EntityModelPair<TEntity, TModel>
        where TEntity : class, IEntityBase
        where TModel : class, IModelBase
    {
        public TEntity Entity { get; set; }
        public TModel Model { get; set; }
    }
}