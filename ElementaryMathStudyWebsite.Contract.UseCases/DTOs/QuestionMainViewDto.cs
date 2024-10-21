
namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class QuestionMainViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string QuestionContext { get; set; } = string.Empty;

        public string QuizName { get; set; } = string.Empty;
        public string QuizId { get; set; } = string.Empty;

        public DateTimeOffset createdTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string CreatorPhone { get; set; } = string.Empty;

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedPersonName { get; set; } = string.Empty;
        public string LastUpdatedPersonPhone { get; set; } = string.Empty;  

    }
}
