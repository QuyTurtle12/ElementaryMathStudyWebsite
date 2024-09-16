using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuizService : IQuizService, IAppQuizServices
    {
        private readonly IGenericRepository<Quiz> _quizRepository;

        public QuizService(IGenericRepository<Quiz> quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public async Task<string> GetQuizNameAsync(string quizId)
        {
            Quiz quiz = await _quizRepository.GetByIdAsync(quizId);
            return quiz?.QuizName ?? string.Empty;
        }
    }
}
