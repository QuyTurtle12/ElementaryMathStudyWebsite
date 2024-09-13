namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class OrderViewDto
    {
        public string CustomerName { get; set; } = string.Empty;

        public double? TotalPrice { get; set; }

        public DateTimeOffset? OrderDate { get; set; }

        public OrderViewDto() { }

        public OrderViewDto(string customerName, double? totalPrice, DateTimeOffset? orderDate)
        {
            CustomerName = customerName;
            TotalPrice = totalPrice;
            OrderDate = orderDate;
        }
    }
}
