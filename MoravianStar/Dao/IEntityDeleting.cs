namespace MoravianStar.Dao
{
    public interface IEntityDeleting<TEntity>
    {
        void Deleting(TEntity entity, ref bool suppressDelete);
    }
}