using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
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

        // Get all questions with full entities, including audit fields and related Quiz
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            // Query all questions from the repository, including related entities
            var questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz) 
                .Include(q => q.CreatedByUser) 
                .Include(q => q.LastUpdatedByUser)
                .ToListAsync();

            // Return the full list of Question entities
            return questions;
        }

        //// Get all questions
        //public async Task<List<QuestionViewDto>> GetAllQuestionsAsync()
        //{
        //    var questions = await _unitOfWork.GetRepository<Question>().Entities
        //        .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
        //        .Include(q => q.Quiz) // Include related Quiz data
        //        .ToListAsync();

        //    return questions.Select(q => new QuestionViewDto
        //    {
        //        Id = q.Id,
        //        QuestionContext = q.QuestionContext,
        //        QuizName = q.Quiz?.QuizName ?? string.Empty,  // Include related quiz name
        //    }).ToList();
        //}



        // Get Question with questionId
        public async Task<QuestionViewDto?> GetQuestionByIdAsync(string questionId)
        {
            // Fetch the question by its Id
            var question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(questionId);

            // Check if question exists
            if (question == null || !string.IsNullOrWhiteSpace(question.DeletedBy))
            {
                return null;
            }
            var creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.CreatedBy ?? string.Empty);
            var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.LastUpdatedBy ?? string.Empty);

            var dto = new QuestionViewDto
            {
                Id = question.Id,
                QuestionContext = question.QuestionContext,
                QuizName = question.Quiz?.QuizName ?? string.Empty,
            };

            return dto; // Return the QuestionViewDto
        }

        // Search for questions where the question context contains a specified string
        public async Task<List<QuestionViewDto>> SearchQuestionsByContextAsync(string questionContext)
        {
            // Fetch questions that match the question context
            var questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuestionContext.Contains(questionContext) && string.IsNullOrWhiteSpace(q.DeletedBy))
                .ToListAsync();

            var questionDtos = questions.Select(q => new QuestionViewDto
            {
                Id = q.Id,
                QuestionContext = q.QuestionContext,
                QuizName = q.Quiz?.QuizName ?? string.Empty,  // Include related quiz name
            }).ToList();

            return questionDtos; // Return the list of QuestionViewDto
        }

        // Get all questions that belong to a specific quiz (by quizId)
        public async Task<List<QuestionViewDto>> GetQuestionsByQuizIdAsync(string quizId)
        {
            var questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.Quiz != null && q.Quiz.Id == quizId && string.IsNullOrWhiteSpace(q.DeletedBy))
                .ToListAsync();

            // Convert Question entities to QuestionViewDto
            var questionDtos = questions.Select(q => new QuestionViewDto
            {
                Id = q.Id,
                QuestionContext = q.QuestionContext,
                QuizName = q.Quiz?.QuizName ?? string.Empty,  // Include related quiz name
            }).ToList();

            return questionDtos;
        }

        // Get all questions with pagination and full properties
        public async Task<BasePaginatedList<QuestionViewDto>?> GetQuestionsAsync(int pageNumber, int pageSize)
        {
            // Query all questions from the database
            IQueryable<Question> query = _unitOfWork.GetRepository<Question>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy));

            // Retrieve all questions from the database
            var allQuestions = await query.ToListAsync();

            // List to hold question view DTOs
            List<QuestionViewDto> questionDtos = new List<QuestionViewDto>();

            foreach (var question in allQuestions)
            {
                // Create a new QuestionViewDto and map properties
                QuestionViewDto dto = new QuestionViewDto
                {
                    Id = question.Id,
                    QuestionContext = question.QuestionContext,
                    QuizName = question.Quiz?.QuizName ?? string.Empty,
                };
                questionDtos.Add(dto); // Add the DTO to the list
            }

            // If pageNumber or pageSize are 0 or negative, return all questions without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<QuestionViewDto>(questionDtos, questionDtos.Count, 1, questionDtos.Count);
            }

            // Paginate the list of questions
            var paginatedQuestionsDto = questionDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new BasePaginatedList<QuestionViewDto>(paginatedQuestionsDto, questionDtos.Count, pageNumber, pageSize);
        }
    }
}
