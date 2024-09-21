using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Repositories.DTOs
{
    public class CreateSubjectDTO
    {
        public string SubjectName { get; set; } = string.Empty;
        public double? Price { get; set; }
    }
}
