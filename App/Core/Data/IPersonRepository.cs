using System.Collections.Generic;
using Core.Entities;


namespace Core
{
              

    public interface IPersonRepository
    {
        IEnumerable<Person> GetAll();
        Person GetById(int id);

        void Save(Person person);

        void Remove(int id);
    }
}
