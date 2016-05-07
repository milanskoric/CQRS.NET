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
    public class Comment : AggregateRoot
    {

        //EF requires a parameter-less constructor, but it can be private
        private Comment()
        {

        }

        //By requiring the rest of the application to only call this constructor,
        //we can ensure we have a valid initialisation of an instance
        public Comment(string correlationId, string content)
        {
            this.SetCommentContent(correlationId, content);

            CommentCreatedEvent change = new CommentCreatedEvent(this);

            this.ApplyChange(change);
        }
        
        //Use Private setters - properties can't be set directly
        [DataMember]
        public string Content { get; private set; }

        //Method must be called, wich can be validated and easily tested
        public void SetCommentContent(string correlationId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) {
                throw new ArgumentNullException("Content parameter must be provided!");
            }
            this.Content = content;
            this.CorrelationId = correlationId;
            this.Version++;
        }

    }
}
