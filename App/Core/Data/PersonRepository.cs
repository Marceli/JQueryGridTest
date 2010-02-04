using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Core.Entities;


namespace Core.Data
{
    public class PersonRepository: IPersonRepository 
    {
        ISession session;
        public PersonRepository(ISession session)
        {
            this.session = session;
        }

        #region IPersonRepository Members

        public IEnumerable<Person> GetAll()
        {
          ICriteria criteria= session.CreateCriteria<Person>();
          return criteria.List<Person>();
         
        }

        public Person GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(Person person)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
