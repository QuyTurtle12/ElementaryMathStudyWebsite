namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class UserAnswer
    {
        public required string QuestionId;

        public required string UserId;

        public required string OptionId;

        public virtual Question? Question { get; set; } // Navigation property, one answer belong to question
        public virtual User? User { get; set; } // Navigation property, one answer belong to one user
        public virtual Option? Option { get; set; } // Navigation property, one answer correlated to one option

        public UserAnswer() { }

        public UserAnswer(string questionId, string userId, string optionId)
        {
            QuestionId = questionId;
            UserId = userId;
            OptionId = optionId;
        }
    }
}
