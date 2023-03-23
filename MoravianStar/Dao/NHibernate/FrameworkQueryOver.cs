using NHibernate.Criterion;
using NHibernate.Impl;

namespace MoravianStar.Dao.NHibernate
{
    public class FrameworkQueryOver<TRoot, TSubType> : QueryOver<TRoot, TSubType>
    {
        public FrameworkQueryOver(CriteriaImpl criteria) : base(criteria)
        {

        }
    }
}