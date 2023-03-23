namespace MoravianStar.Dao
{
    public class DomainObject : IIdentifier
    {
        public virtual int Id { get; set; }

        public virtual bool IsTransient()
        {
            return Id == 0;
        }
    }
}