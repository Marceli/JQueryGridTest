using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Data;
using NHibernate;
using Core.Entities;
using NHibernate.Linq;

namespace MvcApplication1
{
    public static class db 
    {
        
        public static IUnitOfWork UnitOfWork 
        { 
            get {return MvcApplication.Container.Resolve<IUnitOfWork>();}
        }
        public static IOrderedQueryable<Person> Persons
        {
            get { return ((IOrderedQueryable <Person>)MvcApplication.Container.Resolve<IUnitOfWork>().CurrentSession.Linq<Person>()); }
        }
        public static ISession Session
        {
            get { return MvcApplication.Container.Resolve<IUnitOfWork>().CurrentSession; }
        }
    }
    public class UnitOfWork:IUnitOfWork,IDisposable
    {
        #region IDisposable Members
        
        ITransaction _transaction;
        public UnitOfWork()
        {
            ISessionProvider sessionProvider=MvcApplication.Container.Resolve<ISessionProvider>();
            CurrentSession = sessionProvider.Create();
            try
            {
                _transaction = CurrentSession.BeginTransaction();
            }
            catch { }
        }
        public ISession CurrentSession {get;private set;}

    
        public void Dispose()
        {
           _transaction.Commit();
           _transaction.Dispose();
            CurrentSession.Dispose();
        }
        public void Commit()
        {
         _transaction.Commit();
        }

        #endregion
    }
}