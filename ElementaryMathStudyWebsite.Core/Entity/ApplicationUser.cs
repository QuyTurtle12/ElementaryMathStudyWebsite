using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Identity;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

        // Navigation Properties
        public virtual User? UserInfo { get; set; }
        public virtual ICollection<ApplicationUserClaims> Claims { get; set; } = new List<ApplicationUserClaims>();

        public ApplicationUser()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
