using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuizService : IQuizService, IAppQuizServices
    {
        private readonly IGenericRepository<Quiz> _quizRepository;

        // constructor
        public QuizService (IGenericRepository<Quiz> QuizRepository)
        {
            _quizRepository = QuizRepository;
        }

        public Task<BasePaginatedList<QuizViewDto?>> GetQuizzDtoAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Quiz>> GetQuizzesAsync()
        {
            // Lấy tất cả các Quiz từ repository
            IQueryable<Quiz> query = _quizRepository.Entities;

            // Chuyển đổi thành danh sách để làm việc với dữ liệu
            var quizzesList = await query.ToListAsync();

            // Trả về danh sách các Quiz
            return quizzesList;
        }

        public async Task<IList<QuizViewDto>> GetQuizViewDtosAsync()
        {
            // Lấy tất cả các Quiz từ repository
            IQueryable<Quiz> query = _quizRepository.Entities;

            // Chuyển đổi thành danh sách để làm việc với dữ liệu
            var quizzesList = await query.ToListAsync();

            // Chuyển đổi từ Quiz sang QuizViewDto
            var quizViewDtos = quizzesList.Select(q => new QuizViewDto
            {
                QuizName = q.QuizName,
                Criteria = q.Criteria,
                Status = q.Status,
                CreatedByUserId = q.CreatedByUserId,
                LastUpdatedByUserId = q.LastUpdatedByUserId,
                DeletedByUserId = q.DeletedByUserId,
                ChapterId = q.ChapterId,
                TopicId = q.TopicId,
                QuestionIds = q.QuestionIds,
                ProgressIds = q.ProgressIds
            }).ToList();

            // Trả về danh sách các QuizViewDto
            return quizViewDtos;
        }
    }
}
