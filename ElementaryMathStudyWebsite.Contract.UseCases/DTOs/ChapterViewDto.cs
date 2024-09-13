using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ChapterViewDto
    {

        public int? Number { get; set; }
        public string ChapterName { get; set; } = string.Empty;

        public ChapterViewDto() { }

        public ChapterViewDto(int? number, string chapterName)
        {
            Number = number;
            ChapterName = chapterName;
        }
    }
}
