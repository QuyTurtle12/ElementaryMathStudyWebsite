using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuizCreateDto
    {
        public string QuizName { get; set; }
        public double? Criteria { get; set; }
        public string ChapterId { get; set; }
        public string TopicId { get; set; }
    }
}
