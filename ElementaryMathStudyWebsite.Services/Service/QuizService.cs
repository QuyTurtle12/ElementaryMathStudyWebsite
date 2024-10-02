using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;
using AutoMapper;
using System.Collections.Generic;

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

            // Map quizzes to QuizMainViewDto
            List<QuizMainViewDto> quizDtos = _mapper.Map<List<QuizMainViewDto>>(quizzes);

            // Enrich DTOs with additional information
            await EnrichQuizDtosAsync(quizDtos, quizzes);

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
            await EnrichQuizDtoAsync(dto, quiz);

            return dto; // Return the populated QuizMainViewDto
        }

        // Get quizzes with pagination
        public async Task<BasePaginatedList<QuizMainViewDto>> GetQuizzesAsync(int pageNumber, int pageSize)
        {
            // Query all quizzes excluding deleted ones
            List<Quiz> allQuizzes = await GetQuizzesAsync();

            // Use AutoMapper to map quizzes to QuizMainViewDto
            List<QuizMainViewDto> quizDtos = _mapper.Map<List<QuizMainViewDto>>(allQuizzes);

            // Enrich each DTO with additional information
            await Task.WhenAll(quizDtos.Select(async dto =>
            {
                Quiz quiz = allQuizzes.First(q => q.Id == dto.Id);
                await EnrichQuizDtoAsync(dto, quiz);
            }));

            // If pageNumber or pageSize are 0 or negative, return all quizzes without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<QuizMainViewDto>(quizDtos, quizDtos.Count, 1, quizDtos.Count);
            }

            // Paginate the list of quizzes
            var paginatedQuizzesDto = quizDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new BasePaginatedList<QuizMainViewDto>(paginatedQuizzesDto, quizDtos.Count, pageNumber, pageSize);
        }

        // Search for quizzes where the quiz name contains a specified string
        public async Task<List<QuizViewDto>> SearchQuizzesByNameAsync(string quizName)
        {
            // Query all quizzes excluding deleted ones
            List<Quiz> quizzes = await GetQuizzesAsync();

            // Use AutoMapper to map quizzes to QuizViewDto
            List<QuizViewDto> quizDtos = _mapper.Map<List<QuizViewDto>>(quizzes);

            return quizDtos; // Return the list of QuizViewDto
        }

        // Get all quizzes that belong to a specific chapter or topic (by id conference)
        public async Task<List<QuizViewDto>> GetQuizzesByChapterOrTopicIdAsync(string? chapterId, string? topicId)
        {
            // Query all quizzes excluding deleted ones
            List<Quiz> quizzes = await GetQuizzesAsync();

            // Filter quizzes by chapterId or topicId if provided
            quizzes = FilterQuizzesByCriteria(quizzes, chapterId, topicId);

            // Convert Quiz entities to QuizViewDto using AutoMapper
            List<QuizViewDto> quizDtos = _mapper.Map<List<QuizViewDto>>(quizzes);

            return quizDtos; // Return the list of QuizViewDto
        }

        // Get the name of a quiz by its Id
        public async Task<string> GetQuizNameAsync(string quizId)
        {
            // Fetch the quiz by its Id
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);

            // Check if quiz exists and throw an exception if it doesn't
            if (quiz == null)
            {
                throw new BaseException.NotFoundException("not_found", "Quiz not found.");
            }

            // Return the quiz name
            return quiz.QuizName ?? string.Empty; // Return an empty string if QuizName is null
        }

        // Create a new quiz
        public async Task<BaseResponse<string>> AddQuizAsync(QuizCreateDto dto)
        {
            // Validate input DTO
            if (dto == null)
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Quiz data cannot be null.");
            }

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();

            // Create a new Quiz entity
            Quiz quiz = new()
            {
                Id = Guid.NewGuid().ToString(),
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
        }

        // Update an existing quiz
        public async Task<QuizMainViewDto> UpdateQuizAsync(string quizId, QuizUpdateDto dto)
        {
            // Fetch the existing quiz by its ID
            Quiz quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId)
                        ?? throw new BaseException.NotFoundException("not_found", $"Quiz with Id '{quizId}' not found.");

            // Update quiz information with values from the DTO
            quiz.QuizName = dto.QuizName;
            quiz.Criteria = dto.Criteria;
            quiz.Status = dto.Status;

            // Update Chapter reference if ChapterId is provided
            if (!string.IsNullOrWhiteSpace(dto.ChapterId))
            {
                var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(dto.ChapterId)
                             ?? throw new BaseException.BadRequestException("invalid_arguments", $"Chapter with Id '{dto.ChapterId}' not found.");
                quiz.Chapter = chapter; // Set the Chapter reference
            }

            // Update Topic reference if TopicId is provided
            if (!string.IsNullOrWhiteSpace(dto.TopicId))
            {
                var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(dto.TopicId)
                            ?? throw new BaseException.BadRequestException("invalid_arguments", $"Topic with Id '{dto.TopicId}' not found.");
                quiz.Topic = topic; // Set the Topic reference
            }

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();
            quiz.LastUpdatedBy = currentUser.Id ?? string.Empty; // Update LastUpdatedBy

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Fetch the list of quizzes to map
            var quizzes = new List<Quiz> { quiz };
            var quizDtos = _mapper.Map<List<QuizMainViewDto>>(quizzes);

            // Enrich the DTO with additional information
            var quizDto = quizDtos.First();
            await EnrichQuizDtoAsync(quizDto, quiz);

            return quizDto; // Return the updated quiz DTO
        }

        // Delete a quiz
        public async Task<BaseResponse<string>> DeleteQuizAsync(string quizId)
        {
            Quiz? quiz;

            if (_unitOfWork.IsValid<Quiz>(quizId))
            {
                quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);
            }
            else
            {
                throw new BaseException.NotFoundException("not_found", "Quiz ID not found");
            }

            // Audit the quiz to mark it as deleted
            _userService.AuditFields(quiz!, false, true);

            await _unitOfWork.SaveAsync();

            // Fetch all related questions that are not already deleted
            List<Question> questionsToDelete = await _unitOfWork.GetRepository<Question>()
                .GetEntitiesWithCondition(q => q.QuizId == quizId && string.IsNullOrWhiteSpace(q.DeletedBy))
                .ToListAsync();

            // Use Task.WhenAll to delete all questions asynchronously
            var deleteTasks = questionsToDelete.Select(q => _questionService.DeleteQuestion(q.Id));
            await Task.WhenAll(deleteTasks);

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
        private async Task EnrichQuizDtosAsync(List<QuizMainViewDto> quizDtos, List<Quiz> quizzes)
        {
            foreach (Quiz quiz in quizzes)
            {
                var dto = quizDtos.FirstOrDefault(q => q.Id == quiz.Id);

                if (dto != null)
                {
                    await EnrichQuizDtoAsync(dto, quiz);
                }
                else
                {
                    throw new BaseException.NotFoundException("not_found", $"Quiz ID {quiz.Id} not found");
                }
            }
        }

        // Method to enrich a single QuizMainViewDto
        private async Task EnrichQuizDtoAsync(QuizMainViewDto dto, Quiz quiz)
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
        private List<Quiz> FilterQuizzesByCriteria(List<Quiz> quizzes, string? chapterId, string? topicId)
        {
            return quizzes.Where(q =>
                (string.IsNullOrWhiteSpace(chapterId) || q.Topic?.Id == chapterId) && // Filter by chapterId if provided
                (string.IsNullOrWhiteSpace(topicId) || q.Topic?.Id == topicId)           // Filter by topicId if provided
            ).ToList();
        }


    }
}