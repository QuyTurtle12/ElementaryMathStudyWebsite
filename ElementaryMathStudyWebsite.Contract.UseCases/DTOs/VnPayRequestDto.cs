﻿namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public class VnPayRequestDto
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}