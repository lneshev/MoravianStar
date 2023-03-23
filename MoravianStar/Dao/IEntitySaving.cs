namespace MoravianStar.Dao
{
    public interface IEntitySaving<TEntity>
    {
        void Saving(TEntity entity);
    }
}