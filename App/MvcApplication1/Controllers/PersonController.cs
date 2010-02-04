using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using AutoMapper;
using Core;
using System.Linq.Expressions;
using Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Web.Script.Serialization;
using System.Reflection;
using MvcApplication1.Models;
using MvcApplication1;



namespace MvcApplication1.Controllers
{


     public class PersonController : Controller
    {

        IService _service;
       
        public PersonController(IService service)
        {
            _service = service;
            
           
        }
        


        public ActionResult GridData(string sidx, string sord, int page, int rows)
        {
            //PopulateDB();
          
            var pageIndex = page - 1;
            var persons = (from person in db.Persons select person).Skip(pageIndex * rows).Take(rows).OrderBy(sidx + " " + sord).ToList<Person>();
            Mapper.CreateMap<Person, PersonView>();
            var personViews=Mapper.Map<IEnumerable<Person>,List<PersonView>>(persons);
            var personsCount = db.Persons.Count();
           
           
            var jsonData = new
            {
                total = (int)Math.Ceiling((float)personsCount / (float)rows),
                page = page,
                records = persons.Count(), // implement later 
                rows = (from person in persons 
                        select new
                        {
                            id = person.Id,
                            cell = new string[] { person.Id.ToString(), person.Name.First, person.Name.Last }
                        }
                )
            };
           
            return Json(jsonData,JsonRequestBehavior.AllowGet);
        }
            

        
        
        public ActionResult Index()
        {
            return View();
        }
        

        [HttpPost]
        public ActionResult Update(PersonView personView)
        {
            
            if (!this.ModelState.IsValid)
            {
                return View("Edit");
            }
            Mapper.CreateMap<PersonView, Name>().
                ForMember(dest => dest.First, opt => opt.MapFrom(src => src.NameFirst)).
                ForMember(dest => dest.Last, opt => opt.MapFrom(src => src.NameLast));

            Mapper.CreateMap<PersonView, Person>();
            var person = Mapper.Map<PersonView, Person>(personView);
            person.Name = Mapper.Map<PersonView, Name>(personView);            
            db.Session.SaveOrUpdate(person);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Remove(int id)
        {
            db.Session.Delete(db.Session.Get<Person>(id));
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int? id)
        {
            PersonView personToEdit=new PersonView();;
            if (id==null)
            {

                personToEdit = new PersonView();
                personToEdit.Id = -1;
            }
            else
            {
                //using (var tx = _session.BeginTransaction())
                //{
                //    personToEdit = _session.Get<Person>(id);
                //    tx.Commit();
                //}

            }
            return View(personToEdit);
        }
    }
}
