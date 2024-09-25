using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos
{
    public class SubjectUpdateDTO
    {
        public string SubjectName { get; set; } = string.Empty; // avoid null reference issues
        public double Price { get; set; } = 0;
        public bool Status { get; set; }
    }
}
