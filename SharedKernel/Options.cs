using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class OperationOptions
    {
        //set format of data 
        public string Format {get;set;}

        public bool HasFormat()
        {
            return string.IsNullOrWhiteSpace(Format) == false;
        }

        //set format of data 
        public int LangCode { get; set; }
    }

    public class CommandOptions : OperationOptions
    {
        // Get or Set a value indicating whether the command for this message is processed asynchronously or not
        public bool IsAsync { get; set; }
    }

    public class QueryOptions : OperationOptions
    {
        // Specifies a limit of results.

        public string Limit { get; set; }

        public bool HasLimit()
        {
            return string.IsNullOrWhiteSpace(Limit) == false && GetLimit() > 0;
        }

        public int GetLimit()
        {
            return Limit.ToInt(0);
        }

        public bool WithWrap { get; set; }

         
    }
}
