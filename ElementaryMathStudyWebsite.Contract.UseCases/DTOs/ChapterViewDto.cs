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