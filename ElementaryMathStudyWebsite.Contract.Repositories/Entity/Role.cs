using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual User? LastUpdatedByUser { get; set; }
        public virtual User? DeletedByUser { get; set; }
        public virtual ICollection<User>? Users { get; set; } // Navigation property, one role has many users

        public Role () { }

        public Role (string roleName)
        {
            RoleName = roleName;
        }
    }
}
