namespace MoravianStar.Dao
{
    public interface IEntityFilling<TEntity, TModel>
    {
        void Filling(TEntity entity, TModel model);
    }
}