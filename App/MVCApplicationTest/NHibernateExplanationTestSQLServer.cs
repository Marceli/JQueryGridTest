using System;
using System.Linq;
using Core.Data;
using NUnit.Framework;
using NHibernate;
using Core.Entities;
using NHibernate.Linq;
using NHibernate.Criterion;

namespace MVCApplicationTest
{
    [TestFixture]
    public class NHibernateExplanationTestSQLServer
    {

		ISessionFactory factory;
		[TestFixtureSetUp]
		public void CreateFactory()
		{
			factory = new SessionProvider().Factory;
		}

		[TestFixtureTearDown]
		public void DisposeFactory()
		{
			factory.Dispose();
		}

        [Test]
        public void ConfigurationTest()
        {
			using (var session = new SessionProvider().Create()) { }
        }

       
        [Test]
        public void CanReadAndSavePerson()
        {
            factory = new SessionProvider().Factory;
            int personId;
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var person = new Person { Name = new Name { First = "Olo", Last = "Bolus" } };
					var cat = new Cat { Name = "Filip" };
					session.Save(cat);
					session.Save(person);
					transaction.Commit();
					personId = person.Id;
				}
			}
            using (var session = factory.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    var person = (from p in ((IOrderedQueryable<Person>)session.Linq<Person>()) where p.Id == personId select  p).First()  ;
                    Assert.That(person.Name.First,Is.EqualTo("Olo"));
                }
            }
        }

        [Test]
        public void CanReadAndSavePersonWithCats()
        {
            int personid;
            using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // create a couple of Cats each with connected to Person
                    var person = new Person { Name = new Name { First = "Olo", Last = "Bolus" } };
                    session.Save(person);
                    personid = person.Id;
                    Console.WriteLine(person.Id);
                    person.AddCat(new Cat { Name = "Filip" });
                    person.AddCat(new Cat { Name = "Nick" });
                   
                    session.Save(person);
                    transaction.Commit();
                }
			}
			using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
					 var dbPerson = session.Get<Person>(personid);
					Assert.AreEqual(2, dbPerson.Cats.Count);
				}
            }
        }

        [Test]
        public void FuturesTest()
        {
            int personid;
			using (var session = factory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var person = new Person { Name = new Name { First = "Olo", Last = "Bolus" } };
					Console.WriteLine(person.Id);
					person.AddCat(new Cat { Name = "Filip" });
					person.AddCat(new Cat { Name = "Nick" });
					session.Save(person);
					personid = person.Id;
					transaction.Commit();
				}
			}
            using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var dbPerson = session.CreateCriteria<Person>().
						Add(Expression.Eq("Id", personid)).
						SetFetchMode("Cats", FetchMode.Join).
						Future<Person>();
                    var catscount = session.CreateCriteria<Cat>()
						.SetProjection(Projections.Count(Projections.Id()))
						.FutureValue<int>();
                    transaction.Commit();
					Assert.AreEqual(2, dbPerson.First<Person>().Cats.Count);
					Assert.AreEqual(2, catscount.Value);
                }
            }
        }

        [Test]
        public void CanReadAndSavePersonWithTwoSessions()
        {
            int personid;
            Person dbPerson;
            using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var person = new Person { Name = new Name { First = "Olo", Last = "Bolus" } };
                    personid = person.Id;
                    Console.WriteLine(person.Id);
                    person.AddCat(new Cat { Name = "Filip" });
                    person.AddCat(new Cat { Name = "Nick" });
                    person.AddCat(new Cat { Name = "Cage" });
                    session.Save(person);
                    transaction.Commit();
                    personid = person.Id;
                }
                
                dbPerson = session.Get<Person>(personid);
                Assert.AreEqual(3, dbPerson.Cats.Count);
            }
            dbPerson.Name.Last = "New Name";
            using (var session = factory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(dbPerson);
                    transaction.Commit();
                }
                var dbPerson2 = session.Get<Person>(personid);
                Assert.AreEqual(3, dbPerson2.Cats.Count);
                Assert.That(dbPerson2.Name.Last,Is.EqualTo("New Name"));
            }
        }
    }
}
