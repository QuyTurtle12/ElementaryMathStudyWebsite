using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ResultService : IAppResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userServices;
        private readonly IAppProgressServices _progressServices;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        // Constructor
        public ResultService(IUnitOfWork unitOfWork, IAppUserServices userServices, IAppProgressServices progressServices, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _userServices = userServices;
            _progressServices = progressServices;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        // Calculate the latest student score
        public async Task<double> CalculateLatestStudentScoreAsync(string quizId, string studentId)
        {
            // Get a list of question base on quiz id
            IQueryable<Question> questionQuery = _unitOfWork.GetRepository<Question>()
                                    .GetEntitiesWithCondition(
                                        q => q.QuizId.Equals(quizId) &&
                                        string.IsNullOrWhiteSpace(q.DeletedBy));

            ICollection<Question> questionList = await questionQuery.ToListAsync();

            if (!questionList.Any())
            {
                throw new BaseException.NotFoundException("not_found", $"The system didn't find any question related to quiz Id {quizId}");
            }

            // Count the student's correct answer
            int correctAnswer = 0;
            int totalQuestion = questionList.Count;

            foreach (Question question in questionList)
            {
                // Query student answers for the specific question and student
                IQueryable<UserAnswer>? studentAnswers = _unitOfWork.GetRepository<UserAnswer>().Entities
                    .Where(ua => ua.UserId.Equals(studentId) && ua.QuestionId.Equals(question.Id));

                // Get the latest attempt number
                int latestAttemptNumber = studentAnswers.Any() ? studentAnswers.Max(ua => ua.AttemptNumber) : 0;

                // Query the user's answers for the latest attempt
                IEnumerable<UserAnswer> latestStudentAnswers = studentAnswers
                    .Where(ua => ua.AttemptNumber == latestAttemptNumber)
                    .ToList();


                // Get the list of correct answer of the question
                IQueryable<Option>? correctOption = _unitOfWork.GetRepository<Option>()
                    .GetEntitiesWithCondition(o =>
                        o.QuestionId.Equals(question.Id) &&
                        o.IsCorrect == true && string.IsNullOrWhiteSpace(o.DeletedBy));

                // Check student's answer
                foreach (UserAnswer userAnswer in latestStudentAnswers)
                {
                    if (userAnswer != null)
                    {
                        // Use for multiple choices and single choice
                        foreach (Option option in correctOption)
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
            double mark = ((double)correctAnswer / totalQuestion) * 10;
            return mark;
        }

        // Check if the student passed the quiz
        public async Task<bool> IsPassedTheQuizAsync(string quizId, string studentId)
        {
            IQueryable<Result> resultQuery = _unitOfWork.GetRepository<Result>().GetEntitiesWithCondition(
                    r => r.QuizId.Equals(quizId) && r.StudentId.Equals(studentId),
                    r => r.Quiz!,
                    r => r.Student!
                );

            IEnumerable<Result> studentResultList = await resultQuery.ToListAsync();

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
                        r => r.QuizId.Equals(quizId) && r.StudentId.Equals(currentUser.Id), // Condition
                        r => r.Quiz!,                                                       // Include Quiz
                        r => r.Student!                                                     // Include Student
                    );

            IEnumerable<Result> studentResultList = await resultQuery.ToListAsync();

            if (!studentResultList.Any())
            {
                throw new BaseException.NotFoundException("not_found", "This student hasn't done any test in this quiz yet");
            }

            ICollection<ResultViewDto> resultViewDtos = studentResultList.Select(result => _mapper.Map<ResultViewDto>(result)).ToList();

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

                result.IsPassedTheQuiz = await IsPassedTheQuizAsync(newStudentProgress.QuizId, newStudentProgress.StudentId);

                if ( result.IsPassedTheQuiz &&
                    await IsAlreadyAddedProgress(newStudentProgress.QuizId, newStudentProgress.StudentId) == false)
                {
                    // Check if progress is added to database successfully
                    await _progressServices.AddSubjectProgressAsync(newStudentProgress);
                }

                return result;
            }
            // Throw errors
            else if (quiz == null) { throw new BaseException.BadRequestException("invalid_quiz", "This quiz is not existed"); }
            else if (user == null) { throw new BaseException.BadRequestException("invalid_user", "This user is not existed"); }
            else if (latestAttemptNumber == 0) { throw new BaseException.BadRequestException("invalid_latest_attempt_number", "This student hasn't done the quiz yet"); }

            return result;
        }

        private async Task<bool> IsAlreadyAddedProgress(string quizId, string studentId) 
        {
            Progress? studentProgress = await _unitOfWork.GetRepository<Progress>().FindByConditionAsync(p => p.QuizId.Equals(quizId) && p.StudentId.Equals(studentId));

            if (studentProgress != null)
            {
                return true;
            }

            return false;
        }

        // Get latest attempt number
        private async Task<int> GetLatestAttemptNumber(string studentId, string quizId)
        {
            // Get a list of question base on quiz id
            Question? question = await _unitOfWork.GetRepository<Question>()
                .FindByConditionAsync(q =>
                    q.QuizId.Equals(quizId) &&
                    string.IsNullOrWhiteSpace(q.DeletedBy));

            if (question != null)
            {
                // Query student answers for the specific question and student
                IQueryable<UserAnswer>? studentAnswers = _unitOfWork.GetRepository<UserAnswer>()
                    .GetEntitiesWithCondition(ua =>
                        ua.UserId.Equals(studentId) &&
                        ua.QuestionId.Equals(question.Id));

                // Get the latest attempt number
                int latestAttemptNumber = studentAnswers.Any() ? studentAnswers.Max(ua => ua.AttemptNumber) : 0;

                return latestAttemptNumber;
            }

            return 0;
        }

        // Get the children latest result of assigned subject
        public async Task<ResultParentViewDto> GetChildrenLatestResultAsync(string studentId)
        {
            // Get the list of result of specific student with related entities (Quiz and Student)
            IQueryable<Result> resultQuery = _unitOfWork.GetRepository<Result>()
                .GetEntitiesWithCondition(
                    r => r.StudentId.Equals(studentId),
                    r => r.Quiz!,  // Include related Quiz entity
                    r => r.Student! // Include related Student entity
                );
            User currentUser = await _userServices.GetCurrentUserAsync();

            if (!await _userServices.IsCustomerChildren(currentUser.Id, studentId))
            {
                throw new BaseException.BadRequestException("invalid_argument", "They are not parent and child relationship");
            }

            // Group the results by QuizId and select the result with the highest AttemptNumber for each quiz
            IEnumerable<Result?> latestResultsForEachQuiz = await resultQuery
                .GroupBy(r => r.QuizId)  // Group by QuizId
                .Select(g => g.OrderByDescending(r => r.AttemptNumber).FirstOrDefault())  // Get the latest attempt per quiz
                .ToListAsync();  // Execute the query and get a list of results

            if (!latestResultsForEachQuiz.Any())
            {
                throw new BaseException.NotFoundException("not_found", "Cannot find this student result");
            }

            // Create a list to hold the results with their subject names
            List<(string SubjectId, string SubjectName, Result Result)> resultWithSubject = (await Task.WhenAll(latestResultsForEachQuiz.Select(async result =>
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    // Resolve services from the new scope
                    IAppProgressServices progressServices = scope.ServiceProvider.GetRequiredService<IAppProgressServices>();
                    IAppSubjectServices subjectServices = scope.ServiceProvider.GetRequiredService<IAppSubjectServices>();

                    // Check null
                    result = result ?? throw new BaseException.NotFoundException("not_found", $"Cannot find this student's quiz result.");

                    // Fetch subject ID from the quiz ID, then fetch the subject name
                    string subjectId = await progressServices.GetSubjectIdFromQuizIdAsync(result.QuizId);
                    string subjectName = await subjectServices.GetSubjectNameAsync(subjectId);
                    return (subjectId, subjectName, result);
                }
            }))).ToList();

            // Get the student latest result
            ResultParentViewDto resultParentViewDto = new ResultParentViewDto
            {
                StudentId = studentId,
                StudentName = latestResultsForEachQuiz.FirstOrDefault()?.Student?.FullName ?? string.Empty,
                subjectResults = resultWithSubject
                .GroupBy(rs => new { rs.SubjectId, rs.SubjectName })  // Group by both SubjectId and SubjectName
                .Select(g => new SubjectResult
                {
                    SubjectId = g.Key.SubjectId,  // Set SubjectId
                    SubjectName = g.Key.SubjectName,  // Set SubjectName
                    resultInfos = g.Select(rs => new ResultInfo
                    {
                        QuizId = rs.Result.QuizId,  // Set QuizId
                        QuizName = rs.Result.Quiz?.QuizName ?? string.Empty,  // Set QuizName
                        Score = rs.Result.Score,  // Set Score
                        DateTaken = rs.Result.DateTaken  // Set DateTaken
                    }).ToList()  // Create a list of ResultInfo for each subject
                }).ToList()
        };

            return resultParentViewDto;
        }

    }
}
