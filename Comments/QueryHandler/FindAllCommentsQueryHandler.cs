using Comments.Core;
using Microsoft.Practices.ServiceLocation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Domain 
{
    public class FindAllCommentsQueryHandler : QueryHandler<FindAllComments>
    {
        protected override Result Retrieve(FindAllComments query, InvocationContext ctx)
        {
            Result output = new Result();

            output.Success = false;

            string read = query.GetRepositoryName();

            using (IRepository svc = ServiceLocator.Current.TryGet<IRepository>(read))
            {
                var q = svc.GetQueryable<Comment>();

                QueryOptions option = ctx.Options as QueryOptions;

                if (option != null)
                {
                    if (option.HasLimit())
                    {
                        q = q.Take(option.GetLimit());
                    }
                }

                var id = query.Key.ToLong();

                if (id > 0)
                {
                    q = q.Where(i => i.Id == id);
                }

                var result = q.ToList();

                output.Success = (result != null && result.Count > 0);

                if (output.Success)
                {
        

                    output.RecordsAffected = result.Count;
                    output.Data = result;
                } 
            }

            return output;
        }
    }
}
