namespace MoravianStar.Dao
{
    public interface IIdentifier
    {
        int Id { get; set; }
        bool IsTransient();
    }
}