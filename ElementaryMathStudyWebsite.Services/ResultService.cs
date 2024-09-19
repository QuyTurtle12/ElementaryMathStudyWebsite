using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services
{
    public class ResultService : IAppResultService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Constructor
        public ResultService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Calculate the latest student score
        public async Task<double> CalculateLatestStudentScoreAsync(string quizId, string studentId)
        {
            // Get a list of question base on quiz id
            IQueryable<Question> questionQuery = _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.QuizId.Equals(quizId) && string.IsNullOrWhiteSpace(q.DeletedBy));

            var questionList = await questionQuery.ToListAsync();

            // Count the student's correct answer
            int correctAnswer = 0;
            int totalQuestion = questionList.Count;

            foreach (var question in questionList)
            {
                // Get the student's answer based on student Id and question Id
                IQueryable<UserAnswer>? studentAnswers = _unitOfWork.GetRepository<UserAnswer>().Entities
                    .Where(ua => ua.UserId.Equals(studentId) && ua.QuestionId.Equals(question.Id));

                // Get the list of correct answer of the question
                IQueryable<Option>? correctOption = _unitOfWork.GetRepository<Option>().Entities
                    .Where(o => o.QuestionId.Equals(question.Id) && o.IsCorrect == true && string.IsNullOrWhiteSpace(o.DeletedBy));

                // Check student's answer
                foreach (var userAnswer in studentAnswers)
                {
                    if (userAnswer != null)
                    {
                        // Use for multiple choices and single choice
                        foreach (var option in correctOption)
                        {
                            // Check if the user choice is correct
                            if (userAnswer.OptionId.Equals(option?.Id))
                            {
                                correctAnswer++;
                            }
                        }
                    }
                }
            }

            // Calculate student grade
            // Max grade is 10
            return (correctAnswer / totalQuestion) * 10;
        }

        // Check if the student passed the quiz
        public async Task<bool> IsPassedTheQuizAsync(string quizId, string studentId)
        {
            double studentGrade = await CalculateLatestStudentScoreAsync(quizId, studentId);

            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().FindByConditionAsync(q => q.Id.Equals(quizId));

            // Check if quiz not null and student grade >= quiz criteria 
            if (quiz != null && studentGrade >= quiz.Criteria)
            {
                return true; // Passed
            }

            return false; // Not Passed
        }

        // Get a list of student grade of specific quiz
        public Task<BasePaginatedList<double>> GetStudentResultListAsync(string quizId, string studentId)
        {
            throw new NotImplementedException();
        }
    }
}
