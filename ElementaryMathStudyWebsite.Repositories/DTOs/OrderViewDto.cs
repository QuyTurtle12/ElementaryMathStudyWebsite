namespace ElementaryMathStudyWebsite.Repositories.DTOs
{
    public class OrderViewDto
    {
        public string CustomerName { get; set; } = string.Empty;

        public double? TotalPrice { get; set; }

        public DateTimeOffset? OrderDate { get; set; }
    }
}
