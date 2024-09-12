using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class SubjectCreateDTO
    {
        public string SubjectName { get; set; } = string.Empty; // avoid null reference issues

        public double? Price { get; set; }
    }
}
