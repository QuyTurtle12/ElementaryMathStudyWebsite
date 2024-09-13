using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public interface ISubjectBaseDTO
    {
        string SubjectName { get; set; }
        double? Price { get; set; }
        bool Status { get; set; }
    }
}
