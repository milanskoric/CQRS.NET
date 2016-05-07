using Comments.Core;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments
{
    public class CanAddCommentValidationHandler : IValidationHandler<CreateComment>
    {
        public IEnumerable<ValidationResult> Validate(CreateComment command)
        {
            List<ValidationResult> output = new List<ValidationResult>();

            if (command == null || string.IsNullOrWhiteSpace(command.CorrelationId))
                output.Add(new ValidationResult(
                    2210, "Content", "Content of message correlation Id is null", string.Empty));

            if (command == null || string.IsNullOrWhiteSpace(command.Content))
                output.Add(new ValidationResult( 
                    2220, "Content", "Content of comment is null or empty", command.CorrelationId));

            return output;
        }
    }
 
}
