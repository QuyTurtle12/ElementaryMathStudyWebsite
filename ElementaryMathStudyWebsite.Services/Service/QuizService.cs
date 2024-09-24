using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuizService : IAppQuizServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;

        // constructor
        public QuizService(IUnitOfWork unitOfWork, IAppUserServices userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        // Get all quizzes with properties mapped to QuizMainViewDto
        public async Task<List<QuizMainViewDto>> GetAllQuizzesAsync()
        {
            // Query all quizzes excluding deleted ones
            var quizzes = await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .Include(q => q.Chapter)
                .Include(q => q.Topic)
                .ToListAsync();

            // List to hold QuizMainViewDto
            List<QuizMainViewDto> quizDtos = new List<QuizMainViewDto>();

            foreach (var quiz in quizzes)
            {
                // Get creator and last updater information
                var creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.CreatedBy ?? string.Empty);
                var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.LastUpdatedBy ?? string.Empty);

                // Create QuizMainViewDto
                QuizMainViewDto dto = new QuizMainViewDto
                {
                    Id = quiz.Id,
                    QuizName = quiz.QuizName,
                    Criteria = quiz.Criteria,
                    Status = quiz.Status,
                    ChapterName = quiz.Chapter?.ChapterName ?? string.Empty,  // Assuming Chapter has ChapterName
                    TopicName = quiz.Topic?.TopicName ?? string.Empty,        // Assuming Topic has TopicName

                    CreatedBy = quiz.CreatedBy ?? string.Empty,
                    CreatorName = creator?.FullName ?? string.Empty,
                    CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                    LastUpdatedBy = quiz.LastUpdatedBy ?? string.Empty,
                    LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                    LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,

                    CreatedTime = quiz.CreatedTime, // Assuming CreatedTime has a value
                    LastUpdatedTime = quiz.LastUpdatedTime // Assuming LastUpdatedTime has a value
                };

                quizDtos.Add(dto);
            }

            return quizDtos;
        }


        // Get Quiz with quizId
        public async Task<QuizMainViewDto?> GetQuizByQuizIdAsync(string quizId)
        {
            // Fetch the quiz by its Id
            var quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);

            // Check if quiz exists
            if (quiz == null || !string.IsNullOrWhiteSpace(quiz.DeletedBy))
            {
                return null;
            }

            // Get audit field info
            var creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.CreatedBy ?? string.Empty);
            var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.LastUpdatedBy ?? string.Empty);

            // Create a new QuizMainViewDto and map properties
            var dto = new QuizMainViewDto
            {
                Id = quiz.Id,
                QuizName = quiz.QuizName,
                Criteria = quiz.Criteria,
                Status = quiz.Status,
                ChapterName = quiz.Chapter?.ChapterName ?? string.Empty,
                TopicName = quiz.Topic?.TopicName ?? string.Empty,
                CreatedBy = creator?.FullName ?? string.Empty,
                LastUpdatedBy = lastUpdatedPerson?.FullName ?? string.Empty,
            };

            return dto; // Return the QuizMainViewDto
        }

        //==================================================================================

        // Search for quizzes where the quiz name contains a specified string
        public async Task<List<QuizViewDto>> SearchQuizzesByNameAsync(string quizName)
        {
            // Fetch quizzes that match the quiz name
            var quizzes = await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => q.QuizName.Contains(quizName) && string.IsNullOrWhiteSpace(q.DeletedBy))
                .ToListAsync();

            var quizDtos = quizzes.Select(quiz => new QuizViewDto
            {
                Id = quiz.Id,
                QuizName = quiz.QuizName,
                Criteria = quiz.Criteria,
                Status = quiz.Status,
            }).ToList();

            return quizDtos; // Return the list of QuizViewDto
        }

        // Get all quizzes that belong to a specific chapter or topic (by id conference)
        public async Task<List<QuizViewDto>> GetQuizzesByChapterOrTopicIdAsync(string? chapterId, string? topicId)
        {
            IQueryable<Quiz> query = _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy));

            if (!string.IsNullOrWhiteSpace(chapterId))
            {
                query = query.Where(q => q.Chapter != null && q.Chapter.Id == chapterId);
            }

            if (!string.IsNullOrWhiteSpace(topicId))
            {
                query = query.Where(q => q.Topic != null && q.Topic.Id == topicId);
            }

            // Execute the query and get the quizzes
            var quizzes = await query.ToListAsync();

            // Convert Quiz entities to QuizViewDto
            var quizDtos = quizzes.Select(quiz => new QuizViewDto
            {
                Id = quiz.Id,
                QuizName = quiz.QuizName,
                Criteria = quiz.Criteria,
                Status = quiz.Status,
                ChapterName = quiz.Chapter?.ChapterName,
                TopicName = quiz.Topic?.TopicName
            }).ToList();

            return quizDtos;
        }

        // Get all quizzes with pagination and full properties
        public async Task<BasePaginatedList<QuizMainViewDto>?> GetQuizzesAsync(int pageNumber, int pageSize)
        {
            // Query all quizzes from the database
            IQueryable<Quiz> query = _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy));

            // Retrieve all quizzes from the database
            var allQuizzes = await query.ToListAsync();

            // List to hold quiz view DTOs for admin
            List<QuizMainViewDto> quizDtos = new List<QuizMainViewDto>();

            foreach (var quiz in allQuizzes)
            {
                // Get audit field info
                User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.CreatedBy ?? string.Empty);
                User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.LastUpdatedBy ?? string.Empty);

                // Create a new QuizViewDto and map properties
                QuizMainViewDto dto = new QuizMainViewDto
                {
                    Id = quiz.Id,
                    QuizName = quiz.QuizName,
                    Criteria = quiz.Criteria,
                    Status = quiz.Status,
                    ChapterName = quiz.Chapter?.ChapterName ?? string.Empty,
                    TopicName = quiz.Topic?.TopicName ?? string.Empty,
                    CreatedBy = creator?.FullName ?? string.Empty,
                    LastUpdatedBy = lastUpdatedPerson?.FullName ?? string.Empty
                };
                quizDtos.Add(dto); // Add the DTO to the list
            }

            // If pageNumber or pageSize are 0 or negative, return all quizzes without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<QuizMainViewDto>(quizDtos, quizDtos.Count, 1, quizDtos.Count);
            }

            // Paginate the list of quizzes
            var paginatedQuizzesDto = quizDtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new BasePaginatedList<QuizMainViewDto>(paginatedQuizzesDto, quizDtos.Count, pageNumber, pageSize);
        }

        //==================================================================================

        // Get the name of a quiz by its Id
        public async Task<string> GetQuizNameAsync(string quizId)
        {
            // Fetch the quiz by its Id
            var quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);

            // Check if quiz exists
            if (quiz == null)
            {
                // Optionally, you can throw an exception or return a default value
                throw new KeyNotFoundException($"Quiz with Id '{quizId}' not found.");
            }

            // Return the quiz name
            return quiz.QuizName;
        }

        //==================================================================================

        // Create a new quiz
        public async Task<QuizMainViewDto> AddQuizAsync(QuizCreateDto dto)
        {
            // Validate input DTO
            if (dto == null)
            {
                throw new BaseException.BadRequestException("invalid_arguments", "Quiz data cannot be null");
            }

            // Create a new Quiz entity
            Quiz quiz = new()
            {
                Id = Guid.NewGuid().ToString(),
                QuizName = dto.QuizName,
                Criteria = dto.Criteria,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };

            // Insert the new quiz into the repository
            await _unitOfWork.GetRepository<Quiz>().InsertAsync(quiz);
            await _unitOfWork.SaveAsync();

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();
            quiz.LastUpdatedBy = currentUser.Id.ToUpper(); // Update LastUpdatedBy

            // Fetch creator and last updated person information
            var creator = await _unitOfWork.GetRepository<User>().FindByConditionAsync(c => c.Id != null && c.Id.Equals(quiz.CreatedBy));
            var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.LastUpdatedBy);

            // Return the created quiz as DTO including audit fields
            return new QuizMainViewDto
            {
                Id = quiz.Id,
                QuizName = quiz.QuizName,
                Criteria = quiz.Criteria,
                Status = quiz.Status,
                ChapterName = dto.ChapterName,
                TopicName = dto.TopicName,
                CreatedBy = quiz.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = quiz.LastUpdatedBy,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                CreatedTime = quiz.CreatedTime,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
        }



        // Update an existing quiz
        public async Task<QuizUpdateDto> UpdateQuizAsync(QuizUpdateDto dto)
        {
            // Fetch the existing quiz by its ID
            var quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(dto.Id)
                        ?? throw new BaseException.NotFoundException("not_found", $"Quiz with Id '{dto.Id}' not found.");


            // Update quiz information with values from the DTO
            quiz.QuizName = dto.QuizName;
            quiz.Criteria = dto.Criteria;
            quiz.Status = dto.Status;

            // Update Chapter reference if ChapterId is provided
            if (!string.IsNullOrWhiteSpace(dto.ChapterId))
            {
                var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(dto.ChapterId);
                if (chapter == null)
                {
                    throw new BaseException.BadRequestException("invalid_arguments", $"Chapter with Id '{dto.ChapterId}' not found.");
                }
                quiz.Chapter = chapter; // Set the Chapter reference
            }

            // Update Topic reference if TopicId is provided
            if (!string.IsNullOrWhiteSpace(dto.TopicId))
            {
                var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(dto.TopicId);
                if (topic == null)
                {
                    throw new BaseException.BadRequestException("invalid_arguments", $"Topic with Id '{dto.TopicId}' not found.");
                }
                quiz.Topic = topic; // Set the Topic reference
            }

            // Get the current user for auditing purposes
            User currentUser = await _userService.GetCurrentUserAsync();
            quiz.LastUpdatedBy = currentUser.Id.ToUpper(); // Update LastUpdatedBy

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Return the updated quiz information in a DTO
            return new QuizUpdateDto
            {
                Id = quiz.Id,
                QuizName = quiz.QuizName,
                Criteria = quiz.Criteria,
                Status = quiz.Status ?? true,
                ChapterId = quiz.Chapter?.Id ?? string.Empty,
                TopicId = quiz.Topic?.Id ?? string.Empty,
                LastUpdatedBy = currentUser.FullName,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
        }

        // Delete a quiz
        public async Task<DeleteQuizDto> DeleteQuizAsync(string quizId)
        {
            // Check if the provided quizId is null or whitespace
            if (string.IsNullOrWhiteSpace(quizId))
            {
                throw new BaseException.BadRequestException("invalid_arguments", $"Deleted quiz {quizId}");
            }

            // Fetch the quiz by ID
            var quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId)
                        ?? throw new BaseException.NotFoundException("not_found", $"{quizId} not found");

            // Check if the quiz has already been deleted
            if (quiz.DeletedBy != null)
            {
                throw new BaseException.BadRequestException("invalid_arguments", "This quiz has already been deleted.");
            }

            // Get the current user information
            User currentUser = await _userService.GetCurrentUserAsync();

            // Mark the quiz as deleted by setting DeletedBy and DeletedTime
            quiz.DeletedBy = currentUser.Id.ToUpper();
            quiz.DeletedTime = CoreHelper.SystemTimeNow;

            // Fetch the creator and last updated person information
            var creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.CreatedBy ?? string.Empty);
            var lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(quiz.LastUpdatedBy ?? string.Empty);

            // Update the quiz entity and save changes
            await _unitOfWork.GetRepository<Quiz>().UpdateAsync(quiz);
            await _unitOfWork.SaveAsync();

            // Return the details of the deleted quiz in a DeleteQuizDto
            return new DeleteQuizDto
            {
                Id = quiz.Id,
                QuizName = quiz.QuizName,
                DeletedBy = quiz.DeletedBy,
                DeletedTime = CoreHelper.SystemTimeNow,
                DeletedName = currentUser.FullName,
                CreatorName = creator?.FullName ?? string.Empty,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
            };
        }
    }
}
// 10Đ