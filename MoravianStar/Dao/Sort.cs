namespace MoravianStar.Dao
{
    public class Sort
    {
        public Sort()
        {
            Dir = SortDirection.Asc;
        }

        public string Field { get; set; }
        public SortDirection Dir { get; set; }
    }
}