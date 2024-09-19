using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public class ResultViewDto
    {
        public string StudentName { get; set; } = string.Empty;

        public string QuizName {  get; set; } = string.Empty;

        public double Score { get; set; } = 0;

        public DateTimeOffset DateTaken { get; set; } = CoreHelper.SystemTimeNow;
    }
}
