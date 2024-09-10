using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class Question : BaseEntity
    {
        public string QuestionContext { get; set; } = string.Empty; // avoid null reference issues
                                                                    // Question Context include one question and many options

        public char Answer { get; set; }

        public required string QuizId { get; set; }

        public required virtual Quiz Quiz { get; set; } // Navigation property, one question belong to one quiz
        public virtual ICollection<Option>? Options { get; set; } // Navigation property, one question has many options
        public virtual ICollection<UserAnswer>? Answers { get; set; } // Navigation property, one question can accept many user's answers

        public Question() { }

        public Question(string questionContext, char answer, string quizId)
        {
            QuestionContext = questionContext;
            Answer = answer;
            QuizId = quizId;
        }
    }
}
