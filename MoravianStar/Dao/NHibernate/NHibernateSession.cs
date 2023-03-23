using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using System;

namespace MoravianStar.Dao.NHibernate
{
    public class NHibernateSession
    {
        public static void SessionStart()
        {
            CurrentSession.BeginTransaction();
        }

        public static void SessionEnd(Exception e = null)
        {
            ISession currentSession = CurrentSessionContext.Unbind(sessionFactory);

            using (var session = currentSession)
            {
                if (session == null)
                {
                    return;
                }

                if (!session.Transaction.IsActive)
                {
                    return;
                }

                if (e != null)
                {
                    session.Transaction.Rollback();
                }
                else
                {
                    session.Flush();
                    session.Transaction.Commit();
                }
            }
        }

        public static ISession CurrentSession
        {
            get
            {
                if (!CurrentSessionContext.HasBind(sessionFactory))
                {
                    CurrentSessionContext.Bind(sessionFactory.OpenSession());
                }

                return sessionFactory.GetCurrentSession();
            }
        }

        #region Private members
        private static ISessionFactory BuildSessionFactory()
        {
            return new Configuration()
                .Configure()
                .BuildSessionFactory();
        }

        private static readonly ISessionFactory sessionFactory = BuildSessionFactory();
        #endregion
    }
}