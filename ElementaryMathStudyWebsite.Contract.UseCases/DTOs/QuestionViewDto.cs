using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuestionViewDto
    {
        public string Id { get; set; } // Ensure this is a GUID
        public string QuestionContext { get; set; } = string.Empty;
        public string QuizId { get; set; } = string.Empty;
        public QuestionViewDto(string questionContext, string quizId)
        {
            QuestionContext = questionContext;
            QuizId = quizId;
        }

        public QuestionViewDto()
        {
        }
    }
}