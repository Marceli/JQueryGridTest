using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;


namespace Core.Entities
{
    public class Person
    {
        public Person()
        {
            this.Cats = new List<Cat>();
        }

        public virtual int Id { get; set; }

        public virtual Name Name { get; set; }
       
        public virtual IList<Cat> Cats { get; set; }
		public virtual void AddCat(Cat cat)
		{
		    cat.Person = this;
		    this.Cats.Add(cat);
		}
    }    
}
