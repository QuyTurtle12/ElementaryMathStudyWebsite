using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Core.Store
{
    public enum RoleHelper
    {
        [CustomName("Student")]
        Student,

        [CustomName("Parent")]
        Parent,

        [CustomName("Admin")]
        Admin,

        [CustomName("Manager")]
        Manager,

        [CustomName("Content Manager")]
        Content,
    }
}
