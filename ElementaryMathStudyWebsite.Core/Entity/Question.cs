using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Question : BaseEntity
    {
        public string QuestionContext { get; set; } = string.Empty; // avoid null reference issues
                                                                    // Question Context include one question and many options
        public required string QuizId { get; set; }

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual User? LastUpdatedByUser { get; set; }
        public virtual User? DeletedByUser { get; set; }
        public virtual Quiz? Quiz { get; set; } // Navigation property, one question belong to one quiz
        public virtual ICollection<Option>? Options { get; set; } // Navigation property, one question has many options
        public virtual ICollection<UserAnswer>? Answers { get; set; } // Navigation property, one question can accept many user's answers

    }
}
