namespace MoravianStar.Dao
{
    public class PresentationModel : IIdentifier
    {
        public int Id { get; set; }

        public bool IsTransient()
        {
            return Id == 0;
        }
    }
}