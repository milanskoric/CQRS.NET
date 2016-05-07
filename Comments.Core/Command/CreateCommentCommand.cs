using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel;

namespace Comments.Core
{
    //  A message sent to tell the system to do something
    [Serializable]
    public class CreateComment : CommandMessage
    {

        public string Content { get; set; }

    }
}
