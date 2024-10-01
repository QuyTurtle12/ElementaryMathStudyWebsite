using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class ApplicationRoleClaims : IdentityRoleClaim<string>
    {
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

        // Foreign key reference to ApplicationRole
        [ForeignKey("Role")]
        public override string RoleId { get; set; } = string.Empty;

        public virtual ApplicationRole Role { get; set; } = new ApplicationRole();

        public ApplicationRoleClaims()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
