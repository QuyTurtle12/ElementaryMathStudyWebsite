using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class ApplicationUserClaims : IdentityUserClaim<string>
    {
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

        // Foreign key reference to ApplicationUser
        [ForeignKey("User")]
        public override string UserId { get; set; } = string.Empty;

        public virtual ApplicationUser User { get; set; } = new ApplicationUser();
        public ApplicationUserClaims()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
