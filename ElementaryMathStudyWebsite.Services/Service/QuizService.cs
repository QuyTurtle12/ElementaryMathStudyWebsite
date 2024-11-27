using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;
using AutoMapper;
namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuizService : IAppQuizServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppQuestionServices _questionService;
        private readonly IMapper _mapper;

        // constructor
        public QuizService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppQuestionServices questionService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _questionService = questionService;
            _mapper = mapper;
        }

        // Get All Quizzes exist without deleted
        public async Task<List<QuizMainViewDto>> GetAllQuizzesAsync()
        {
            // Query all quizzes, excluding deleted ones
            List<Quiz> quizzes = await _unitOfWork.GetRepository<Quiz>()
                .GetEntitiesWithCondition(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .ToListAsync();

            // If no quizzes are found, throw an exception
            if (quizzes == null || quizzes.Count == 0)
                throw new BaseException.NotFoundException("not_found", "Quizzes not found.");

            // Map quizzes to DTOs, including user information for creators and last-updated users
            List<QuizMainViewDto> quizDtos = await MapQuizzesToDto(quizzes);

            // Return the mapped quiz DTOs
            return quizDtos;
        }

        // Get Quiz by its quizId
        public async Task<QuizMainViewDto> GetQuizByQuizIdAsync(string quizId)
        {
            // Fetch the quiz by its Id using Where
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => q.Id == quizId)
                .Include(q => q.Chapter)
                .Include(q => q.Topic)
                .FirstOrDefaultAsync();


            // Check if quiz exists or has been deleted
            if (quiz == null || !string.IsNullOrWhiteSpace(quiz.DeletedBy))
            {
                throw new BaseException.NotFoundException("not_found", $"Quiz ID {quizId} not found");
            }

            // Create a list with the single quiz since the MapQuizzesToDto method works with a list of quizzes
            List<Quiz> quizzes = new List<Quiz> { quiz };

            // Map the Quiz entity to QuizMainViewDto using the mapping function
            List<QuizMainViewDto> quizDtos = await MapQuizzesToDto(quizzes);

            return quizDtos.First(); // Return the populated QuizMainViewDto
        }

        // Get quizzes with pagination
        public async Task<BasePaginatedList<QuizViewDto>> GetQuizzesAsync(int pageNumber, int pageSize)
        {
            // Query all quizzes excluding deleted ones
            List<Quiz> allQuizzes = await GetQuizzesAsync();

            if (allQuizzes == null)
                throw new BaseException.NotFoundException("not_found", $"Quizzes not found.");

            // Use AutoMapper to map quizzes to QuizMainViewDto
            List<QuizViewDto> quizDtos = _mapper.Map<List<QuizViewDto>>(allQuizzes);

            // If pageNumber or pageSize are 0 or negative, return all quizzes without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<QuizViewDto>(quizDtos, quizDtos.Count, 1, quizDtos.Count);
            }

            // Paginate the list of quizzes
            List<QuizViewDto> paginatedQuizzesDto = quizDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new BasePaginatedList<QuizViewDto>(paginatedQuizzesDto, quizDtos.Count, pageNumber, pageSize);
        }
        public async Task<BasePaginatedList<QuizMainViewDto>> GetQuizzesMainViewAsync(int pageNumber, int pageSize)
        {
            List<Quiz> allQuizzes = await GetQuizzesAsync();

            if (allQuizzes == null)
                throw new BaseException.NotFoundException("not_found", $"Quizzes not found.");

            List<QuizMainViewDto> quizDtos = await MapQuizzesToDto(allQuizzes);

            if (pageNumber <= 0 || pageSize <= 0)
                return new BasePaginatedList<QuizMainViewDto>(quizDtos, quizDtos.Count, 1, quizDtos.Count);

            List<QuizMainViewDto> paginatedQuizzesDto = quizDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new BasePaginatedList<QuizMainViewDto>(paginatedQuizzesDto, quizDtos.Count, pageNumber, pageSize);
        }

        // Search for quizzes where the quiz name contains a specified string
        public async Task<List<QuizViewDto>> SearchQuizzesByNameAsync(string quizName)
        {
            // Query all quizzes excluding deleted ones
            List<Quiz> quizzes = await GetQuizzesAsync();

            if (quizzes == null)
                throw new BaseException.NotFoundException("not_found", $"Quizzes not found.");

            // Check if quizName is null or empty
            if (string.IsNullOrWhiteSpace(quizName))
            {
                return _mapper.Map<List<QuizViewDto>>(quizzes);
            }

            // Check if any quiz contains the specified name
            if (!quizzes.Any(q => q.QuizName.Contains(quizName, StringComparison.OrdinalIgnoreCase)))
                // If no quiz is found, throw a BaseException
                throw new BaseException.NotFoundException("not_found", "No quizzes found with the specified name.");

            // Filter quizzes where the quiz name contains the specified string (case-insensitive)
            List<Quiz> filteredQuizzes = quizzes
                .Where(q => q.QuizName.Contains(quizName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Use AutoMapper to map filtered quizzes to QuizViewDto
            List<QuizViewDto> quizDtos = _mapper.Map<List<QuizViewDto>>(filteredQuizzes);

            return quizDtos; // Return the list of filtered QuizViewDto
        }
        public async Task<BasePaginatedList<QuizMainViewDto>> SearchQuizzesMainViewByNameAsync(string quizName, int pageNumber, int pageSize)
        {
            // Lấy tất cả các quiz (ngoại trừ những quiz đã bị xóa)
            List<Quiz> quizzes = await GetQuizzesAsync();

            // Kiểm tra nếu danh sách quiz bị null hoặc rỗng
            if (quizzes == null || !quizzes.Any())
                throw new BaseException.NotFoundException("not_found", "No quizzes found.");

            // Lọc các quiz chứa tên tương ứng (không phân biệt chữ hoa/thường)
            List<Quiz> filteredQuizzes = quizzes
                .Where(q => q.QuizName.Contains(quizName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Nếu không có quiz nào phù hợp, ném ngoại lệ
            if (!filteredQuizzes.Any())
                throw new BaseException.NotFoundException("not_found", $"No quizzes found matching the name '{quizName}'.");

            // Use AutoMapper to map quizzes to QuizMainViewDto
            List<QuizMainViewDto> quizDtos = await MapQuizzesToDto(filteredQuizzes);

            // If pageNumber or pageSize are 0 or negative, return all quizzes without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<QuizMainViewDto>(quizDtos, quizDtos.Count, 1, quizDtos.Count);
            }

            // Paginate the list of quizzes
            List<QuizMainViewDto> paginatedQuizzesDto = quizDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new BasePaginatedList<QuizMainViewDto>(paginatedQuizzesDto, quizDtos.Count, pageNumber, pageSize);
        }

        // Get a quiz that belongs to a specific chapter or topic (by id conference)
        public async Task<QuizViewDto?> GetQuizByChapterOrTopicIdAsync(string? chapterId, string? topicId)
        {
            // Query all quizzes excluding deleted ones
            List<Quiz> quizzes = await GetQuizzesAsync();

            // Filter the quiz by chapterId or topicId using the existing FilterQuizzesByCriteria method
            Quiz? filteredQuiz = FilterQuizzesByCriteria(quizzes, chapterId, topicId);

            // If no quiz is found, throw a NotFoundException
            if (filteredQuiz == null)
                throw new BaseException.NotFoundException("not_found", $"Quiz with chapterId '{chapterId}' or topicId '{topicId}' not found");


            QuizViewDto quizDto = _mapper.Map<QuizViewDto>(filteredQuiz);

            return quizDto; // Return the QuizViewDto
        }

        // Get the name of a quiz by its Id
        public async Task<string> GetQuizNameAsync(string quizId)
        {
            // Fetch the quiz by its Id
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);

            // Check if quiz exists and throw an exception if it doesn't
            if (quiz == null)
                throw new BaseException.NotFoundException("not_found", $"Quiz with Id '{quizId}' not found.");

            // Return the quiz name
            return quiz.QuizName ?? string.Empty; // Return an empty string if QuizName is null
        }

        // Create a new quizs
        public async Task<QuizMainViewDto?> AddQuizAsync(QuizCreateDto dto, User? currentUser)
        {
            // Validate QuizName
            if (string.IsNullOrWhiteSpace(dto.QuizName))
            {
                throw new BaseException.ValidationException("Invalid_arguments", "Quizname and cannot be null or empty.");
            }

            // Validate data
            if (dto.Criteria <= 0 || dto.Criteria > 10)
                throw new BaseException.ValidationException("Invalid_arguments", "Criteria must be greater than 0 and less than or equal to 10.");

            if (string.IsNullOrWhiteSpace(dto.QuizName))
                throw new BaseException.ValidationException("Invalid_arguments", "Quiz name cannot be empty.");

            Quiz? existingQuiz = await _unitOfWork.GetRepository<Quiz>()
                .Entities
                .FirstOrDefaultAsync(q => q.QuizName.ToLower() == dto.QuizName.ToLower());

            if (existingQuiz != null)
                throw new BaseException.ValidationException("quiz_name_exists", "A quiz with the same name already exists.");

            if (currentUser == null)
                throw new BaseException.ValidationException("user_not_exists", "User not found or not authorized.");

            // Get the current user for auditing purposes
            //User currentUser = await _userService.GetCurrentUserAsync(); 

            // Create a new Quiz entity
            Quiz quiz = new()
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                QuizName = dto.QuizName,
                Criteria = dto.Criteria,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                CreatedBy = currentUser.Id.ToUpper(), // Set CreatedBy
                LastUpdatedBy = currentUser.Id.ToUpper() // Set LastUpdatedBy to the same user
            };

            // Insert the new quiz into the repository
            await _unitOfWork.GetRepository<Quiz>().InsertAsync(quiz);
            await _unitOfWork.SaveAsync();

            // Map the newly created quiz to QuizMainViewDto
            List<Quiz> quizzes = new List<Quiz> { quiz };
            List<QuizMainViewDto> quizDtos = await MapQuizzesToDto(quizzes);

            // Return the created quiz DTO
            return quizDtos.First();
        }

        // Update an existing quiz
        public async Task<QuizMainViewDto?> UpdateQuizAsync(string quizId, QuizUpdateDto dto, User? currentUser)
        {
            // Fetch the existing quiz by its ID
            Quiz quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId)
                        ?? throw new BaseException.NotFoundException("not_found", $"Quiz with Id '{quizId}' not found.");

            // Validate data
            if (dto.Criteria <= 0 || dto.Criteria > 10)
                throw new BaseException.ValidationException("Invalid_arguments", "Criteria must be greater than 0 and less than or equal to 10.");

            if (string.IsNullOrWhiteSpace(dto.QuizName))
                throw new BaseException.ValidationException("Invalid_arguments", "Quiz name cannot be empty.");

            Quiz? existingQuiz = await _unitOfWork.GetRepository<Quiz>()
                .Entities
                .FirstOrDefaultAsync(q => q.QuizName.ToLower() == dto.QuizName.ToLower());

            if (existingQuiz != null)
                return null;
                //throw new BaseException.ValidationException("quiz_name_exists", "A quiz with the same name already exists.");

            if (currentUser == null)
                throw new BaseException.ValidationException("user_not_exists", "User not found or not authorized.");

            // Update quiz information with values from the DTO
            quiz.QuizName = dto.QuizName;
            quiz.Criteria = dto.Criteria;
            quiz.Status = dto.Status;

            // Get the current user for auditing purposes
            //User currentUser = await _userService.GetCurrentUserAsync();
            //quiz.LastUpdatedBy = currentUser.Id ?? string.Empty; // Update LastUpdatedBy

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Map the newly created quiz to QuizMainViewDto
            List<Quiz> quizzes = new List<Quiz> { quiz };
            List<QuizMainViewDto> quizDtos = await MapQuizzesToDto(quizzes);

            // Return the created quiz DTO
            return quizDtos.First();
        }

        // Delete a quiz
        public async Task<BaseResponse<string>> DeleteQuizAsync(string quizId)
        {
            if (string.IsNullOrWhiteSpace(quizId))
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Quiz ID cannot be null or empty.");
            }

            Quiz? quiz;

            if (_unitOfWork.IsValid<Quiz>(quizId))
                quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);
            else
                throw new BaseException.NotFoundException("not_found", "Quiz ID not found");

            // Audit the quiz to mark it as deleted
            _userService.AuditFields(quiz!, false, true);
            await _unitOfWork.SaveAsync();

            // Fetch all related questions that are not already deleted
            //List<Question> questionsToDelete = await _unitOfWork.GetRepository<Question>()
            //    .GetEntitiesWithCondition(q => q.QuizId == quizId && string.IsNullOrWhiteSpace(q.DeletedBy))
            //    .ToListAsync();

            // Use Task.WhenAll to delete all questions asynchronously
            //IEnumerable<Task> deleteTasks = questionsToDelete.Select(q => _questionService.DeleteQuestion(q.Id));
            //await Task.WhenAll(deleteTasks);

            // Return a success response
            return BaseResponse<string>.OkResponse("Quiz deleted successfully.");
        }

        //===================================================================================================

        public async Task<List<QuizMainViewDto>> MapQuizzesToDto(IEnumerable<Quiz> quizzes)
        {
            // Get distinct IDs of users who created and last updated the quizzes
            List<string?> createdByIds = quizzes.Select(q => q.CreatedBy).Distinct().ToList();
            List<string?> lastUpdatedByIds = quizzes.Select(q => q.LastUpdatedBy).Distinct().ToList();

            // Retrieve user information based on the IDs of creators and last-updated users
            List<User> createdUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => createdByIds.Contains(u.Id))
                .ToListAsync();

            List<User> updatedUsers = await _unitOfWork.GetRepository<User>()
                .GetEntitiesWithCondition(u => lastUpdatedByIds.Contains(u.Id))
                .ToListAsync();

            // Check if the user information could not be found and throw an exception if so
            if (createdUsers == null || updatedUsers == null)
            {
                throw new BaseException.NotFoundException("not_found", "Some user information could not be found.");
            }

            // Map the quiz data to the DTO format and map the creator and updater user information to the DTO
            List<QuizMainViewDto> quizDtos = _mapper.Map<List<QuizMainViewDto>>(quizzes, opts =>
            {
                opts.Items["CreatedUser"] = createdUsers;
                opts.Items["UpdatedUser"] = updatedUsers;
            });

            // Return the mapped quiz DTOs
            return quizDtos;
        }

        // Method to query quizzes from the repository
        private async Task<List<Quiz>> GetQuizzesAsync()
        {
            return await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Chapter)
                .Include(q => q.Topic)
                .ToListAsync();
        }

        // Method to filter quizzes by chapterId or topicId
        private Quiz? FilterQuizzesByCriteria(List<Quiz> quizzes, string? chapterId, string? topicId)
        {
            return quizzes.FirstOrDefault(q =>
                (string.IsNullOrWhiteSpace(chapterId) || q.Chapter?.Id == chapterId) &&  // Filter by chapterId if provided
                (string.IsNullOrWhiteSpace(topicId) || q.Topic?.Id == topicId)           // Filter by topicId if provided
            );
        }


    }
}