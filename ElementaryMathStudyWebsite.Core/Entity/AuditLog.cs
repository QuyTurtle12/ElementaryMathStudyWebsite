using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Core.Entity
{
    public class AuditLog : BaseEntity
    {
        public string? Form { get; set; }
        public string? Headers { get; set; }
        public string? HttpURL { get; set; }
        public string? LocalAddress { get; set; }
        public string? RemoteHost { get; set; }
        public string? ResponseBody { get; set; }
        public string? ResponseStatusCode { get; set; }
        public string? ResQuestBody { get; set; }
        public string? Claims { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
