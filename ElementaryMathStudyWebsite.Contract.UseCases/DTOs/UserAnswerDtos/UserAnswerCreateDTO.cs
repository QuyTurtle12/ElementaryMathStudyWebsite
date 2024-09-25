using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos
{
    public class UserAnswerCreateDTO
    {
        public IEnumerable<UserAnswersDTO> UserAnswerList { get; set; } = new List<UserAnswersDTO>();
    }

    public class UserAnswersDTO
    {
        public required string QuestionId { get; set; }
        public required string OptionId { get; set; }

    }
}
