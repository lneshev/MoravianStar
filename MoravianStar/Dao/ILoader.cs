using System.Collections.Generic;

namespace MoravianStar.Dao
{
    public interface ILoader<TEntity, TFilter>
        where TEntity : class, IIdentifier
        where TFilter : EntityFilter
    {
        IList<TEntity> List(IList<KendoSort> sort = null, int? startIndex = null, int? maxCount = null);
        int Count();
    }
}