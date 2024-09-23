using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuestionUpdateDto
    {
        public string Id { get; set; }  = string.Empty;
        public string QuestionContext { get; set; } = string.Empty;
        public required string QuizId { get; set; }
    }
}
