using System.Collections.Generic;

namespace MoravianStar.Dao
{
    public class KendoDataSourceRequest
    {
        public KendoDataSourceRequest()
        {
            Page = 1;
            //this.Aggregates = (IList<AggregateDescriptor>)new List<AggregateDescriptor>();
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<KendoSort> Sort { get; set; }
        //public IList<IFilterDescriptor> Filters { get; set; }
        //public IList<GroupDescriptor> Groups { get; set; }
        //public IList<AggregateDescriptor> Aggregates { get; set; }
    }
}