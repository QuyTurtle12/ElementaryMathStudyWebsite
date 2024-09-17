using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Option : BaseEntity
    {
        public required string QuestionId { get; set; }

        public string? Answer { get; set; } // Answer context

        public bool IsCorrect { get; set; } = false;

        // Navigation properties
        [JsonIgnore]
        public virtual User? CreatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? LastUpdatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? DeletedByUser { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserAnswer>? Answers { get; set; } // Navigation property, one option can be in many user's answer
        [JsonIgnore]
        public virtual Question? Question { get; set; } // Navigation property, one option belong to one question


    }
}
