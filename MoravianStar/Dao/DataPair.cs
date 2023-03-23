namespace MoravianStar.Dao
{
    public class DataPair<TEntity, TModel>
    {
        public TEntity Entity { get; set; }
        public TModel Model { get; set; }
    }
}