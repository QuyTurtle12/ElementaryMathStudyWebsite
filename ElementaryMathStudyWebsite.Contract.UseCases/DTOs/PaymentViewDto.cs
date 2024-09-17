namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class PaymentViewDto
    {
        public string? PaymentId { get; set; }
        public string? CustomerName { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public List<PaymentSubjectStudent> SubjectStudents { get; set; } = new();

    }

    public class PaymentSubjectStudent
    {
        public string SubjectName { get; set; }
        public string StudentName { get; set; }
    }
}
