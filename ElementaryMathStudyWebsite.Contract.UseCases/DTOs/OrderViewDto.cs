namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderViewDto
    {
        public string OrderId { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public double TotalPrice { get; set; } = 0;

        public DateTimeOffset? OrderDate { get; set; }

        public IEnumerable<OrderDetailViewDto>? details {  get; set; } 

    }
}
