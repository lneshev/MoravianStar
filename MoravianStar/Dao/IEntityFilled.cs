namespace MoravianStar.Dao
{
    public interface IEntityFilled<TEntity, TModel>
    {
        void Filled(TEntity entity, TModel model);
    }
}