namespace MoravianStar.Dao
{
    public interface IEntitySaved<TEntity>
    {
        void Saved(TEntity entity, bool entityWasTransient);
    }
}