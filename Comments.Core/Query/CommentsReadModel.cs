using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Core 
{
    public class FindAllComments : IQueryMessage
    {
        public string Key { get; set; }

        public DateTime TimeStamp
        {
            get;
            set;
        }

        public string CorrelationId
        {
            get;
            set;
        }
    }
}
