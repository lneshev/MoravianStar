namespace MoravianStar.Dao
{
    public interface IModelFilled<TEntity, TModel>
    {
        void Filled(TEntity entity, TModel model);
    }
}