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
        private readonly IAppUserServices _userService;
        private readonly IAppOptionServices _optionService;
        public QuestionService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppOptionServices optionService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _optionService = optionService;
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
                    CreatedTime = question.CreatedTime,

                    LastUpdatedBy = question.LastUpdatedBy ?? string.Empty,
                    LastUpdatedPersonName = question.LastUpdatedByUser?.FullName ?? string.Empty,
                    LastUpdatedPersonPhone = question.LastUpdatedByUser?.PhoneNumber ?? string.Empty,
                    LastUpdatedTime = question.LastUpdatedTime
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
            QuestionMainViewDto dto = new QuestionMainViewDto
            {
                Id = question.Id,
                QuestionContext = question.QuestionContext,
                QuizName = question.Quiz?.QuizName ?? string.Empty,
                QuizId = question.QuizId,

                CreatedBy = question.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                CreatedTime = question.CreatedTime,

                LastUpdatedBy = question.LastUpdatedBy ?? string.Empty,
                LastUpdatedPersonName = question.LastUpdatedByUser?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = question.LastUpdatedByUser?.PhoneNumber ?? string.Empty,
                LastUpdatedTime = question.LastUpdatedTime
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
                .Include(q => q.Quiz)
                .Include(q => q.CreatedByUser)
                .Include(q => q.LastUpdatedByUser);

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
                    CreatedTime = question.CreatedTime,
                    LastUpdatedBy = question.LastUpdatedBy ?? string.Empty,
                    LastUpdatedPersonName = question.LastUpdatedByUser?.FullName ?? string.Empty,
                    LastUpdatedPersonPhone = question.LastUpdatedByUser?.PhoneNumber ?? string.Empty,
                    LastUpdatedTime = question.LastUpdatedTime
                };

                questionDtos.Add(dto);
            }

            // Return paginated list of QuestionMainViewDto
            return new BasePaginatedList<QuestionMainViewDto>(questionDtos, totalCount, pageNumber, pageSize);
        }

        // Method to add a new question
        public async Task<QuestionMainViewDto> AddQuestionAsync(QuestionCreateDto dto)
        {
            // Create new question entity
            var question = new Question
            {
                QuestionContext = dto.QuestionContext,
                QuizId = dto.QuizId,
                CreatedBy = (await _userService.GetCurrentUserAsync()).Id.ToUpper(),
                CreatedTime = DateTime.UtcNow
            };

            // Add the question to the repository
            await _unitOfWork.GetRepository<Question>().InsertAsync(question);
            await _unitOfWork.SaveAsync();

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();
            question.LastUpdatedBy = currentUser.Id ?? string.Empty; // Update LastUpdatedBy

            // Fetch creator and last updated person information
            var creator = await _unitOfWork.GetRepository<User>().FindByConditionAsync(c => c.Id != null && c.Id.Equals(question.CreatedBy));
            var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.LastUpdatedBy);

            // Return the created question information in a DTO
            return new QuestionMainViewDto
            {
                Id = question.Id,
                QuestionContext = question.QuestionContext,
                QuizName = question.Quiz?.QuizName ?? string.Empty,
                QuizId = question.QuizId,

                CreatedBy = question.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                CreatedTime = question.CreatedTime,

                LastUpdatedBy = question.LastUpdatedBy ?? string.Empty,
                LastUpdatedPersonName = question.LastUpdatedByUser?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = question.LastUpdatedByUser?.PhoneNumber ?? string.Empty,
                LastUpdatedTime = question.LastUpdatedTime
            };
        }

        // Method to update an existing question
        public async Task<QuestionMainViewDto> UpdateQuestionAsync(string id, QuestionUpdateDto dto)
        {
            // Fetch the existing question by its ID
            var question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(id)
                            ?? throw new BaseException.NotFoundException("not_found", $"Question with Id '{id}' not found.");

            // Update question information with values from the DTO
            question.QuestionContext = dto.QuestionContext;
            question.QuizId = dto.QuizId;

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();
            question.LastUpdatedBy = currentUser.Id ?? string.Empty; // Update LastUpdatedBy

            // Fetch creator and last updated person information
            var creator = await _unitOfWork.GetRepository<User>().FindByConditionAsync(c => c.Id != null && c.Id.Equals(question.CreatedBy));
            var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.LastUpdatedBy);

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Return the updated question information in a DTO
            return new QuestionMainViewDto
            {
                Id = question.Id,
                QuestionContext = question.QuestionContext,
                QuizName = question.Quiz?.QuizName ?? string.Empty,
                QuizId = question.QuizId,

                CreatedBy = question.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                CreatedTime = question.CreatedTime,

                LastUpdatedBy = question.LastUpdatedBy ?? string.Empty,
                LastUpdatedPersonName = question.LastUpdatedByUser?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = question.LastUpdatedByUser?.PhoneNumber ?? string.Empty,
                LastUpdatedTime = question.LastUpdatedTime
            };
        }

        public async Task<bool> DeleteQuestion(string questionId)
        {
            Question? question;

            if (_unitOfWork.IsValid<Question>(questionId))
                question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(questionId);
            else throw new BaseException.NotFoundException("not_found", "Question ID not found");

            _userService.AuditFields(question!, false, true);

            await _unitOfWork.SaveAsync();

            IQueryable<Option> query = _unitOfWork.GetRepository<Option>().GetEntitiesWithCondition(
                            o => o.QuestionId == questionId &&
                            string.IsNullOrWhiteSpace(o.DeletedBy)
                            );

            foreach (var option in query)
            {
                await _optionService.DeleteOption(option.Id);
            }

            return true;
        }
    }
}
