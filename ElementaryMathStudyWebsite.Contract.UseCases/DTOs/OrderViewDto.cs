namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderViewDto
    {
        private DateTimeOffset createdTime;

        public OrderViewDto(string? customerName, double totalPrice, DateTimeOffset createdTime)
        {
            CustomerName = customerName;
            TotalPrice = totalPrice;
            this.createdTime = createdTime;
        }

        public string CustomerName { get; set; } = string.Empty;

        public double? TotalPrice { get; set; }

        public DateTimeOffset? OrderDate { get; set; }

    }
}
