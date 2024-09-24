using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ResultParentViewDto
    {
        public string StudentId { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public IEnumerable<SubjectResult> subjectResults { get; set; } = new List<SubjectResult>();
    }

    public class SubjectResult
    {
        public string SubjectId { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public IEnumerable<ResultInfo> resultInfos { get; set; } = new List<ResultInfo>();
    }

    public class ResultInfo
    {
        public string QuizId { get; set; } = string.Empty;

        public string QuizName { get; set; } = string.Empty;

        public double Score { get; set; } = 0;

        public DateTimeOffset DateTaken { get; set; } = CoreHelper.SystemTimeNow;
    }


}
