namespace MoravianStar.Dao
{
    public interface IPersister<TContract>
        where TContract : class, IIdentifier
    {
        TContract Load();
        void Save();
        void Delete();
    }
}