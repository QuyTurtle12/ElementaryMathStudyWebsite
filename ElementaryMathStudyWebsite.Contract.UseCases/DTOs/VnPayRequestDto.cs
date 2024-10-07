namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class VnPayRequestDto
    {
        public int OrderId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset ExpireDate { get; set; }
    }
}
