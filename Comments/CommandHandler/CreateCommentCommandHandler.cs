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

namespace Comments.CommandHandler
{
   
    public class CreateCommentCommandHandler : CommandHandler<CreateComment>
    {
        [InjectionConstructor]
        public CreateCommentCommandHandler()
            : this(ServiceLocator.Current.TryGet<IUnitOfWork>(Extensions.GetUnitOfWorkName<Comment>()))
        {
        }

        public CreateCommentCommandHandler(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            
        }

        protected override void Handle(CreateComment command, InvocationContext context)
        {
            Comment item = new Comment(command.CorrelationId, command.Content);

            var output = this.UnitOfWork.Save<Comment>(item, context);

            this.Result = output.ToResult();
        }
    }
} 
