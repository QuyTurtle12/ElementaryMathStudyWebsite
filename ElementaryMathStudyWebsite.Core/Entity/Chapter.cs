using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Chapter : BaseEntity
    {
        public int Number { get; set; } = 0; // Number use for arranging the chapter orderly

        public string ChapterName { get; set; } = string.Empty; // avoid null reference issues

        public bool Status { get; set; } = true;

        public string SubjectId { get; set; } = string.Empty;

        public string? QuizId { get; set; } // Quiz may not exist yet and can be created later

        // Navigation properties
        [JsonIgnore]
        public virtual Subject? Subject { get; set; } // Navigation property, one chapter belong to one subject
        [JsonIgnore]
        public virtual ICollection<Topic>? Topics { get; set; } // Navigation property, one chapter has many topics
        [JsonIgnore]
        public virtual Quiz? Quiz { get; set; } // Navigation property, one chapter can only has one quiz
        //[JsonIgnore]
        //public virtual User? User { get; set; }
    }
}
