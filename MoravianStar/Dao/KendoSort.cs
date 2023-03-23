namespace MoravianStar.Dao
{
    public class KendoSort
    {
        public KendoSort()
        {
            Dir = KendoSortDirection.Asc;
        }

        public string Field { get; set; }
        public KendoSortDirection Dir { get; set; }
    }
}