using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ResultService : IAppResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userServices;
        private readonly IAppProgressServices _progressServices;
        private readonly IAppSubjectServices _subjectServices;

        // Constructor
        public ResultService(IUnitOfWork unitOfWork, IAppUserServices userServices, IAppProgressServices progressServices, IAppSubjectServices subjectServices)
        {
            _unitOfWork = unitOfWork;
            _userServices = userServices;
            _progressServices = progressServices;
            _subjectServices = subjectServices;
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
                // Query student answers for the specific question and student
                IQueryable<UserAnswer>? studentAnswers = _unitOfWork.GetRepository<UserAnswer>().Entities
                    .Where(ua => ua.UserId.Equals(studentId) && ua.QuestionId.Equals(question.Id));

                // Get the latest attempt number
                int latestAttemptNumber = studentAnswers.Any() ? studentAnswers.Max(ua => ua.AttemptNumber) : 0;

                // Query the user's answers for the latest attempt
                List<UserAnswer> latestStudentAnswers = studentAnswers
                    .Where(ua => ua.AttemptNumber == latestAttemptNumber)
                    .ToList();


                // Get the list of correct answer of the question
                IQueryable<Option>? correctOption = _unitOfWork.GetRepository<Option>().Entities
                    .Where(o => o.QuestionId.Equals(question.Id) && o.IsCorrect == true && string.IsNullOrWhiteSpace(o.DeletedBy));

                // Check student's answer
                foreach (var userAnswer in latestStudentAnswers)
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
            return correctAnswer / totalQuestion * 10;
        }

        // Check if the student passed the quiz
        public async Task<bool> IsPassedTheQuizAsync(string quizId, string studentId)
        {
            IQueryable<Result> resultQuery = _unitOfWork.GetRepository<Result>().GetEntitiesWithCondition(
                    r => r.QuizId.Equals(quizId) && r.StudentId.Equals(studentId),
                    r => r.Quiz,
                    r => r.Student
                );

            var studentResultList = await resultQuery.ToListAsync();

            // Get the latest attempt number
            int latestAttemptNumber = studentResultList.Any() ? studentResultList.Max(r => r.AttemptNumber) : 0;

            // Query the latest result
            Result? latestStudentResult = studentResultList
                .Where(r => r.AttemptNumber == latestAttemptNumber)
                .FirstOrDefault();

            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().FindByConditionAsync(q => q.Id.Equals(quizId));

            // Check if quiz not null and student grade >= quiz criteria 
            if (quiz != null && latestStudentResult?.Score >= quiz.Criteria)
            {
                return true; // Passed
            }

            return false; // Not Passed
        }

        // Get a list of student grade of specific quiz
        public async Task<BasePaginatedList<ResultViewDto>> GetStudentResultListAsync(string quizId, int pageNumber, int pageSize)
        {
            // Get current logged in user info
            User currentUser = await _userServices.GetCurrentUserAsync();

            // Get student results from database
            IQueryable<Result> resultQuery = _unitOfWork.GetRepository<Result>().GetEntitiesWithCondition(
                        r => r.QuizId.Equals(quizId) && r.StudentId.Equals(currentUser.Id),
                        r => r.Quiz,
                        r => r.Student
                    );

            var studentResultList = await resultQuery.ToListAsync();

            IList<ResultViewDto> resultViewDtos = new List<ResultViewDto>();

            // Map result data to view dto
            foreach (var result in studentResultList)
            {
                ResultViewDto dto = new()
                {
                    StudentName = result.Student.FullName,
                    QuizName = result.Quiz.QuizName,
                    Score = result.Score,
                    Attempt = result.AttemptNumber,
                    DateTaken = result.DateTaken
                };

                resultViewDtos.Add(dto);
            }

            // Show all student's results in 1 page
            if (pageNumber < 0 || pageSize < 0)
            {
                return new BasePaginatedList<ResultViewDto>((IReadOnlyCollection<ResultViewDto>)resultViewDtos, resultViewDtos.Count, 1, resultViewDtos.Count);
            }

            // Show all student's results with pagination
            return _unitOfWork.GetRepository<ResultViewDto>().GetPaggingDto(resultViewDtos, pageNumber, pageSize);
        }

        // Add student result to database
        public async Task<ResultProgressDto> AddStudentResultAsync(ResultCreateDto dto)
        {
            ResultProgressDto result = new ResultProgressDto();

            User student = await _userServices.GetCurrentUserAsync();

            int latestAttemptNumber = await GetLatestAttemptNumber(student.Id, dto.QuizId);
            double Score = await CalculateLatestStudentScoreAsync(dto.QuizId, student.Id);

            // Get entities data from database
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().FindByConditionAsync(q => q.Id.Equals(dto.QuizId) && string.IsNullOrWhiteSpace(q.DeletedBy));
            User? user = await _unitOfWork.GetRepository<User>().FindByConditionAsync(u => u.Id.Equals(student.Id) && string.IsNullOrWhiteSpace(u.DeletedBy));

            if (quiz != null && user != null && latestAttemptNumber != 0)
            {
                Result studentResult = new Result
                {
                    QuizId = dto.QuizId,
                    StudentId = student.Id,
                    AttemptNumber = latestAttemptNumber,
                    Score = Score,
                    DateTaken = CoreHelper.SystemTimeNow,
                    Quiz = quiz,
                    Student = user

                };

                await _unitOfWork.GetRepository<Result>().InsertAsync(studentResult);
                await _unitOfWork.GetRepository<Result>().SaveAsync();

                // Added result to database successfully
                result.IsAddedResult = true;

                // Identify subject id using quiz id 
                string subjectId = await _progressServices.GetSubjectIdFromQuizIdAsync(studentResult.QuizId);

                // Update student progress
                Progress newStudentProgress = new()
                {
                    StudentId = studentResult.StudentId,
                    QuizId = studentResult.QuizId,
                    SubjectId = subjectId,
                };

                if (await IsPassedTheQuizAsync(newStudentProgress.QuizId, newStudentProgress.StudentId))
                {
                    // Check if progress is added to database successfully
                    result.IsAddedProgress = await _progressServices.AddSubjectProgressAsync(newStudentProgress);
                }

                return result;
            }
            // Throw errors
            else if (quiz == null) { throw new BaseException.BadRequestException("invalid_quiz", "This quiz is not existed"); }
            else if (user == null) { throw new BaseException.BadRequestException("invalid_user", "This user is not existed"); }
            else if (latestAttemptNumber == 0) { throw new BaseException.BadRequestException("invalid_latest_attempt_number", "This student hasn't done the quiz yet"); }

            return result;
        }

        // Get latest attempt number
        private async Task<int> GetLatestAttemptNumber(string studentId, string quizId)
        {
            // Get a list of question base on quiz id
            Question? question = await _unitOfWork.GetRepository<Question>().FindByConditionAsync(q => q.QuizId.Equals(quizId) && string.IsNullOrWhiteSpace(q.DeletedBy));

            if (question != null)
            {
                // Query student answers for the specific question and student
                IQueryable<UserAnswer>? studentAnswers = _unitOfWork.GetRepository<UserAnswer>().Entities
                    .Where(ua => ua.UserId.Equals(studentId) && ua.QuestionId.Equals(question.Id));

                // Get the latest attempt number
                int latestAttemptNumber = studentAnswers.Any() ? studentAnswers.Max(ua => ua.AttemptNumber) : 0;

                return latestAttemptNumber;
            }

            return 0;
        }

        // Get the children latest result of assigned subject
        public async Task<ResultParentViewDto> GetChildrenLatestResultAsync(string studentId)
        {
            var currentUser = await _userServices.GetCurrentUserAsync();

            if (!await _userServices.IsCustomerChildren(currentUser.Id, studentId))
            {
                throw new BaseException.BadRequestException("bad_request", "They are not parents and child relationship");
            }

            // Get the list of result of specific student
            IQueryable<Result> resultQuery = _unitOfWork.GetRepository<Result>()
                .GetEntitiesWithCondition(
                r => r.StudentId.Equals(studentId),
                r => r.Quiz,
                r => r.Student
                );

            // Group the results by QuizId and select the result with the highest AttemptNumber for each quiz
            var latestResultsForEachQuiz = await resultQuery
                .GroupBy(r => r.QuizId) // Group by QuizId
                .Select(g => g.OrderByDescending(r => r.AttemptNumber).FirstOrDefault()) // Select the result with the highest AttemptNumber
                .ToListAsync();  // Convert the query into a list

            // Create a list to hold the results with their subject names
            var resultWithSubject = new List<(string SubjectName, Result Result)>();

            foreach (var result in latestResultsForEachQuiz)
            {
                if (result != null)
                {
                    // Fetch subject ID from the quiz ID, then fetch the subject name
                    var subjectId = await _progressServices.GetSubjectIdFromQuizIdAsync(result.QuizId);
                    var subjectName = await _subjectServices.GetSubjectNameAsync(subjectId);

                    // Add the result along with its subject name to the list
                    resultWithSubject.Add((subjectName, result));
                }
            }

            // Group the results by subject name in-memory
            var groupedResultsBySubject = resultWithSubject
                .GroupBy(rs => rs.SubjectName)
                .Select(g => new SubjectResult
                {
                    SubjectName = g.Key,  // Group key is the subject name
                    resultInfos = g.Select(rs => new ResultInfo
                    {
                        QuizName = rs.Result.Quiz.QuizName,  // Set quiz name
                        Score = rs.Result.Score,             // Set score
                        DateTaken = rs.Result.DateTaken      // Set date taken
                    }).ToList()  // Create a list of ResultInfo for each subject
                }).ToList();

            // Create the parent view DTO
            var resultParentViewDto = new ResultParentViewDto
            {
                StudentName = latestResultsForEachQuiz.FirstOrDefault()?.Student?.FullName ?? string.Empty,
                subjectResults = groupedResultsBySubject
            };

            return resultParentViewDto;
        }
    }
}
