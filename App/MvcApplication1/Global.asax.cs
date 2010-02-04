using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using MvcContrib.Castle;
using MvcApplication1.Controllers;
using Core;
using Castle.MicroKernel.Registration;
using Core.Data;
using Castle.Core;
using NHibernate;
using Castle.Facilities.FactorySupport;

namespace MvcApplication1
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Person", action = "index", id = "" }  // Parameter defaults
            );

        }
        /// <summary>
        /// Instantiate the container and add all Controllers that derive from 
        /// WindsorController to the container.  Also associate the Controller 
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeWindsor()
        {
            if (_container == null)
            {
                _container = new WindsorContainer();
                ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(Container));
                _container
                    .RegisterControllers(typeof(HomeController).Assembly)
                    .AddComponent<IService, Service>()
                    .AddComponent<IPersonRepository, PersonInMemoryRepository>()
                .AddComponentLifeStyle<ISessionProvider, SessionProvider>(LifestyleType.Singleton);
                _container.AddComponentLifeStyle<IUnitOfWork, UnitOfWork>(LifestyleType.PerWebRequest);
            }
        }
        

        protected void Application_Start()
        {
            InitializeWindsor();
            RegisterRoutes(RouteTable.Routes);
            BeginRequest += new EventHandler(MvcApplication_BeginRequest);
            EndRequest += new EventHandler(MvcApplication_EndRequest);
           
           


        }

        void MvcApplication_EndRequest(object sender, EventArgs e)
        {
        }

        void MvcApplication_BeginRequest(object sender, EventArgs e)
        {

        }
        
        private static WindsorContainer _container;
        public static IWindsorContainer Container
        {
            get { return _container; }
        }
    }
}