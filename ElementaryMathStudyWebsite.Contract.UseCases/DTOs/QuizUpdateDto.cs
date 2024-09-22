namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class UpdateQuizDto
    {
        public string Id { get; set; } = string.Empty; 
        public string QuizName { get; set; } = string.Empty;
        public double? Criteria { get; set; } 
        public bool? Status { get; set; } 
        public required string ChapterId { get; set; }
        public required string TopicId { get; set; } 
    }

}
