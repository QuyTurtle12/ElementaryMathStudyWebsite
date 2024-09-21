using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Core.Store
{
    public enum PaymentStatusHelper
    {
        [CustomName("Success")]
        Success,

        [CustomName("Failed")]
        Failed,

        [CustomName("Pending")]
        Pending,

    }
}
