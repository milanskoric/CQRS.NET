using Comments.Core;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using SharedKernel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Domain 
{
    public class FindAllCommentsQueryHandler : QueryHandler<FindAllComments>
    {
        [InjectionConstructor]
        public FindAllCommentsQueryHandler()
            : this(ServiceLocator.Current.TryGet<IRepository>(Extensions.GetRepositoryName<FindAllComments>() + "Read"))
        { 
            
        }

        public FindAllCommentsQueryHandler(IRepository repository)
            : base(repository)
        { 
             
        }

        protected override Result Retrieve(FindAllComments query, InvocationContext ctx)
        {
            Result output = new Result();

            var q = this.Repository.BuildQuery<Comment>(ctx.Options as QueryOptions);

            var id = query.Key.ToLong(0);

            if (id != 0)
                q = q.Where(i => i.Id == id);

            var result = this.Repository.ExecuteList<Comment>(q);

            output.Success = (result != null && result.Count() > 0);

            if (output.Success)
            {
                output.RecordsAffected = result.Count();
                output.Data = result;
            } 

            return output;
        }
    }
}
