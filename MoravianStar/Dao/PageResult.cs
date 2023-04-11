using System.Collections.Generic;

namespace MoravianStar.Dao
{
    public class PageResult<T>
        where T : class
    {
        public PageResult()
        {
            Items = new List<T>();
        }

        public IEnumerable<T> Items { get; set; }
        public int? TotalCount { get; set; }
        public bool TotalCountGet { get; set; }
    }
}