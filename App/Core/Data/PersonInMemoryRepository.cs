using System.Collections.Generic;
using System.Linq;
using Core.Entities;


namespace Core
{
    public class PersonInMemoryRepository:IPersonRepository
    {
        static Dictionary<int,Person> _persons = new Dictionary<int,Person>();

        static PersonInMemoryRepository()
        {
            for (int i = 0; i < 300; i++)
            {
                _persons.Add(i, new Person() { Name = new Name { First = "olo" + i.ToString(), Last = "bolo" + i.ToString()} });

            }
        }

        #region IPersonRepository Members

        public IEnumerable<Person> GetAll()
        {
            return from person in _persons.Values 
                   orderby person.Id 
                   select person ;
            
        }

        public Person GetById(int id)
        {

            return _persons[id];
        }
        public void Save(Person personToSave)
        {
            if (personToSave.Id == null || personToSave.Id < 0)
            {
                int maxId = (from person in _persons.Values select person.Id).Max() + 1;
                personToSave.Id = maxId;
                _persons.Add(maxId, personToSave);

            }
            _persons[personToSave.Id] = personToSave;
        }

        #endregion

        #region IPersonRepository Members


        public void Remove(int id)
        {
            _persons.Remove(id);
        }

        #endregion
    }
}
