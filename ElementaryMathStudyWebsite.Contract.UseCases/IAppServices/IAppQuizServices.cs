namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppQuizServices
    {
        // Get quiz name by quiz id
        Task<string> GetQuizNameAsync(string quizId);
    }
}
