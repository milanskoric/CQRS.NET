using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    [Serializable]
    public class Result
    {
        public Result()
        {
            this.ObjectId = "0";
        }
         
        public Result(ICommandMessage message)
        {
            this.ObjectId = message.CorrelationId;
        }

        public static Result GetUnknowResult()
        {
            Result it = new Result();
            it.Success = false;
            it.Message = "UnknowCommandResult";
            return it;
        }

        public static Result GetBadReqResult()
        {
            Result it = new Result();
            it.Success = false;
            it.Message = "Bad Req";
            return it;
        }

        public string ObjectId { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public string ExecuteTime { get;set;}

        public int RecordsAffected {get;set;}

        public IEnumerable<ValidationResult> ValidationResults { get;set;}
    }
     
    public class Result<TData> : Result
    {
        public new TData Data { get; set; }
    }
}
