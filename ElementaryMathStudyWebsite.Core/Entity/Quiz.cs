using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Quiz : BaseEntity
    {
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; } // Mark that need to be achieved to passed
        [JsonIgnore]
        public bool? Status { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual User? CreatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? LastUpdatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? DeletedByUser { get; set; }
        [JsonIgnore]
        public virtual Chapter? Chapter { get; set; } // Navigation property, one quiz belong to chapter
        [JsonIgnore]
        public virtual Topic? Topic { get; set; } // Navigation property, one quiz belong to one topic
        [JsonIgnore]
        public virtual ICollection<Question>? Questions { get; set; } // Navigation property, one quiz has many question
        [JsonIgnore]
        public virtual ICollection<Progress>? Progresses { get; set; } // Navigation property, one quiz can be in many progresses

        public Quiz() { }

        public Quiz(string quizName, double criteria)
        {
            QuizName = quizName;
            Criteria = criteria;
        }
    }
}
