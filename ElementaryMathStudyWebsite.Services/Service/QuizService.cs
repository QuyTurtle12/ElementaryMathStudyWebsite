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

        // Get All Quiz with full properties
        public async Task<List<QuizMainViewDto>> GetAllQuizzesAsync()
        {
            // Query all quizzes excluding deleted ones
            List<Quiz> quizzes = await GetQuizzesAsync();

            if (quizzes == null)
                throw new BaseException.NotFoundException("not_found", $"Quizzes not found.");

            // Map quizzes to QuizMainViewDto
            List<QuizMainViewDto> quizDtos = _mapper.Map<List<QuizMainViewDto>>(quizzes);

            // Enrich DTOs with additional information
            await EnrichDtosAsync(quizDtos, quizzes);

            return quizDtos;
        }

        // Get Quiz by its quizId
        public async Task<QuizMainViewDto> GetQuizByQuizIdAsync(string quizId)
        {
            // Fetch the quiz by its Id
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);

            // Check if quiz exists or has been deleted
            if (quiz == null || !string.IsNullOrWhiteSpace(quiz.DeletedBy))
            {
                throw new BaseException.NotFoundException("not_found", $"Quiz ID {quizId} not found");
            }

            // Map the Quiz entity to QuizMainViewDto
            QuizMainViewDto dto = _mapper.Map<QuizMainViewDto>(quiz);

            // Enrich the DTO with additional information
            await EnrichDtoAsync(dto, quiz);

            return dto; // Return the populated QuizMainViewDto
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
        public async Task<QuizMainViewDto> AddQuizAsync(QuizCreateDto dto)
        {
            // Validate QuizName
            if (string.IsNullOrWhiteSpace(dto.QuizName))
            {
                throw new BaseException.BadRequestException("Invalid_arguments", "Quizname and cannot be null or empty.");
            }

            if (dto.Criteria > 10 || dto.Criteria < 0)
                throw new BaseException.BadRequestException("Invalid_arguments", "Criteria must be less than or equal to 10.");

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();

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
            QuizMainViewDto quizDto = _mapper.Map<QuizMainViewDto>(quiz);

            // Enrich the DTO with additional information
            await EnrichDtoAsync(quizDto, quiz);

            // Return the created quiz DTO
            return quizDto;
        }

        // Update an existing quiz
        public async Task<QuizMainViewDto> UpdateQuizAsync(string quizId, QuizUpdateDto dto)
        {
            // Fetch the existing quiz by its ID
            Quiz quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId)
                        ?? throw new BaseException.NotFoundException("not_found", $"Quiz with Id '{quizId}' not found.");

            // Validate data
            if (dto.Criteria <= 0 || dto.Criteria > 10)
                throw new BaseException.BadRequestException("Invalid_arguments", "Criteria must be greater than 0 and less than or equal to 10.");

            if (string.IsNullOrWhiteSpace(dto.QuizName))
                throw new BaseException.NotFoundException("not_found", "Quiz name cannot be empty.");

            // Update quiz information with values from the DTO
            quiz.QuizName = dto.QuizName;
            quiz.Criteria = dto.Criteria;
            quiz.Status = dto.Status;

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();
            quiz.LastUpdatedBy = currentUser.Id ?? string.Empty; // Update LastUpdatedBy

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Map the updated quiz to QuizMainViewDto
            QuizMainViewDto quizDto = _mapper.Map<QuizMainViewDto>(quiz);

            // Enrich the DTO with additional information
            await EnrichDtoAsync(quizDto, quiz);

            // Return the updated quiz DTO
            return quizDto;
        }

        // Delete a quiz
        public async Task<BaseResponse<string>> DeleteQuizAsync(string quizId)
        {
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

        // Method to query quizzes from the repository
        private async Task<List<Quiz>> GetQuizzesAsync()
        {
            return await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Chapter)
                .Include(q => q.Topic)
                .ToListAsync();
        }

        // Method to enrich DTOs with additional information
        private async Task EnrichDtosAsync(List<QuizMainViewDto> quizDtos, List<Quiz> quizzes)
        {
            foreach (Quiz quiz in quizzes)
            {
                QuizMainViewDto? dto = quizDtos.FirstOrDefault(q => q.Id == quiz.Id);

                if (dto != null)
                {
                    await EnrichDtoAsync(dto, quiz);
                }
                else
                {
                    throw new BaseException.NotFoundException("not_found", $"Quiz ID {quiz.Id} not found");
                }
            }
        }

        // Method to enrich a single QuizMainViewDto
        private async Task EnrichDtoAsync(QuizMainViewDto dto, Quiz quiz)
        {
            // Fetch creator and last updated person info
            User? creator = await _userService.GetUserByIdAsync(quiz.CreatedBy!);
            User? lastUpdatedPerson = await _userService.GetUserByIdAsync(quiz.LastUpdatedBy!);

            // Assign additional information manually after mapping
            dto.CreatedBy = quiz.CreatedBy ?? string.Empty;
            dto.CreatorName = creator?.FullName ?? string.Empty;
            dto.CreatorPhone = creator?.PhoneNumber ?? string.Empty;

            dto.LastUpdatedBy = quiz.LastUpdatedBy ?? string.Empty;
            dto.LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty;
            dto.LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty;

            dto.CreatedTime = quiz.CreatedTime;
            dto.LastUpdatedTime = quiz.LastUpdatedTime;
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