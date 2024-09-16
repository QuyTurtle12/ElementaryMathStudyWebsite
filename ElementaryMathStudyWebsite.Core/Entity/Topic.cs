using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Topic : BaseEntity
    {
        public int? Number { get; set; } // Number use for arranging the topic orderly

        public string TopicName { get; set; } = string.Empty; // avoid null reference issues

        public bool Status { get; set; }

        public string? QuizId { get; set; }

        public required string ChapterId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual User? CreatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? LastUpdatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? DeletedByUser { get; set; }
        [JsonIgnore]
        public virtual Chapter? Chapter { get; set; } // Navigation property, one topic belong to one chapter
        [JsonIgnore]
        public virtual Quiz? Quiz { get; set; } // Navigation property, one topic can only has one quiz

        public Topic() { }

        public Topic(int? number, string topicName, string chapterId, string? quizId)
        {
            Number = number;
            TopicName = topicName;
            Status = true; // Always true when initialized
            ChapterId = chapterId;
            QuizId = quizId;
        }
    }
}
