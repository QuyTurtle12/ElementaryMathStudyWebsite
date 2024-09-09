using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Repositories.Entity
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty; // Using string.empty to avoid null reference issues
    }
}
