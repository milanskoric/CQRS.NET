using Comments.Core;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments
{
    public class CanAddCommentValidationHandler : IValidationHandler<CreateCommentCommand>
    {
        public IEnumerable<ValidationResult> Validate(CreateCommentCommand command)
        {
            List<ValidationResult> output = new List<ValidationResult>();

            if (command == null || string.IsNullOrWhiteSpace(command.Content))
                output.Add(new ValidationResult(){Code=2233, Target = "Content", Message = "Content of comment is null or empty", Success = false, ObjectId = command.CorrelationId});

            return output;
        }
    }
 
}
