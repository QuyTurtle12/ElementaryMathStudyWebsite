namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto
{
    public class OrderCreateDto
    {
        public string CustomerName { get; set; } = string.Empty;

        public double? TotalPrice { get; set; }
    }
}
