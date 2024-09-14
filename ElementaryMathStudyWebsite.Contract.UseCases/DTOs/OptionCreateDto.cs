using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OptionCreateDto
    {
        public required string QuestionId { get; set; }
        public string? Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
