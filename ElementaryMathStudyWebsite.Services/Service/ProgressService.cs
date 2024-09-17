using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ProgressService : IAppProgressServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppSubjectServices _subjectService;

        // Constructor
        public ProgressService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppSubjectServices subjectService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _subjectService = subjectService;
        }

        // Add new progress that student has just assigned to study a subject
        public async Task<bool> AddSubjectProgress(Progress studentProgress)
        {
            try
            {
                if(await IsPassedTheQuiz(studentProgress.QuizId, studentProgress.StudentId))
                {
                    await _unitOfWork.GetRepository<Progress>().InsertAsync(studentProgress);
                    await _unitOfWork.SaveAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get a list of subject progress that student currently studying
        public async Task<BasePaginatedList<ProgressViewDto>> GetStudentProgressesDtoAsync(string studentId, int pageNumber, int pageSize)
        {
            // Get all progresses in database
            // Filter student progresses directly with LINQ
            IQueryable<OrderDetail> assignedSubjects = _unitOfWork.GetRepository<OrderDetail>().Entities
                .Where(d => d.StudentId.Equals(studentId));

            IList<ProgressViewDto> studentProgressDtos = new List<ProgressViewDto>();

            // Cast domain service to application service
            var userAppService = _userService as IAppUserServices;


            // If pageNumber or pageSize are 0 or negative, show all progresses without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var studentProgresses = await assignedSubjects.ToListAsync();

                foreach (var prog in studentProgresses)
                {
                    double subjectPercentage = await CalculateSubjectPercentageAsync(studentId, prog.SubjectId);
                    string studentName = await userAppService.GetUserNameAsync(prog.StudentId);
                    string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);

                    ProgressViewDto dto = new ProgressViewDto { StudentName = studentName, SubjectName = subjectName, SubjectPercentage = subjectPercentage };
                    studentProgressDtos.Add(dto);
                }
                return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, studentProgressDtos.Count, 1, studentProgressDtos.Count);
            }

            // Show paginated progress
            BasePaginatedList<OrderDetail>? paginatedProgress = await _unitOfWork.GetRepository<OrderDetail>().GetPagging(assignedSubjects, pageNumber, pageSize);

            foreach (var prog in paginatedProgress.Items)
            {
                double subjectPercentage = await CalculateSubjectPercentageAsync(studentId, prog.SubjectId);
                string studentName = await userAppService.GetUserNameAsync(prog.StudentId);
                string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);
                ProgressViewDto dto = new ProgressViewDto { StudentName = studentName, SubjectName = subjectName, SubjectPercentage = subjectPercentage };
                studentProgressDtos.Add(dto);
            }

            // Show all student progresses with pagination
            return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, paginatedProgress.TotalItems, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<ProgressViewDto>> GetAllStudentProgressesDtoAsync(string parentId, int pageNumber, int pageSize)
        {
            // Get the list of children of the mentioned parent
            IQueryable<User> students = _unitOfWork.GetRepository<User>().Entities
                .Where(u => parentId == null // Check if parent id is null
                            ? u.CreatedBy == null // Get all users that have CreatedBy is null
                            : u.CreatedBy != null && u.CreatedBy.Equals(parentId)); // Get all users that have CreatedBy is not null
                                                                                    // and CreatedBy is equal parent id

            var studentList = await students.ToListAsync();

            IList<ProgressViewDto> studentProgressDtos = new List<ProgressViewDto>();

            // Get list of progress of children
            foreach (var student in studentList)
            {
                // Get all progresses in the database
                // Filter student progresses directly with LINQ
                IQueryable<OrderDetail> assignedSubjects = _unitOfWork.GetRepository<OrderDetail>().Entities
                    .Where(d => d.StudentId.Equals(student.Id));

                var studentProgresses = await assignedSubjects.ToListAsync();
                foreach (var prog in studentProgresses)
                {
                    double subjectPercentage = await CalculateSubjectPercentageAsync(student.Id, prog.SubjectId);
                    string studentName = await _userService.GetUserNameAsync(prog.StudentId);
                    string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);

                    ProgressViewDto dto = new() { StudentName = studentName, SubjectName = subjectName, SubjectPercentage = subjectPercentage };
                    studentProgressDtos.Add(dto);
                }
            }

            // If pageNumber or pageSize are 0 or negative, show all progresses without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, studentProgressDtos.Count, 1, studentProgressDtos.Count);
            }

            // Show paginated progress for all students
            BasePaginatedList<ProgressViewDto> paginatedDtos = await _unitOfWork.GetRepository<ProgressViewDto>().GetPaggingDto(studentProgressDtos, pageNumber, pageSize);

            return paginatedDtos;
        }


        // Calculate subject progress
        private async Task<double> CalculateSubjectPercentageAsync(string studentId, string subjectId)
        {
            // Get all progresses in database for the student and subject
            IQueryable<Progress> progressQuery = _unitOfWork.GetRepository<Progress>().Entities
                .Where(p => p.StudentId.Equals(studentId) && p.SubjectId.Equals(subjectId));

            // Get all completed quizzes that student has done
            var completedQuizProgress = await progressQuery.ToListAsync();
            int completedQuizzes = completedQuizProgress.Count;

            // Count chapters for the subject
            int totalChapters = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.SubjectId.Equals(subjectId))
                .CountAsync();

            // Get list of chapters for the specific subject
            var chapters = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.SubjectId.Equals(subjectId))
                .ToListAsync();

            int totalTopics = 0;

            // Count topics for each chapter
            foreach (var chapter in chapters)
            {
                int topicCount = await _unitOfWork.GetRepository<Topic>().Entities
                    .Where(t => t.ChapterId.Equals(chapter.Id))
                    .CountAsync();

                totalTopics += topicCount;
            }

            // Each chapter and each topic is associated with 1 quiz
            int totalQuizzes = totalTopics + totalChapters;

            // Calculate percentage and round to 2 decimal places
            double subjectPercentage = totalQuizzes > 0 ? Math.Round((double)completedQuizzes / totalQuizzes * 100, 2) : 0;

            return subjectPercentage;
        }

        // Check if student is currently studying a specific subjcet
        public bool IsCurrentlyStudyingThisSubject(string studentId, string subjectId)
        {
            // Get all progresses in database
            // Filter student progresses directly with LINQ
            IQueryable<Progress> query = _unitOfWork.GetRepository<Progress>().Entities
                .Where(p => p.StudentId.Equals(studentId));

            var studentProgresses = query.ToList();

            // Validation process
            foreach (var progress in studentProgresses)
            {
                if (progress.SubjectId.Equals(subjectId))
                {
                    return true;
                }
            }

            return false;

        }

        // Update student learning progress
        public async Task<double> GetStudentGrade(string quizId, string studentId)
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
        public async Task<bool> IsPassedTheQuiz(string quizId, string studentId)
        {
            double studentGrade = await GetStudentGrade(quizId, studentId);

            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().Entities.FirstOrDefaultAsync(q => q.Id.Equals(quizId));

            // Check if quiz not null and student grade >= quiz criteria 
            if (quiz != null && studentGrade >= quiz.Criteria)
            {
                return true; // Passed
            }

            return false; // Not Passed
        }
    }
}
