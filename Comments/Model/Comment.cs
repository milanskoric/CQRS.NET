using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Domain
{
   
    [Serializable]
    [DataContract]
    public class Comment : CoreObject
    {
        //the private constructor - for EF
        private Comment()
        {

        }

        public Comment(string correlationId, string content)
        {
            this.Content = content;
            this.Version = 1;
            this.CorrelationId = correlationId;

            CommentCreatedEvent change = new CommentCreatedEvent(this);

            this.ApplyChange(change);
        }
        
        [DataMember]
        public string Content { get; set; }


    }
}
