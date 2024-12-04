using ElementaryMathStudyWebsite.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Core.Base
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString().ToUpper();
            CreatedTime = LastUpdatedTime = CoreHelper.SystemTimeNow;
        }

        [Key]
        public string Id { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        [Display(Name = "Last Updated By")]
        public string? LastUpdatedBy { get; set; }
        [Display(Name = "Deleted By")]
        public string? DeletedBy { get; set; }
        [Display(Name = "Created Time")]
        public DateTimeOffset CreatedTime { get; set; }
        [Display(Name = "Last Updated Time")]
        public DateTimeOffset LastUpdatedTime { get; set; }
        [Display(Name = "Deleted Time")]
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
