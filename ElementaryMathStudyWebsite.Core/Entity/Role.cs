using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<User>? Users { get; set; } // Navigation property, one role has many users

    }
}
