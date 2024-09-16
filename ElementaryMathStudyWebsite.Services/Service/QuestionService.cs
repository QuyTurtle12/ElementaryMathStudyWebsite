using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class QuestionService : IQuestionService, IAppQuestionServices
    {
        public readonly IGenericRepository<Question> _questionRepository;

        public QuestionService(IGenericRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<QuestionViewDto> CreateQuestionAsync(CreateQuestionDto dto)
        {
            var question = new Question
            {
                QuestionContext = dto.QuestionContext,
                QuizId = dto.QuizId
            };

            await _questionRepository.InsertAsync(question);
            return new QuestionViewDto
            {
                Id = question.Id,
                QuestionContext = question.QuestionContext,
                QuizId = question.QuizId
            };
        }
        // Get a question by ID
        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id.ToString()); // Assuming ID is a string in repo
            if (question == null) throw new Exception("Question not found");
            return question;
        }

        // Get all questions
        public async Task<IList<QuestionViewDto>> GetAllQuestionsAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            return questions.Select(q => new QuestionViewDto
            {
                QuestionContext = q.QuestionContext,
                QuizId = q.QuizId,
            }).ToList();
        }

        // Update a question by ID
        public async Task<bool> UpdateQuestionAsync(int id, UpdateQuestionDto dto)
        {
            var question = await _questionRepository.GetByIdAsync(id.ToString()); // Assuming ID is a string in repo
            if (question == null) throw new Exception("Question not found");

            if (!string.IsNullOrEmpty(dto.QuestionContext))
            {
                question.QuestionContext = dto.QuestionContext;
            
            }

            await _questionRepository.UpdateAsync(question);
            return true;
        }

        // Delete a question by ID
        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id.ToString()); // Assuming ID is a string in repo
            if (question == null) throw new Exception("Question not found");

            await _questionRepository.DeleteAsync(question);
            return true;
        }
    }
}
