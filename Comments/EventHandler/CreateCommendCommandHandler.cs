using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;
using Comments.Core;
using Comments.Domain;
using Microsoft.Practices.ServiceLocation;

namespace Comments.Domain
{
    public class CommentCreatedEventHandler : DomainEventHandler<CommentCreatedEvent>
    {
        protected override void Handle(CommentCreatedEvent change, InvocationContext context)
        {
            Comment item = change.Data;

            string read = item.GetReadRepositoryName();

            using (IRepository svc = ServiceLocator.Current.TryGet<IRepository>(read))
            {
                svc.Save<Comment>(item, context);
            }
        }
    }
} 
