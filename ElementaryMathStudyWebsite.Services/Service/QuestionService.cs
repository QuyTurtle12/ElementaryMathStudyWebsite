using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


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

            // Retrieve the list of questions that are not deleted
            Question? question = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.Id == questionId)
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync();

            // Check if quiz exists or has been deleted
            if (question == null || !string.IsNullOrWhiteSpace(question.DeletedBy))
            {
                throw new BaseException.NotFoundException("not_found", $"Question ID {questionId} not found");
            }

            List<Question> questions = new List<Question> { question };

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

            return questionDtos.First(); // Return the QuestionMainViewDto
        }

        //=====================================================================================================================================
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
        public async Task<BasePaginatedList<QuestionMainViewDto>> SearchQuestionsByContextMainViewAsync(string questionContext, int pageNumber, int pageSize)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(questionContext))
            {
                throw new BaseException.NotFoundException("not_found", "Search term cannot be null or empty.");
            }

            // Query all questions excluding deleted ones
            IQueryable<Question> questions = _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuestionContext.Contains(questionContext) && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz);

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

            // Get the total count of questions for pagination
            int totalQuestionsCount = await questions.CountAsync();

            // Check if any questions were found
            if (questions == null || !questions.Any())
            {
                throw new BaseException.NotFoundException("not_found", "No questions found matching the specified context.");
            }

            List<QuestionMainViewDto> questionDtos = _mapper.Map<List<QuestionMainViewDto>>(questions, opt =>
            {
                opt.Items["CreatedUsers"] = createdUsers;
                opt.Items["UpdatedUsers"] = updatedUsers;
            });

            // Return paginated results
            return new BasePaginatedList<QuestionMainViewDto>(questionDtos, totalQuestionsCount, pageNumber, pageSize);
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
        public async Task<BasePaginatedList<QuestionViewDto>> GetQuestionsByQuizIdAsync(string id, int pageNumber, int pageSize)
        {
            IQueryable<Question> query = _unitOfWork.GetRepository<Question>().Entities
            .Where(q => q.QuizId == id && string.IsNullOrWhiteSpace(q.DeletedBy))
            .Include(q => q.Quiz);

            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<QuestionViewDto>(new List<QuestionViewDto>(), 0, pageNumber, pageSize);
            }

            // Get the total count of questions for pagination
            int totalQuestionsCount = await query.CountAsync();

            // Fetch the paginated questions
            List<Question> paginatedQuestions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Use AutoMapper to map questions to QuestionViewDto
            List<QuestionViewDto> questionDtos = _mapper.Map<List<QuestionViewDto>>(paginatedQuestions);

            // Return paginated results
            return new BasePaginatedList<QuestionViewDto>(questionDtos, totalQuestionsCount, pageNumber, pageSize);
        }
        public async Task<BasePaginatedList<QuestionMainViewDto>> GetQuestionsMainViewByQuizIdAsync(string id, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
            {
                throw new BaseException.BadRequestException("invalid_page_number", "Page number must be greater than 0.");
            }

            if (pageSize <= 0)
            {
                throw new BaseException.BadRequestException("invalid_page_size", "Page size must be greater than 0.");
            }

            IQueryable<Question> query = _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuizId == id && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Quiz);

            // Get distinct IDs of users who created and last updated the questions
            List<string?> createdByIds = query.Select(q => q.CreatedBy).Distinct().ToList();
            List<string?> lastUpdatedByIds = query.Select(q => q.LastUpdatedBy).Distinct().ToList();

            // Retrieve user information based on the IDs
            List<User> createdUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => createdByIds.Contains(u.Id))
                .ToListAsync();

            List<User> updatedUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => lastUpdatedByIds.Contains(u.Id))
                .ToListAsync();


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

            // Use AutoMapper to map the Question entities to DTOs
            List<QuestionMainViewDto> questionDtos = _mapper.Map<List<QuestionMainViewDto>>(paginatedQuestions, opt =>
            {
                opt.Items["CreatedUsers"] = createdUsers;
                opt.Items["UpdatedUsers"] = updatedUsers;
            });


            // Return paginated results
            return new BasePaginatedList<QuestionMainViewDto>(questionDtos, totalQuestionsCount, pageNumber, pageSize);
        }

        //=====================================================================================================================================
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
        public async Task<BasePaginatedList<QuestionMainViewDto>> GetQuestionsMainViewAsync(int pageNumber, int pageSize)
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

            // Get distinct IDs of users who created and last updated the questions
            List<string?> createdByIds = query.Select(q => q.CreatedBy).Distinct().ToList();
            List<string?> lastUpdatedByIds = query.Select(q => q.LastUpdatedBy).Distinct().ToList();

            // Retrieve user information based on the IDs
            List<User> createdUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => createdByIds.Contains(u.Id))
                .ToListAsync();

            List<User> updatedUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => lastUpdatedByIds.Contains(u.Id))
                .ToListAsync();

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

            // Use AutoMapper to map the Question entities to DTOs
            List<QuestionMainViewDto> questionDtos = _mapper.Map<List<QuestionMainViewDto>>(paginatedQuestions, opt =>
            {
                opt.Items["CreatedUsers"] = createdUsers;
                opt.Items["UpdatedUsers"] = updatedUsers;
            });

            // Return paginated results
            return new BasePaginatedList<QuestionMainViewDto>(questionDtos, totalQuestionsCount, pageNumber, pageSize);
        }

        //=====================================================================================================================================

        // Method to add one or more questions
        public async Task<BaseResponse<string>?> AddQuestionAsync(List<QuestionCreateDto> dtos, User? currentUser)
        {
            // Check if the question list is null or empty
            if (dtos == null || !dtos.Any())
            {
                throw new BaseException.BadRequestException("invalid_arguments", "The list of questions cannot be empty.");
            }

            // Check if the current user is null (unauthorized user)
            if (currentUser == null)
                throw new BaseException.ValidationException("user_not_exists", "User not found or not authorized.");


            // Get a list of quiz IDs from the provided question DTOs (distinct quiz IDs)
            List<string> quizIds = dtos.Select(dto => dto.QuizId).Distinct().ToList();

            // Query the repository to get the quizzes that match the provided quiz IDs
            List<Quiz> existingQuizzes = await _unitOfWork.GetRepository<Quiz>()
                .Entities
                .Where(q => quizIds.Contains(q.Id))
                .ToListAsync();

            // Check for any missing quizzes (quizzes that do not exist in the repository)
            List<string> missingQuizzes = quizIds.Except(existingQuizzes.Select(q => q.Id)).ToList();
            if (missingQuizzes.Any())
            {
                throw new BaseException.NotFoundException("not_found", $"Quiz IDs not found: {string.Join(", ", missingQuizzes)}.");
            }

            // Query the repository to check for existing questions that have the same context (case-insensitive)
            List<Question> existingQuestions = await _unitOfWork.GetRepository<Question>()
                .Entities
                .Where(q => dtos.Select(dto => dto.QuestionContext.ToLower()).Contains(q.QuestionContext.ToLower()))
                .ToListAsync();

            // Check if any questions are duplicates based on the context (case-insensitive)
            List<QuestionCreateDto> duplicateQuestions = dtos.Where(dto => existingQuestions
                .Any(eq => eq.QuestionContext.ToLower() == dto.QuestionContext.ToLower()))
                .ToList();

            // If any duplicate questions are found, return null (indicating failure to insert)
            if (duplicateQuestions.Any())
            {
                return null;
            }

            // Create a list of new Question entities using the data from the question DTOs
            List<Question> questions = dtos.Select(dto => new Question
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuestionContext = dto.QuestionContext,
                QuizId = dto.QuizId,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow, 
                CreatedBy = currentUser.Id.ToUpper(), 
                LastUpdatedBy = currentUser.Id.ToUpper()
            }).ToList();

            // Insert all the questions into the database concurrently using Task.WhenAll
            List<Task> insertTasks = questions.Select(question =>
                _unitOfWork.GetRepository<Question>().InsertAsync(question)
            ).ToList();

            // Await all tasks to complete the insertion of all questions
            await Task.WhenAll(insertTasks);

            // Save the changes to the database
            await _unitOfWork.SaveAsync();

            // Return a successful response indicating the number of questions created
            return BaseResponse<string>.OkResponse($"{dtos.Count} question(s) created successfully.");
        }


        // Method to update an existing question
        public async Task<QuestionMainViewDto?> UpdateQuestionAsync(string id, QuestionUpdateDto dto, User? currentUser)
        {
            // Fetch the existing question by its ID
            Question question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(id)
                                ?? throw new BaseException.NotFoundException("not_found", $"Question with Id '{id}' not found.");

            // Validate DTO
            if (dto == null || string.IsNullOrWhiteSpace(dto.QuestionContext) /*|| string.IsNullOrWhiteSpace(dto.QuizId)*/)
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Question context or Quizid cannot be null or empty.");
            }

            // Validate that the provided QuizId exists
            //Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(dto.QuizId);
            //if (quiz == null)
            //{
            //    throw new BaseException.NotFoundException("not_found", $"Quiz with Id '{dto.QuizId}' does not exist.");
            //}

            Question? existingQuestion = await _unitOfWork.GetRepository<Question>()
               .Entities
               .FirstOrDefaultAsync(q => q.QuestionContext.ToLower() == dto.QuestionContext.ToLower());

            if (existingQuestion != null)
                return null;

            // Update QuizId if provided and valid
            //if (!string.IsNullOrWhiteSpace(dto.QuizId))
            //{
            //    question.QuizId = dto.QuizId;
            //}

            if (currentUser == null)
                throw new BaseException.ValidationException("user_not_exists", "User not found or not authorized.");

            // Update question context
            question.QuestionContext = dto.QuestionContext;
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

            // Optional: Check if users were found
            if (createdByUser == null || lastUpdatedByUser == null)
            {
                throw new BaseException.NotFoundException("not_found", "Some user information could not be found.");
            }

            // Fetch users who created and last updated the question
            List<User> createdUsers = new List<User> { createdByUser };
            List<User> updatedUsers = new List<User> { lastUpdatedByUser };

            // Map the updated question to QuestionMainViewDto using AutoMapper
            QuestionMainViewDto questionDto = _mapper.Map<QuestionMainViewDto>(question, opt =>
            {
                opt.Items["CreatedUsers"] = createdUsers;
                opt.Items["UpdatedUsers"] = updatedUsers;
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

        public async Task<IList<Question>> GetQuestionsWithOptionsEntitiesByQuizIdAsync(string quizId)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(quizId))
            {
                throw new BaseException.BadRequestException("invalid_quiz_id", "Quiz ID cannot be null or empty.");
            }

            // Fetch questions with options
            var questions = await _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuizId == quizId && string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Options) // Include navigation property
                .ToListAsync();

            // Throw an exception if no questions are found
            if (!questions.Any())
            {
                throw new BaseException.NotFoundException("not_found", $"No questions found for Quiz ID {quizId}.");
            }

            return questions;
        }
    }
}