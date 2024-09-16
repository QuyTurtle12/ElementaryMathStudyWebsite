using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OptionUpdateDto
    {
        public string? Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
