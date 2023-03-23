using NHibernate;

namespace MoravianStar.Dao.NHibernate
{
    public interface ICriteriaFilled<TEntity, TFilter>
    {
        void Filled(ICriteria criteria, TFilter filter);
    }
}