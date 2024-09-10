using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;

        public virtual ICollection<User>? Users { get; set; } // Navigation property, one role has many users

        public Role () { }

        public Role (string roleName)
        {
            RoleName = roleName;
        }
    }
}
