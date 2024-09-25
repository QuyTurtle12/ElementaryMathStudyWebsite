using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos
{
    public class UserAnswerWithDetailsDTO
    {
        public string QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string OptionId { get; set; }
        public string OptionAnswer { get; set; }
        public int AttemptNumber { get; set; }
    }

}
