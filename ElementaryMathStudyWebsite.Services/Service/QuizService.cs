using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuizService : IAppQuizServices
    {
        private readonly IUnitOfWork _unitOfWork;

        // constructor
        public QuizService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //Get all quiz by default properties
        public async Task<List<Quiz>> GetAllQuizzesAsync()
        {
            // Get all quizzes excluding deleted ones
            return await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => string.IsNullOrWhiteSpace(q.DeletedBy))
                .ToListAsync();
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
                ChapterName = quiz.Chapter?.ChapterName,
                TopicName = quiz.Topic?.TopicName,
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
                ChapterName = quiz.Chapter?.ChapterName,  
                TopicName = quiz.Topic?.TopicName     
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
                    ChapterName = quiz.Chapter?.ChapterName,
                    TopicName = quiz.Topic?.TopicName,      
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
        public async Task<QuizViewDto> AddQuizAsync(CreateQuizDto dto)
        {
            // Validate input DTO
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Quiz data cannot be null.");
            }

            // Create a new Quiz entity
            Quiz quiz = new()
            {
                Id = Guid.NewGuid().ToString(),
                QuizName = dto.QuizName,
                Criteria = dto.Criteria
            };

            // Insert the new quiz into the repository
            await _unitOfWork.GetRepository<Quiz>().InsertAsync(quiz);
            await _unitOfWork.SaveAsync();

            // Return the created quiz as DTO
            return new QuizViewDto
            {
                Id = quiz.Id,
                QuizName = quiz.QuizName,
                Criteria = quiz.Criteria,
                Status = quiz.Status
            };
        }


        // Update an existing quiz
        public async Task<bool> UpdateQuizAsync(UpdateQuizDto dto)
        {
            // Fetch the existing quiz by its ID
            var quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(dto.Id);

            // Check if the quiz exists
            if (quiz == null)
            {
                throw new KeyNotFoundException($"Quiz with Id '{dto.Id}' not found.");
            }

            // Update quiz information
            quiz.QuizName = dto.QuizName; 
            quiz.Criteria = dto.Criteria; 
            quiz.Status = dto.Status; 

            // Update Chapter and Topic references if provided
            if (!string.IsNullOrWhiteSpace(dto.ChapterId))
            {
                var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(dto.ChapterId);
                if (chapter != null)
                {
                    quiz.Chapter = chapter; // Set the Chapter reference
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.TopicId))
            {
                var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(dto.TopicId);
                if (topic != null)
                {
                    quiz.Topic = topic; // Set the Topic reference
                }
            }

            await _unitOfWork.SaveAsync();

            return true; // Indicate success
        }

        // Delete a quiz
        public async Task<bool> DeleteQuizAsync(string quizId)
        {
            // Fetch the existing quiz
            var quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);

            if (quiz == null)
            {
                throw new KeyNotFoundException($"Quiz with Id '{quizId}' not found.");
            }

            // Mark quiz as deleted
            quiz.DeletedBy = "system"; // Or set to the user who deleted it
            await _unitOfWork.SaveAsync(); // Save changes

            return true; // Indicate success
        }
    }
}
