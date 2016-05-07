using Comments.Core;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Reflection;
using System.Web.Http;
using SharedKernel.Infrastructure;

namespace CQRS.NET
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            BuildContainer();
        }

        protected static UnityContainer _container = null;

        private void BuildContainer()
        {
            if (_container == null)
                _container = new UnityContainer();

            _container.RegisterType<IEventStore, EventStore>();
            _container.RegisterType<IQueryDispatcher, QueryDispatcher>();
            _container.RegisterType<ICommandDispatcher, CommandDispatcher>();
            _container.RegisterType<ICommandBus, CommandBus>();

            UnityServiceLocator locator = new UnityServiceLocator(_container);

            ServiceLocator.SetLocatorProvider(() => locator);

            

            var typeFinder = new AvailableTypeFinder(a => a.GetAttribute<ModuleAttribute>() != null);

            List<Type> bootstrapperTypes = typeFinder.GetConcreteTypes(
                it => it.DerivesFrom(typeof(UnityContainerExtension))).ToList();

            bootstrapperTypes.SortByPriority<Type>();

            bootstrapperTypes.ForEach(b => initBootstrapper(b, _container));                
        }
        
        private void initBootstrapper(Type bootstrapperType, UnityContainer container)
        {
            UnityContainerExtension extension = null;
 
            extension = (UnityContainerExtension)container.Resolve(bootstrapperType);

            container.AddExtension(extension);

            IBootstrapper bootstrapper = extension as IBootstrapper;

            bootstrapper.Build();
        }
    }
}
