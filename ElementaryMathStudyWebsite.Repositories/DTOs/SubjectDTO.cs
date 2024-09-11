using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Repositories.DTOs
{
    public class SubjectDTO
    {
        public string SubjectName { get; set; } = string.Empty; // avoid null reference issues

        public double? Price { get; set; }

        public bool Status { get; set; }
    }
}
