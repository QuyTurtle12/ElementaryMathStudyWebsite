using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizMainViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; }
        public bool? Status { get; set; }
        public string? ChapterName { get; set; }
        public string? TopicName { get; set; }   
        public string CreatedBy { get; set; } = string.Empty;
        public string LastUpdatedBy { get; set; } = string.Empty; 
    }
}
