using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ResultViewDto
    {
        public string StudentName { get; set; } = string.Empty;

        public string QuizName { get; set; } = string.Empty;

        public double Score { get; set; } = 0;

        public int Attempt {  get; set; } = 0;

        public DateTimeOffset DateTaken { get; set; } = CoreHelper.SystemTimeNow;
    }
}
