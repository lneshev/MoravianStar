using System.Collections.Generic;

namespace MoravianStar.Dao
{
    public class EntityFilter
    {
        public EntityFilter()
        {
            Ids = new List<int>();
            ExcludeIds = new List<int>();
        }

        public List<int> Ids { get; set; }
        public List<int> ExcludeIds { get; set; }
    }
}