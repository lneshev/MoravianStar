using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;

namespace MoravianStar.Dao.NHibernate
{
    public static class CriteriaExtensions
    {
        public static QueryOver<TRoot, TRoot> CreateQueryOver<TRoot>(this ICriteria criteria)
        {
            return new FrameworkQueryOver<TRoot, TRoot>(criteria as CriteriaImpl);
        }
    }
}