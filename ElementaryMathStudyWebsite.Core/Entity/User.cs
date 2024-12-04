using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Core.Repositories.Entity
{
    public class User : BaseEntity
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty; // Using string.empty to avoid null reference issues
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string Gender { get; set; } = "Male"; // Male, Female, Others

        public string? Email { get; set; }

        public string RoleId { get; set; } = string.Empty;

        public string? VerificationToken { get; set; }

        public string? PasswordResetToken { get; set; }

        public DateTimeOffset? ResetTokenExpiry { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        // Navigation Properties
        public virtual Role? Role { get; set; } // Navigation property, one user has one role
        public virtual ICollection<Order>? Orders { get; set; } // Navigation property, one user can create many orders
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } // Navigation property, one user can be assigned to many order detail
        public virtual ICollection<UserAnswer>? Answers { get; set; } // Navigation property, one user has many answers
        public virtual ICollection<Progress>? Progresses { get; set; } // Navigation property, one user has many subject progresses
        public virtual ICollection<Result>? Results { get; set; } // Navigation property, one user has many quiz results

    }
}
