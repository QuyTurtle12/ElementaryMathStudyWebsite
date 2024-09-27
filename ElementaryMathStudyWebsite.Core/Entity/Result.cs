using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class Result
    {
        public string StudentId { get; set; } = string.Empty;

        public string QuizId { get; set; } = string.Empty;

        public int AttemptNumber { get; set; } = 1;

        public double Score { get; set; } = 0;

        public DateTimeOffset DateTaken { get; set; } = CoreHelper.SystemTimeNow;

        // Navigation Properties
        public virtual Quiz? Quiz { get; set; } // Navigation property, one result can belong to one quizzes

        public virtual User? Student { get; set; } // Navigation property, one result can belong to one student


    }
}
