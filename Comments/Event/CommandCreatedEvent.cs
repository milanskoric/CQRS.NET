using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Domain 
{
     [Serializable]
    public class CommentCreatedEvent : DomainEvent<Comment>
    {
        public CommentCreatedEvent(Comment data)
        {
            this.AggregateRootID = data.Id;
            this.EntityVersion = data.Version;
            this.Data = data;
            this.CorrelationId = data.CorrelationId;
        }
 
    }
}
