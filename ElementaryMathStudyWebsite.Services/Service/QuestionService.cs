using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuestionService : IAppQuestionServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Get questions with all properties
        public async Task<List<QuestionMainViewDto>> GetAllQuestionsMainViewDtoAsync()
        {
            // Query all questions from the repository, including related entities
            var questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .Include(q => q.CreatedByUser)
                .Include(q => q.LastUpdatedByUser)
                .ToListAsync();

            // List to hold QuestionMainViewDto
            IList<QuestionMainViewDto> questionDtos = new List<QuestionMainViewDto>();

            foreach (var question in questions)
            {
                // Get creator and last updater information
                var creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.CreatedBy ?? string.Empty);
                var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.LastUpdatedBy ?? string.Empty);

                // Create QuestionMainViewDto
                QuestionMainViewDto dto = new QuestionMainViewDto
                {
                    Id = question.Id,
                    QuestionContext = question.QuestionContext,
                    QuizName = question.Quiz?.QuizName ?? string.Empty,
                    QuizId = question.QuizId,
                    CreatedBy = question.CreatedBy ?? string.Empty,
                    CreatorName = creator?.FullName ?? string.Empty,
                    CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                    CreatedTime = question.CreatedTime, // No null check needed
                    LastUpdatedTime = question.LastUpdatedTime // No null check needed
                };

                questionDtos.Add(dto);
            }

            return questionDtos.ToList(); // Return the list of QuestionMainViewDto
        }

        // Get questions by Id question
        public async Task<QuestionMainViewDto?> GetQuestionByIdAsync(string questionId)
        {
            // Fetch the question by its Id
            var question = await _unitOfWork.GetRepository<Question>().Entities
                .Include(q => q.Quiz)
                .Include(q => q.CreatedByUser)
                .Include(q => q.LastUpdatedByUser)
                .FirstOrDefaultAsync(q => q.Id == questionId && string.IsNullOrWhiteSpace(q.DeletedBy));

            // Check if question exists
            if (question == null)
            {
                return null; // Or throw an exception based on your preference
            }

            // Get creator and last updater information
            var creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.CreatedBy ?? string.Empty);
            var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.LastUpdatedBy ?? string.Empty);

            // Create QuestionMainViewDto
            var dto = new QuestionMainViewDto
            {
                Id = question.Id,
                QuestionContext = question.QuestionContext,
                QuizName = question.Quiz?.QuizName ?? string.Empty,
                QuizId = question.QuizId,
                CreatedBy = question.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                CreatedTime = question.CreatedTime, // No null check needed
                LastUpdatedTime = question.LastUpdatedTime // No null check needed
            };

            return dto; // Return the QuestionMainViewDto
        }

        // Search for questions where the question context contains a specified string
        public async Task<List<QuestionViewDto>> SearchQuestionsByContextAsync(string questionContext)
        {
            // Fetch questions that match the question context
            var questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuestionContext.Contains(questionContext) && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();

            var questionDtos = questions.Select(q => new QuestionViewDto
            {
                Id = q.Id,
                QuestionContext = q.QuestionContext,
                QuizName = q.Quiz?.QuizName ?? string.Empty, 
                QuizId = q.Quiz?.Id ?? string.Empty 
            }).ToList();

            return questionDtos;
        }

        // Get all questions that belong to a specific quiz (by quizId)
        public async Task<List<QuestionViewDto>> GetQuestionsByQuizIdAsync(string quizId)
        {
            // Fetch questions that belong to the specified quiz
            var questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuizId == quizId && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();

            var questionDtos = questions.Select(q => new QuestionViewDto
            {
                Id = q.Id,
                QuestionContext = q.QuestionContext,
                QuizId = q.QuizId,
                QuizName = q.Quiz?.QuizName ?? string.Empty
            }).ToList();

            return questionDtos;
        }

        // Get all questions with pagination and full properties
        public async Task<BasePaginatedList<QuestionMainViewDto>?> GetQuestionsAsync(int pageNumber, int pageSize)
        {
            IQueryable<Question> query = _unitOfWork.GetRepository<Question>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz) // Include related Quiz data
                .Include(q => q.CreatedByUser) // Include creator info
                .Include(q => q.LastUpdatedByUser); // Include last updater info

            // Retrieve total count for pagination
            var totalCount = await query.CountAsync();

            // Paginate the query results
            var questions = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // List to hold QuestionMainViewDto
            List<QuestionMainViewDto> questionDtos = new List<QuestionMainViewDto>();

            foreach (var question in questions)
            {
                // Get creator and last updater information
                var creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.CreatedBy ?? string.Empty);
                var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.LastUpdatedBy ?? string.Empty);

                // Create QuestionMainViewDto
                QuestionMainViewDto dto = new QuestionMainViewDto
                {
                    Id = question.Id,
                    QuestionContext = question.QuestionContext,
                    QuizName = question.Quiz?.QuizName ?? string.Empty,
                    QuizId = question.QuizId,
                    CreatedBy = question.CreatedBy ?? string.Empty,
                    CreatorName = creator?.FullName ?? string.Empty,
                    CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                    CreatedTime = question.CreatedTime, // No null check needed
                    LastUpdatedTime = question.LastUpdatedTime // No null check needed
                };

                questionDtos.Add(dto);
            }

            // Return paginated list of QuestionMainViewDto
            return new BasePaginatedList<QuestionMainViewDto>(questionDtos, totalCount, pageNumber, pageSize);
        }


    }
}
