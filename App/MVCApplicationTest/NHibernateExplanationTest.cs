using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using FluentNHibernate.Automapping;
using Core.Entities;
using NHibernate.Linq;


namespace MVCApplicationTest
{
    [TestFixture]
    public class NHibernateExplanationTestSQLLite
    {
        static Configuration config;
        public const string DB_FILE = "firstProject.db";
        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure().
                Database(SQLiteConfiguration.Standard.UsingFile(DB_FILE)        
                    .ShowSql())
                
                .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Person>()
                    .Where(t => t.Namespace == "Core.Entities").Setup(convention =>
                    {
                        convention.IsComponentType =
                            type => type == typeof(Name);
                    })))
        .ExposeConfiguration(BuildSchema).BuildSessionFactory();
        }
        private static void BuildSchema(Configuration config)
        {
            NHibernateExplanationTestSQLLite.config = config;
            // delete the existing db on each run
            if (File.Exists(DB_FILE))
               File.Delete(DB_FILE);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            
        }

        [Test]
        public void Basic()
        {
            Assert.AreEqual(1, 1);
        }
        ISessionFactory factory;
        [TestFixtureSetUp]
        public void CreateFactory()
        {
            factory = CreateSessionFactory();

        }
        [TestFixtureTearDown]
        public void DisposeFactory()
        {
            factory.Dispose();

        }
        public ISession GetSession()
        {
            var session=factory.OpenSession();
            new SchemaExport(config).Execute(false, true, false, session.Connection, Console.Out);
            return session;
        }
        [Test]
        public void CanReadAndSavePerson()
        {
            int personId;
            using (var session = GetSession())
            {
                
                
                using (var transaction = session.BeginTransaction())
                {
                    
                    // create a couple of Stores each with some Products and Employees
                    var person = new Person { Name = new Name { First = "Olo", Last = "Bolus" } };
                    session.SaveOrUpdate(person);
                    transaction.Commit();
                    personId = person.Id;
                    session.Evict(person);
                    //session.Linq<Person>().Where(p=>p.Id==1);
                }

                using (session.BeginTransaction())
                {
                    var query=(from user in session.Linq<Person>() select user );

                    var person = query.First<Person>();
                    //Assert.AreEqual("Olo", person.FirstName);
                    //Assert.AreEqual("Bolus", person.LastName);


                }
               
            }
            using (var session = GetSession())
            {
                

                using (var transaction = session.BeginTransaction())
                {
                    // create a couple of Stores each with some Products and Employees
                    var person = new Person { Name = new Name { First = "Olo", Last = "Bolus" } };
                    session.SaveOrUpdate(person);
                    transaction.Commit();
                    personId = person.Id;
                    session.Evict(person);
                    //session.Linq<Person>().Where(p=>p.Id==1);
                }

                using (session.BeginTransaction())
                {
                    var query = (from user in session.Linq<Person>() where user.Name.First == "Bolus1" select user);

                    var person = query.First<Person>();
                  
                    Assert.AreEqual("Olo1", person.Name.First);
                    Assert.AreEqual("Bolus1", person.Name.Last);
                }

            }

        }
    }

 
}
