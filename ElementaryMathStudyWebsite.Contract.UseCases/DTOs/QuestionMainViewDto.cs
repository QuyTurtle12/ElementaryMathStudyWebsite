using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuestionMainViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string QuestionContext { get; set; } = string.Empty;

        public string QuizName { get; set; } = string.Empty;
        public string QuizId { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string CreatorPhone { get; set; } = string.Empty;
        public DateTimeOffset CreatedTime { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedPersonName { get; set; } = string.Empty;
        public string LastUpdatedPersonPhone { get; set; } = string.Empty;
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}
