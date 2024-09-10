using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class Option : BaseEntity
    {
        public required string QuestionId { get; set; }

        public string? Answer {  get; set; } // Answer context

        public bool IsCorrect { get; set; }
        
        public virtual ICollection<UserAnswer>? Answers { get; set; } // Navigation property, one option can be in many user's answer
        public required virtual Question Question { get; set; } // Navigation property, one option belong to one question


        public Option() { }

        public Option(string questionId, string? answer, bool isCorrect)
        {
            QuestionId = questionId;
            Answer = answer;
            IsCorrect = isCorrect;
        }
    }
}
