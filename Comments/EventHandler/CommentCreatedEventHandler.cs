using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;
using Comments.Core;
using Comments.Domain;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Comments.Domain
{
    public class CommentCreatedEventHandler : DomainEventHandler<CommentCreatedEvent>
    {
        [InjectionConstructor]
        public CommentCreatedEventHandler()
            : this(ServiceLocator.Current.TryGet<IUnitOfWork>(Extensions.GetReadUnitOfWorkName<Comment>()))
        {
    
        }

        public CommentCreatedEventHandler(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

        protected override void Handle(CommentCreatedEvent change, InvocationContext context)
        {
            Comment item = change.Data;

            this.UnitOfWork.Set<Comment>(item, context);

            this.UnitOfWork.Commit();
        }
    }
} 
