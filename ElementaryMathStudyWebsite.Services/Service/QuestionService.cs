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

        // Get all questions exist

        public async Task<List<QuestionMainViewDto>> GetAllQuestionsMainViewDtoAsync()
        {
            // Retrieve the list of questions that are not deleted
            List<Question> questions = await _unitOfWork.GetRepository<Question>()
                .GetEntitiesWithCondition(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();

            // Check if any questions were found
            if (questions == null || !questions.Any())
            {
                throw new BaseException.NotFoundException("not_found", "No questions found.");
            }

            // Get distinct IDs of users who created and last updated the questions
            List<string?> createdByIds = questions.Select(q => q.CreatedBy).Distinct().ToList();
            List<string?> lastUpdatedByIds = questions.Select(q => q.LastUpdatedBy).Distinct().ToList();

            // Retrieve user information based on the IDs
            List<User> createdUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => createdByIds.Contains(u.Id))
                .ToListAsync();

            List<User> updatedUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => lastUpdatedByIds.Contains(u.Id))
                .ToListAsync();

            // Optional: Check if users were found
            if (createdUsers == null || updatedUsers == null)
            {
                throw new BaseException.NotFoundException("not_found", "Some user information could not be found.");
            }

            // Use AutoMapper to map the Question entities to DTOs
            List<QuestionMainViewDto> questionDtos = _mapper.Map<List<QuestionMainViewDto>>(questions, opt =>
            {
                opt.Items["CreatedUsers"] = createdUsers;
                opt.Items["UpdatedUsers"] = updatedUsers;
            });

            return questionDtos;
        }

        // Get question by its id
        public async Task<QuestionMainViewDto> GetQuestionByIdAsync(string questionId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(questionId))
            {
                throw new BaseException.BadRequestException("invalid_question_id", "Question ID cannot be null or empty.");
            }

            // Fetch the question by its Id along with related users
            Question? question = await _unitOfWork.GetRepository<Question>().Entities
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == questionId && string.IsNullOrWhiteSpace(q.DeletedBy));

            // Check if the question exists
            if (question == null)
            {
                throw new BaseException.NotFoundException("not_found", $"Question ID {questionId} not found");
            }

            // Fetch user information for created and last updated users if they exist
            User? createdUser = null;
            if (!string.IsNullOrWhiteSpace(question.CreatedBy))
            {
                createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.CreatedBy);
            }

            User? lastUpdatedUser = null;
            if (!string.IsNullOrWhiteSpace(question.LastUpdatedBy))
            {
                lastUpdatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(question.LastUpdatedBy);
            }

            // Map question to QuestionMainViewDto
            QuestionMainViewDto dto = _mapper.Map<QuestionMainViewDto>(question, opt =>
            {
                // Include created and last updated user information
                opt.Items["CreatedUser"] = createdUser;
                opt.Items["LastUpdatedUser"] = lastUpdatedUser;
            });

            return dto; // Return the QuestionMainViewDto
        }

        // Search for questions where the question context contains a specified string
        public async Task<List<QuestionViewDto>> SearchQuestionsByContextAsync(string questionContext)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(questionContext))
            {
                throw new BaseException.NotFoundException("not_found", "Search term cannot be null or empty.");
            }

            // Fetch questions that match the question context
            List<Question> questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuestionContext.Contains(questionContext) && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();


            // Check if any questions were found
            if (questions == null || !questions.Any())
            {
                throw new BaseException.NotFoundException("not_found", "No questions found matching the specified context.");
            }

            // Use AutoMapper to map questions to QuestionViewDto using a separate mapping method
            List<QuestionViewDto> questionDtos = _mapper.Map<List<QuestionViewDto>>(questions);

            return questionDtos;
        }

        // Get all questions that belong to a specific quiz (by quizId)
        public async Task<List<QuestionViewDto>> GetQuestionsByQuizIdAsync(string quizId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(quizId))
            {
                throw new BaseException.BadRequestException("invalid_quiz_id", "Quiz ID cannot be null or empty.");
            }

            // Fetch questions that belong to the specified quiz
            List<Question> questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuizId == quizId && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz)
                .ToListAsync();

            // Check if any questions were found
            if (questions == null || !questions.Any())
            {
                throw new BaseException.NotFoundException("not_found", $"No questions found for Quiz ID {quizId}.");
            }

            // Use AutoMapper to map questions to QuestionViewDto using a separate mapping method
            List<QuestionViewDto> questionDtos = _mapper.Map<List<QuestionViewDto>>(questions);

            return questionDtos;
        }

        // Get questions with pagination
        public async Task<BasePaginatedList<QuestionViewDto>> GetQuestionsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                throw new BaseException.BadRequestException("invalid_page_number", "Page number must be greater than 0.");
            }

            if (pageSize <= 0)
            {
                throw new BaseException.BadRequestException("invalid_page_size", "Page size must be greater than 0.");
            }

            // Query all questions excluding deleted ones
            IQueryable<Question> query = _unitOfWork.GetRepository<Question>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz);

            // Get the total count of questions for pagination
            int totalQuestionsCount = await query.CountAsync();

            // Fetch the paginated questions
            List<Question> paginatedQuestions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Check if there are any results after pagination
            if (!paginatedQuestions.Any())
            {
                throw new BaseException.NotFoundException("not_found", "No questions found for the given page.");
            }

            // Use AutoMapper to map questions to QuestionViewDto
            List<QuestionViewDto> questionDtos = _mapper.Map<List<QuestionViewDto>>(paginatedQuestions);

            // Return paginated results
            return new BasePaginatedList<QuestionViewDto>(questionDtos, totalQuestionsCount, pageNumber, pageSize);
        }

        //=============================================================================================================

        // Method to add one or more questions
        public async Task<BaseResponse<string>> AddQuestionAsync(List<QuestionCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Question list cannot be null or empty.");
            }

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();

            // Validate all questions and prepare question entities using LINQ Select
            var questions = await Task.WhenAll(dtos.Select(async dto =>
            {
                // Validate QuestionContext and QuizId
                if (string.IsNullOrWhiteSpace(dto.QuestionContext) || string.IsNullOrWhiteSpace(dto.QuizId))
                {
                    throw new BaseException.BadRequestException("invalid_arguments", "Question context or quiz id cannot be null or empty.");
                }

                // Check if the quiz exists
                Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(dto.QuizId);
                if (quiz == null)
                {
                    throw new BaseException.NotFoundException("not_found", $"Quiz ID {dto.QuizId} not found.");
                }

                // Return a new Question entity
                return new Question
                {
                    Id = Guid.NewGuid().ToString().ToUpper(),
                    QuestionContext = dto.QuestionContext,
                    QuizId = dto.QuizId,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow,
                    CreatedBy = currentUser.Id.ToUpper(), // Set CreatedBy
                    LastUpdatedBy = currentUser.Id.ToUpper() // Set LastUpdatedBy to the same user
                };
            }));

            // Insert each question using LINQ Select
            await Task.WhenAll(questions.Select(async question =>
                await _unitOfWork.GetRepository<Question>().InsertAsync(question)
            ));

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            return BaseResponse<string>.OkResponse($"{dtos.Count} question(s) created successfully.");
        }

        // Method to update an existing question
        public async Task<QuestionMainViewDto> UpdateQuestionAsync(string id, QuestionUpdateDto dto)
        {
            // Fetch the existing question by its ID
            Question question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(id)
                                ?? throw new BaseException.NotFoundException("not_found", $"Question with Id '{id}' not found.");

            // Validate DTO
            if (dto == null || string.IsNullOrWhiteSpace(dto.QuestionContext))
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Question context cannot be null or empty.");
            }

            // If QuizId is null or empty, retain the existing QuizId
            if (!string.IsNullOrWhiteSpace(dto.QuizId))
            {
                question.QuizId = dto.QuizId;
            }

            // Update question context
            question.QuestionContext = dto.QuestionContext;

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();

            // Update LastUpdatedBy and LastUpdatedTime
            question.LastUpdatedBy = currentUser.Id ?? string.Empty;
            question.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Fetch users who created and last updated the question
            User? createdByUser = await _unitOfWork.GetRepository<User>()
                                .GetEntitiesWithCondition(u => u.Id == question.CreatedBy)
                                .FirstOrDefaultAsync();

            User? lastUpdatedByUser = await _unitOfWork.GetRepository<User>()
                                .GetEntitiesWithCondition(u => u.Id == question.LastUpdatedBy)
                                .FirstOrDefaultAsync();

            // Map the updated question to QuestionMainViewDto using AutoMapper
            QuestionMainViewDto questionDto = _mapper.Map<QuestionMainViewDto>(question, opt =>
            {
                opt.Items["CreatedUser"] = createdByUser;
                opt.Items["LastUpdatedUser"] = lastUpdatedByUser;
            });

            // Return the updated question DTO
            return questionDto;
        }

        // Method delete question by id
        public async Task<BaseResponse<string>> DeleteQuestionAsync(string questionId)
        {
            // validate questionid
            if (string.IsNullOrWhiteSpace(questionId))
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Question ID cannot be null or empty.");
            }

            // Check if the question exists
            Question? question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(questionId);
            if (question == null)
            {
                throw new BaseException.NotFoundException("not_found", $"Question with ID '{questionId}' not found.");
            }

            // Mark the question as deleted
            _userService.AuditFields(question, false, true);
            await _unitOfWork.SaveAsync();

            // Fetch and soft delete related options
            List<Option> options = await _unitOfWork.GetRepository<Option>()
                .GetEntitiesWithCondition(o => o.QuestionId == questionId && string.IsNullOrWhiteSpace(o.DeletedBy))
                .ToListAsync();

            foreach (var option in options)
            {
                await _optionService.DeleteOption(option.Id);
            }

            return BaseResponse<string>.OkResponse("Question deleted successfully.");
        }
    }
}