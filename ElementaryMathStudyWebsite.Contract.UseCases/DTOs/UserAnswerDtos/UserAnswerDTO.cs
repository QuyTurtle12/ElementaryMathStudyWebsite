using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos
{
    public class UserAnswerDTO
    {
        public required string QuestionId { get; set; }

        public required string UserId { get; set; }

        public required string OptionId { get; set; }
        public required int AttemptNumber { get; set; }
    }
}
