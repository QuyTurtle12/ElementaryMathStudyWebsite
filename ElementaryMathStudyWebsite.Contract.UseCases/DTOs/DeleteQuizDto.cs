namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class DeleteQuizDto
    {
        public required string Id { get; set; }
        public string QuizName { get; set; } = string.Empty;

        public string CreatorName { get; set; } = string.Empty;
        public required string DeletedBy { get; set; }
        public string DeletedName { get; set; } = string.Empty;
        public DateTimeOffset DeletedTime { get; set; }
        public string LastUpdatedPersonName { get; set; } = string.Empty;
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}