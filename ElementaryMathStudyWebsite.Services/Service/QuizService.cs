using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuizService : IQuizService, IAppQuizServices
    {
        private readonly IGenericRepository<Quiz> _quizRepository;
        private readonly IGenericRepository<Chapter> _chapterRepository;
        private readonly IGenericRepository<Topic> _topicRepository;

        // constructor
        public QuizService (IGenericRepository<Quiz> QuizRepository)
        {
            _quizRepository = QuizRepository;
        }

        public Task<BasePaginatedList<QuizViewDto?>> GetQuizzDtoAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<QuizViewDto?>> GetQuizzesAsync()
        {
            IQueryable<Quiz> query = _quizRepository.Entities;
            IList<QuizViewDto> listQuiz = new List<QuizViewDto>();
            
            var allQuiz = query.ToList();
            foreach (var quiz  in allQuiz)
            {
                QuizViewDto dto = new QuizViewDto(quiz.QuizName, quiz.Criteria, quiz.Status);
                listQuiz.Add(dto);
            }
            return listQuiz;
        }

        public async Task<QuizDetailsDto?> GetQuizByQuizIdAsync(string quizId)
        {
            Quiz? quiz = await _quizRepository.Entities
                .Include(q => q.Questions)
                .Include(q => q.Chapter)  
                .Include(q => q.Topic)   
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null) return null;

            var quizDetailsDto = new QuizDetailsDto
            {
                QuizName = quiz.QuizName,
                Criteria = quiz.Criteria,
                Chapter = quiz.Chapter != null ? new ChapterDto
                {
                    ChapterName = quiz.Chapter.ChapterName
                } : null,
                Topic = quiz.Topic != null ? new TopicDto
                {
                    TopicName = quiz.Topic.TopicName
                } : null,
                Questions = quiz.Questions?.Select(q => new QuestionDto
                {
                    QuestionContext = q.QuestionContext
                }).ToList() ?? new List<QuestionDto>()
            };

            return quizDetailsDto;
        }

        public async Task<IList<QuizViewDto>> SearchQuizzesAsync(string? quizName, double? criteria)
        {
            IQueryable<Quiz> query = _quizRepository.Entities;

            if (!string.IsNullOrEmpty(quizName))
            {
                query = query.Where(q => q.QuizName.Contains(quizName));
            }

            if (criteria.HasValue)
            {
                query = query.Where(q => q.Criteria == criteria.Value);
            }

            var quizzes = await query.Select(q => new QuizViewDto
            {
                QuizName = q.QuizName,
                Criteria = q.Criteria
            }).ToListAsync();

            return quizzes;
        }

        public async Task<QuizViewDto> AddQuizAsync(QuizCreateDto dto)
        {
            var chapter = await _chapterRepository.GetByIdAsync(dto.ChapterId);
            var topic = await _topicRepository.GetByIdAsync(dto.TopicId);

            if (chapter == null)
            {
                throw new ArgumentException("ChapterId does not exist.");
            }

            if (topic == null)
            {
                throw new ArgumentException("TopicId does not exist.");
            }

            Quiz newQuiz = new Quiz
            {
                QuizName = dto.QuizName,
                Criteria = dto.Criteria,
                Chapter = chapter,
                Topic = topic
            };

            await _quizRepository.InsertAsync(newQuiz);
            await _quizRepository.SaveAsync();

            return new QuizViewDto
            {
                QuizName = newQuiz.QuizName,
                Criteria = newQuiz.Criteria,
                ChapterName = chapter.ChapterName,
                TopicName = topic.TopicName 
            };
        }

        public async Task<bool> DeleteQuizAsync(string quizId)
        {
            if (!Guid.TryParse(quizId, out var quizGuid))
            {
                throw new ArgumentException("QuizId không hợp lệ.");
            }

            var quiz = await _quizRepository.GetByIdAsync(quizGuid);
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz không tồn tại.");
            }

            _quizRepository.DeleteAsync(quiz);
            await _quizRepository.SaveAsync();

            return true;
        }
    }
}
