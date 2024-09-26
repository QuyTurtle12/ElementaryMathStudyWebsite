using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ChapterNumberDto
    {
        public IEnumerable<ChangeNumberDto> ChapterNumbersOrder { get; set; } = new List<ChangeNumberDto>();
    }
    public class ChangeNumberDto
    {
        public required string Id { get; set; }
        public int? Number { get; set; }

    }
}
