namespace MoravianStar.Dao
{
    public interface IEntityDeleted<TEntity>
    {
        void Deleted(TEntity entity);
    }
}