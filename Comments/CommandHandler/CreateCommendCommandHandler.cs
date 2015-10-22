using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;
using Comments.Core;
using Comments.Domain;
using Microsoft.Practices.ServiceLocation;

namespace Comments.CommandHandler
{
   
    public class CreateCommentCommandHandler : CommandHandler<CreateCommentCommand>
    {
        protected override void Handle(CreateCommentCommand command, InvocationContext context)
        {
            Comment item = new Comment(command.CorrelationId, command.Content);

            string write = item.GetWriteRepositoryName();

            using (IRepository svc = ServiceLocator.Current.TryGet<IRepository>(write))
            {
                var r = svc.Save<Comment>(item, context);

                this.Result = r.ToResult();
            }

        }
    }
} 
