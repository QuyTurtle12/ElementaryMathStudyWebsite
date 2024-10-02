using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;


namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuestionService : IAppQuestionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppOptionServices _optionService;
        private readonly IMapper _mapper;

        public QuestionService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppOptionServices optionService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _optionService = optionService;
            _mapper = mapper;
        }

        // Get questions with all properties
        public async Task<List<QuestionMainViewDto>> GetAllQuestionsMainViewDtoAsync()
        {
            // Query all questions from the repository, including related entities
            List<Question> questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .Include(q => q.CreatedByUser)
                .Include(q => q.LastUpdatedByUser)
                .ToListAsync();

            // Map the list of questions to a list of QuestionMainViewDto using AutoMapper
            List<QuestionMainViewDto> questionDtos = _mapper.Map<List<QuestionMainViewDto>>(questions);

            return questionDtos; // Return the list of QuestionMainViewDto
        }

        // Get questions by Id question
        public async Task<QuestionMainViewDto> GetQuestionByIdAsync(string questionId)
        {
            // Fetch the question by its Id
            Question? question = await _unitOfWork.GetRepository<Question>().Entities
                .Include(q => q.Quiz)
                .Include(q => q.CreatedByUser)
                .Include(q => q.LastUpdatedByUser)
                .FirstOrDefaultAsync(q => q.Id == questionId && string.IsNullOrWhiteSpace(q.DeletedBy));

            // Check if question exists
            if (question == null)
            {
                throw new BaseException.NotFoundException("not_found", $"Question ID {questionId} not found");
            }

            // Use AutoMapper to map to QuestionMainViewDto
            QuestionMainViewDto dto = _mapper.Map<QuestionMainViewDto>(question);

            return dto; // Return the QuestionMainViewDto
        }

        // Search for questions where the question context contains a specified string
        public async Task<List<QuestionViewDto>> SearchQuestionsByContextAsync(string questionContext)
        {
            // Fetch questions that match the question context
            List<Question> questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuestionContext.Contains(questionContext) && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();

            // Use AutoMapper to map questions to QuestionViewDto
            List<QuestionViewDto> questionDtos = _mapper.Map<List<QuestionViewDto>>(questions);

            return questionDtos;
        }

        // Get all questions that belong to a specific quiz (by quizId)
        public async Task<List<QuestionViewDto>> GetQuestionsByQuizIdAsync(string quizId)
        {
            // Fetch questions that belong to the specified quiz
            List<Question> questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuizId == quizId && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();

            // Use AutoMapper to map questions to QuestionViewDto
            List<QuestionViewDto> questionDtos = _mapper.Map<List<QuestionViewDto>>(questions);

            return questionDtos;
        }

        // Get questions with pagination
        public async Task<BasePaginatedList<QuestionViewDto>> GetQuestionsAsync(int pageNumber, int pageSize)
        {
            // Query all questions excluding deleted ones
            List<Question> allQuestions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();

            // Use AutoMapper to map questions to QuestionMainViewDto
            List<QuestionViewDto> questionDtos = _mapper.Map<List<QuestionViewDto>>(allQuestions);

            // If pageNumber or pageSize are 0 or negative, return all questions without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<QuestionViewDto>(questionDtos, questionDtos.Count, 1, questionDtos.Count);
            }

            // Paginate the list of questions
            List<QuestionViewDto> paginatedQuestionsDto = questionDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new BasePaginatedList<QuestionViewDto>(paginatedQuestionsDto, questionDtos.Count, pageNumber, pageSize);
        }


        // Method to add a new question
        public async Task<QuestionMainViewDto> AddQuestionAsync(QuestionCreateDto dto)
        {
            // Validate input DTO
            if (dto == null)
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Question data cannot be null.");
            }

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();

            // Create a new Question entity
            Question question = new()
            {
                Id = Guid.NewGuid().ToString(),
                QuestionContext = dto.QuestionContext,
                QuizId = dto.QuizId,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                CreatedBy = currentUser.Id.ToUpper(), // Set CreatedBy
                LastUpdatedBy = currentUser.Id.ToUpper() // Set LastUpdatedBy to the same user
            };

            // Insert the new question into the repository
            await _unitOfWork.GetRepository<Question>().InsertAsync(question);
            await _unitOfWork.SaveAsync();

            // Map the newly created question to QuestionMainViewDto using AutoMapper
            QuestionMainViewDto questionDto = _mapper.Map<QuestionMainViewDto>(question);

            // Return the created question DTO
            return questionDto;
        }

        // Method to update an existing question
        public async Task<QuestionMainViewDto> UpdateQuestionAsync(string id, QuestionUpdateDto dto)
        {
            // Fetch the existing question by its ID
            Question question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(id)
                                ?? throw new BaseException.NotFoundException("not_found", $"Question with Id '{id}' not found.");

            // Update question information with values from the DTO
            question.QuestionContext = dto.QuestionContext;
            question.QuizId = dto.QuizId;

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();

            // Update LastUpdatedBy and LastUpdatedTime
            question.LastUpdatedBy = currentUser.Id ?? string.Empty; // Update LastUpdatedBy
            question.LastUpdatedTime = CoreHelper.SystemTimeNow; // Update LastUpdatedTime

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Map the updated question to QuestionMainViewDto using AutoMapper
            QuestionMainViewDto questionDto = _mapper.Map<QuestionMainViewDto>(question);

            // Return the updated question information in a DTO
            return questionDto;
        }

        public async Task<BaseResponse<string>> DeleteQuestion(string questionId)
        {
            Question? question;

            if (_unitOfWork.IsValid<Question>(questionId))
                question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(questionId);
            else
                throw new BaseException.NotFoundException("not_found", "Question ID not found");

            // Audit the quiz to mark it as deleted
            _userService.AuditFields(question!, false, true);
            await _unitOfWork.SaveAsync();


            // Fetch all related questions that are not already deleted
            IQueryable<Option> query = _unitOfWork.GetRepository<Option>().GetEntitiesWithCondition(
                            o => o.QuestionId == questionId &&
                            string.IsNullOrWhiteSpace(o.DeletedBy)
                            );

            foreach (var option in query)
            {
                await _optionService.DeleteOption(option.Id);
            }

            return BaseResponse<string>.OkResponse("Question deleted successfully.");
        }
    }
}