using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class CreateQuestionDto
    {
        public required string QuestionContext { get; set; }
        public required string QuizId { get; set; }
    }
}
