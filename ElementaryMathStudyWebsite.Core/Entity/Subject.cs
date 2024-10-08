﻿using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.Text.Json.Serialization;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class Subject : BaseEntity
    {
        public string SubjectName { get; set; } = string.Empty; // avoid null reference issues

        public double Price { get; set; } = 0;

        public bool Status { get; set; } = true;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<OrderDetail>? Detail { get; set; } // Navigation property, one subject may belong to many order detail
        [JsonIgnore]
        public virtual ICollection<Chapter>? Chapters { get; set; } // Navigation property, one subject has many chapters
        [JsonIgnore]
        public virtual ICollection<Progress>? Progresses { get; set; } // Navigation property, one subject has many progresses

    }
}
