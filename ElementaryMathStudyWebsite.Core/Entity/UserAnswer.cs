using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class UserAnswer
    {
        public string QuestionId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string OptionId { get; set; } = string.Empty;

        public int AttemptNumber { get; set; } = 0;
        
        public virtual Question Question { get; set; } = new(); // Navigation property, one answer belong to question
        public virtual User User { get; set; } = new(); // Navigation property, one answer belong to one user
        public virtual Option Option { get; set; } = new(); // Navigation property, one answer correlated to one option

    }
}
