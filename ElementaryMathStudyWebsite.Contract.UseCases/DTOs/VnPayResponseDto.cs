namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class VnPayResponseDto
    {
        public bool Success { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string VnPayResponseCode { get; set; } = string.Empty;
    }
}
