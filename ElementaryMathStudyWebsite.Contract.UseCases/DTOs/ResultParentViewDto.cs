using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class ResultParentViewDto
    {
        public string StudentName {  get; set; } = string.Empty;

        public IEnumerable<SubjectResult> subjectResults { get; set; } = new List<SubjectResult>();
    }

    public class SubjectResult
    {
        public string SubjectName { get; set; } = string.Empty;

        public IEnumerable<ResultInfo> resultInfos { get; set; } = new List<ResultInfo>();
    }

    public class ResultInfo
    {    

        public string QuizName { get; set; } = string.Empty;

        public double Score { get; set; } = 0;

        public DateTimeOffset DateTaken { get; set; } = CoreHelper.SystemTimeNow;
    }


}
