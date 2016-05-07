using Comments.Core;
using Comments.Domain;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: Module(typeof(Comments.Infrastructure.CommentsInfrastructureBootstrapper))]
namespace Comments.Infrastructure
{
 
    [Priority(10)]
    public class  CommentsInfrastructureBootstrapper : UnityContainerExtension, IBootstrapper 
    {

        protected override void Initialize()
        {
            Initialize(this.Container);
        }

        public void Initialize(Microsoft.Practices.Unity.IUnityContainer container)
        {
            var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(a => a.Name.StartsWith("Comments"));

            foreach (var aName in assemblyNames)
            {
                Assembly assembly = Assembly.Load(aName);

                var types = assembly.GetExportedTypes();

                var cmdMessages = from it in types
                                  where it.InheritsFrom<CommandMessage>() == true
                                  select it;

                foreach (Type t in cmdMessages)
                {
                    container.RegisterType(
                        typeof(ICommandMessage),
                        t,
                        t.Name);
                }

                var cmdHandlers = from it in types
                                  where it.Implements(typeof(ICommandHandler<>)) == true
                                  select it;

                foreach (Type t in cmdHandlers)
                {
                   Type msgHandler = t.GetInterfaces().Where(
                       i=>i.IsGenericType 
                           && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                   ).SingleOrDefault();

                   if (msgHandler != null)
                   {

                       container.RegisterType(
                           msgHandler,
                           t);
                   }
                }

                var qurMessages = from it in types
                                  where it.InheritsFrom<IQueryMessage>() == true
                                  select it;

                foreach (Type t in qurMessages)
                {
                    container.RegisterType(
                        typeof(IQueryMessage),
                        t,
                        t.Name);
                }

                var qurHandlers = from it in types
                                  where it.Implements(typeof(IQueryHandler<>)) == true
                                  select it;

                foreach (Type t in qurHandlers)
                {
                    Type msgHandler = t.GetInterfaces().Where(
                        i => i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IQueryHandler<>)
                    ).SingleOrDefault();

                    if (msgHandler != null)
                    {

                        container.RegisterType(
                            msgHandler,
                            t);
                    }
                }
            }

            //Register Write Repository
            container.RegisterType<IRepository, CommentRepository>(CommentRepository.Identifier);

            container.RegisterType<ICommentWriteRepository, CommentRepository>();

            container.RegisterType<IUnitOfWork, CommentUnitOfWork>(
                CommentUnitOfWork.Identifier);
             
            //Register Read Repository
            container.RegisterType<IRepository, CommentsReadRepository>(  
                CommentsReadRepository.Identifier);

            container.RegisterType<ICommentReadRepository, CommentsReadRepository>();

           container.RegisterType<IUnitOfWork, CommentsReadUnitOfWork>(
                CommentsReadUnitOfWork.Identifier);
             

            //Register handler for the event
            container.RegisterType <IEventHandler<CommentCreatedEvent> , CommentCreatedEventHandler > ();
            
        }

        public bool Build()
        {
            IEventStore eventStore = ServiceLocator.Current.TryGet<IEventStore>();

            ICommandBus bus = ServiceLocator.Current.TryGet<ICommandBus>();

            new CommentsReadRepository(eventStore, bus).Build();

            //Create store if not exsist
            return new CommentRepository().Build();
        }

        public void Seed()
        {
             
        }
    }
}

