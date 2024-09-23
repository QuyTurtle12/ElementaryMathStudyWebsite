using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Topic : BaseEntity
    {

        public int? Number { get; set; } // Number use for arranging the topic orderly

        public string TopicName { get; set; } = string.Empty;

        public string? TopicContext { get; set; }

        public bool Status { get; set; } = true;

        public string? QuizId { get; set; }

        public string ChapterId { get; set; } = string.Empty;

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
    }
}
