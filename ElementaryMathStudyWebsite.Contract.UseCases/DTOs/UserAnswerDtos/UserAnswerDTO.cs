using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos
{
    public class UserAnswerDTO
    {
        public required string QuestionId;

        public required string UserId;

        public required string OptionId;
    }
}
