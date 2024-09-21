using ElementaryMathStudyWebsite.Core.Base;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Chapter : BaseEntity
    {
        public int? Number { get; set; } // Number use for arranging the chapter orderly

        public string ChapterName { get; set; } = string.Empty; // avoid null reference issues

        public bool Status { get; set; } = true;

        public required string SubjectId { get; set; }

        public string? QuizId { get; set; } // Quiz may not exist yet and can be created later

        // Navigation properties
        [JsonIgnore]
        public virtual User? CreatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? LastUpdatedByUser { get; set; }
        [JsonIgnore]
        public virtual User? DeletedByUser { get; set; }
        [JsonIgnore]
        public virtual Subject? Subject { get; set; } // Navigation property, one chapter belong to one subject
        [JsonIgnore]
        public virtual ICollection<Topic>? Topics { get; set; } // Navigation property, one chapter has many topics
        [JsonIgnore]
        public virtual Quiz? Quiz { get; set; } // Navigation property, one chapter can only has one quiz

    }
}
