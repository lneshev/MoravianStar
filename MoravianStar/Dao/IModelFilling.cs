namespace MoravianStar.Dao
{
    public interface IModelFilling<TEntity, TModel>
    {
        void Filling(TEntity entity, TModel model);
    }
}