using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Identity;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class ApplicationRole : IdentityRole<string>
    {
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

        // Navigation Properties
        public virtual Role? Role { get; set; }
        public virtual ICollection<ApplicationRoleClaims> RoleClaims { get; set; } = new List<ApplicationRoleClaims>();

        public ApplicationRole()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
