using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Topic : BaseEntity
    {
        public int? Number { get; set; } // Number use for arranging the topic orderly

        public string TopicName { get; set; } = string.Empty; // avoid null reference issues

        public bool Status { get; set; }

        public double? Criteria { get; set; } // Mark that need to be achieved to passed

        public string? QuizId { get; set; }

        public required string ChapterId { get; set; }

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual User? LastUpdatedByUser { get; set; }
        public virtual User? DeletedByUser { get; set; }
        public virtual Chapter? Chapter { get; set; } // Navigation property, one topic belong to one chapter
        public virtual Quiz? Quiz { get; set; } // Navigation property, one topic can only has one quiz

        public Topic() { }

        public Topic(int? number, string topicName, double? criteria, string chapterId, string? quizId)
        {
            Number = number;
            TopicName = topicName;
            Status = true; // Always true when initialized
            Criteria = criteria;
            ChapterId = chapterId;
            QuizId = quizId;
        }
    }
}
