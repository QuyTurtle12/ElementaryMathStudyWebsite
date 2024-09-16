namespace ElementaryMathStudyWebsite.Contract.UseCases.DTOs
{
    public interface ISubjectBaseDTO
    {
        string SubjectName { get; set; }
        double? Price { get; set; }
        bool Status { get; set; }
    }
}
