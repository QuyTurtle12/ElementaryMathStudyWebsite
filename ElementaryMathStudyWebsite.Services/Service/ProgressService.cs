using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Store;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ProgressService : IAppProgressServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppSubjectServices _subjectService;
        private readonly IAppQuizServices _quizService;

        // Constructor
        public ProgressService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppSubjectServices subjectService, IAppQuizServices quizService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _subjectService = subjectService;
            _quizService = quizService;
        }

        // Add new progress that student has just assigned to study a subject
        public async Task<bool> AddSubjectProgressAsync(Progress studentProgress)
        {
            await _unitOfWork.GetRepository<Progress>().InsertAsync(studentProgress);
            await _unitOfWork.SaveAsync();
            return true;

        }

        // Get list of finished topic and list of finished chapter of specific student
        private async Task<(IList<FinishedTopic> finishedTopics, IList<FinishedChapter> finishedChapters)> GetFinishedTopicsAndChaptersAsync(string studentId)
        {
            // Get a student progress list
            IQueryable<Progress> progressQuery = _unitOfWork.GetRepository<Progress>().GetEntitiesWithCondition(p => p.StudentId.Equals(studentId));

            var studentProgressInfoList = await progressQuery.ToListAsync();

            IList<FinishedTopic> finishedTopicList = new List<FinishedTopic>();
            IList<FinishedChapter> finishedChapterList = new List<FinishedChapter>();

            // Get list of finished chapters and topics
            foreach (var progress in studentProgressInfoList)
            {
                var finishedTopicInfo = await _unitOfWork.GetRepository<Topic>()
                                                        .Entities
                                                        .Where(t => t.QuizId != null && t.QuizId.Equals(progress.QuizId))
                                                        .Select(t => new { t.Id, t.TopicName }) // Select TopicId and TopicName
                                                        .FirstOrDefaultAsync();

                var finishedChapterInfo = await _unitOfWork.GetRepository<Chapter>()
                                                        .Entities
                                                        .Where(c => c.QuizId != null && c.QuizId.Equals(progress.QuizId))
                                                        .Select(c => new { c.Id, c.ChapterName }) // Select ChapterId and ChapterName
                                                        .FirstOrDefaultAsync();

                // Add topic to finishedTopicList
                if (finishedTopicInfo != null)
                {
                    finishedTopicList.Add(new FinishedTopic
                    {
                        Id = finishedTopicInfo.Id,
                        name = finishedTopicInfo.TopicName
                    });
                }

                // Add chapter to finishedChapterList
                if (finishedChapterInfo != null)
                {
                    finishedChapterList.Add(new FinishedChapter
                    {
                        Id = finishedChapterInfo.Id,
                        name = finishedChapterInfo.ChapterName
                    });
                }
            }

            return (finishedTopicList, finishedChapterList);
        }

        // Get a list of subject progress that one student of one parent currently studying
        public async Task<BasePaginatedList<ProgressViewDto>> GetStudentProgressesDtoAsync(string studentId, int pageNumber, int pageSize)
        {
            // Get all orders have "SUCCESS" status
            IQueryable<string> ordersQuery = _unitOfWork.GetRepository<Order>().Entities
                .Where(o => o.Status == PaymentStatusHelper.SUCCESS.ToString())
                .Select(o => o.Id);

            var allSuccessOrderId = await ordersQuery.ToListAsync();

            // Get list of assigned subject of specific student
            IQueryable<OrderDetail> assignedSubjects = _unitOfWork.GetRepository<OrderDetail>().Entities
                .Where(od => od.StudentId.Equals(studentId) && allSuccessOrderId.Contains(od.OrderId));

            IList<ProgressViewDto> studentProgressDtos = new List<ProgressViewDto>();

            var studentProgresses = await assignedSubjects.ToListAsync();

            foreach (var prog in studentProgresses)
            {
                // Calculate learning progress percentage
                double subjectPercentage = await CalculateSubjectPercentageAsync(studentId, prog.SubjectId);

                // Convert Id to Name
                string studentName = await _userService.GetUserNameAsync(studentId);
                string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);

                // Get student finished chapters and topics list
                var (finishedTopics, finishedChapters) = await GetFinishedTopicsAndChaptersAsync(prog.StudentId);

                ProgressViewDto dto = new()
                {
                    StudentId = studentId,
                    StudentName = studentName,
                    SubjectId = prog.SubjectId,
                    SubjectName = subjectName,
                    SubjectPercentage = subjectPercentage,
                    FinishedTopics = finishedTopics,
                    FinishedChapter = finishedChapters
                };
                studentProgressDtos.Add(dto);
            }

            if (studentProgressDtos.Count == 0)
            {
                throw new BaseException.NotFoundException("not_found", "cannot found children learning progresses");
            }

            // If pageNumber or pageSize are 0 or negative, show all progresses in 1 page
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, studentProgressDtos.Count, 1, studentProgressDtos.Count);
            }

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, studentProgressDtos.Count, pageSize);

            // Show all student progresses with pagination
            return _unitOfWork.GetRepository<ProgressViewDto>().GetPaggingDto(studentProgressDtos, pageNumber, pageSize);
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
                // Get all orders have "SUCCESS" status
                IQueryable<string> ordersQuery = _unitOfWork.GetRepository<Order>().Entities
                    .Where(o => o.Status == PaymentStatusHelper.SUCCESS.ToString())
                    .Select(o => o.Id);

                var allSuccessOrderId = await ordersQuery.ToListAsync();

                // Get list of assigned subject of specific student
                IQueryable<OrderDetail> assignedSubjects = _unitOfWork.GetRepository<OrderDetail>().Entities
                    .Where(od => od.StudentId.Equals(student.Id) && allSuccessOrderId.Contains(od.OrderId));

                var studentProgresses = await assignedSubjects.ToListAsync();
                foreach (var prog in studentProgresses)
                {
                    // Calculate learning progress percentage
                    double subjectPercentage = await CalculateSubjectPercentageAsync(student.Id, prog.SubjectId);

                    // Convert Id to Name
                    string studentName = await _userService.GetUserNameAsync(prog.StudentId);
                    string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);

                    // Get student finished chapters and topics list
                    var (finishedTopics, finishedChapters) = await GetFinishedTopicsAndChaptersAsync(prog.StudentId);

                    ProgressViewDto dto = new()
                    {
                        StudentId = prog.StudentId,
                        StudentName = studentName,
                        SubjectId = prog.SubjectId,
                        SubjectName = subjectName,
                        SubjectPercentage = subjectPercentage,
                        FinishedTopics = finishedTopics,
                        FinishedChapter = finishedChapters
                    };
                    studentProgressDtos.Add(dto);
                }
            }

            if (studentProgressDtos.Count == 0)
            {
                throw new BaseException.NotFoundException("not_found", "cannot found children learning progresses");
            }

            // If pageNumber or pageSize are 0 or negative, show all progresses without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, studentProgressDtos.Count, 1, studentProgressDtos.Count);
            }

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, studentProgressDtos.Count, pageSize);

            // Show paginated progress for all students
            BasePaginatedList<ProgressViewDto> paginatedDtos = _unitOfWork.GetRepository<ProgressViewDto>().GetPaggingDto(studentProgressDtos, pageNumber, pageSize);

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
        public async Task<bool> IsCurrentlyStudyingThisSubjectAsync(string studentId, string subjectId)
        {
            // Get all progresses in database
            // Filter student progresses directly with LINQ
            IQueryable<Progress> query = _unitOfWork.GetRepository<Progress>().Entities
                .Where(p => p.StudentId.Equals(studentId));

            var studentProgresses = await query.ToListAsync();

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

        // Identify which subject does the quiz belong to
        public async Task<string> GetSubjectIdFromQuizIdAsync(string quizId)
        {
            // Get the chapter if the quiz associated with the chapter
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().FindByConditionAsync(c => c.QuizId != null && c.QuizId.Equals(quizId));

            if (chapter is not null)
            {
                // Get subject from chapter id
                Subject? subject = await _unitOfWork.GetRepository<Subject>().FindByConditionAsync(s => s.Id.Equals(chapter.SubjectId));
                return (subject is not null) ? subject.Id : string.Empty; // return subject id if subject is not null
            }

            // Get the topic if the quiz associated with the topic
            Topic? topic = await _unitOfWork.GetRepository<Topic>().FindByConditionAsync(t => t.QuizId != null && t.QuizId.Equals(quizId));

            if (topic is not null)
            {
                // Get the chapter from the topic id
                chapter = await _unitOfWork.GetRepository<Chapter>().FindByConditionAsync(c => c.Id.Equals(topic.ChapterId));

                // Get subject from chapter id
                Subject? subject = (chapter is not null) ? await _unitOfWork.GetRepository<Subject>().FindByConditionAsync(s => s.Id.Equals(chapter.SubjectId)) 
                                                            : null;

                return (subject is not null) ? subject.Id : string.Empty; // return subject id if subject is not null 
            }

            return string.Empty;
        }

        // Validate before perform any tasks
        public async Task<string> IsGenerallyValidatedAsync(string quizId, string studentId)
        {
            string quizName = await _quizService.GetQuizNameAsync(quizId);

            if (string.IsNullOrWhiteSpace(quizName)) return "Invalid quiz Id";

            string subjectId = await GetSubjectIdFromQuizIdAsync(quizId);

            if (!await HasStudentBeenAssignedToTheSubjectAsync(studentId, subjectId)) return "Student has not been assigned to this subject yet";

            return string.Empty;
        }

        // Check if student has been assigned to a specific subject
        public async Task<bool> HasStudentBeenAssignedToTheSubjectAsync(string studentId, string subjectId)
        {
            OrderDetail? orderDetail = await _unitOfWork.GetRepository<OrderDetail>().FindByConditionAsync(od => od.StudentId.Equals(studentId) && od.SubjectId.Equals(subjectId));

            return orderDetail is not null ? true : false;
        }

        // Get a list of assigned subject of specific student
        public async Task<BasePaginatedList<AssignedSubjectDto>?> GetAssignedSubjectListAsync(int pageNumber, int pageSize)
        {

            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            // Get all orders have "SUCCESS" status
            IQueryable<string> ordersQuery = _unitOfWork.GetRepository<Order>().Entities
                .Where(o => o.Status == PaymentStatusHelper.SUCCESS.ToString())
                .Select(o => o.Id);

            var allSuccessOrderId = await ordersQuery.ToListAsync();

            // Get list of assigned subject of specific student
            IQueryable<OrderDetail> orderDetailQuery = _unitOfWork.GetRepository<OrderDetail>().Entities
                .Where( od => od.StudentId.Equals(currentUser.Id) && allSuccessOrderId.Contains(od.OrderId));

            var assignedSubjectList = await orderDetailQuery.ToListAsync();
            IList<AssignedSubjectDto> assignedSubjectDtos = new List<AssignedSubjectDto>();

            foreach (OrderDetail assignedSubject in assignedSubjectList)
            {
                // Convert Id to Name
                string? subjectName = await _subjectService.GetSubjectNameAsync(assignedSubject.SubjectId);

                AssignedSubjectDto assignedSubjectDto = new()
                {
                    SubjectName = subjectName
                };

                assignedSubjectDtos.Add(assignedSubjectDto);
            }

            if (assignedSubjectDtos.Count == 0)
            {
                throw new BaseException.NotFoundException("not_found", "cannot found any assigned subject");
            }

            // Show all assigned subjects in 1 page
            if (pageSize < 0 ||  pageNumber < 0)
            {
                return new BasePaginatedList<AssignedSubjectDto>((IReadOnlyCollection<AssignedSubjectDto>)assignedSubjectDtos, assignedSubjectDtos.Count, 1, assignedSubjectDtos.Count);
            }

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, assignedSubjectDtos.Count, pageSize);

            // Show all assigned subjects with pagination
            return _unitOfWork.GetRepository<AssignedSubjectDto>().GetPaggingDto(assignedSubjectDtos, pageNumber, pageSize);
        }

        // Get a list of subject progress that student currently studying
        public async Task<BasePaginatedList<ProgressViewDto>> GetStudentProgressesDtoForStudentAsync(int pageNumber, int pageSize)
        {
            // Get current logged in user info
            User currentUser = await _userService.GetCurrentUserAsync();

            // Get all orders have "SUCCESS" status
            IQueryable<string> ordersQuery = _unitOfWork.GetRepository<Order>().Entities
                .Where(o => o.Status == PaymentStatusHelper.SUCCESS.ToString())
                .Select(o => o.Id);

            var allSuccessOrderId = await ordersQuery.ToListAsync();

            // Get list of assigned subject of specific student
            IQueryable<OrderDetail> assignedSubjects = _unitOfWork.GetRepository<OrderDetail>().Entities
                .Where(od => od.StudentId.Equals(currentUser.Id) && allSuccessOrderId.Contains(od.OrderId));

            IList<ProgressViewDto> studentProgressDtos = new List<ProgressViewDto>();

            var studentProgresses = await assignedSubjects.ToListAsync();

            foreach (var prog in studentProgresses)
            {
                // Calculate learning progress percentage
                double subjectPercentage = await CalculateSubjectPercentageAsync(currentUser.Id, prog.SubjectId);

                // Convert Id to Name
                string studentName = await _userService.GetUserNameAsync(currentUser.Id);
                string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);

                // Get student finished chapters and topics list
                var (finishedTopics, finishedChapters) = await GetFinishedTopicsAndChaptersAsync(prog.StudentId);

                ProgressViewDto dto = new ProgressViewDto
                {
                    StudentId = currentUser.Id,
                    StudentName = studentName,
                    SubjectId = prog.SubjectId,
                    SubjectName = subjectName,
                    SubjectPercentage = subjectPercentage,
                    FinishedTopics = finishedTopics,
                    FinishedChapter = finishedChapters
                };
                studentProgressDtos.Add(dto);
            }

            if (studentProgressDtos.Count == 0)
            {
                throw new BaseException.NotFoundException("not_found", "cannot found children learning progresses");
            }

            // If pageNumber or pageSize are 0 or negative, show all progresses without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, studentProgressDtos.Count, 1, studentProgressDtos.Count);
            }

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, studentProgressDtos.Count, pageSize);

            // Show all student progresses with pagination
            return _unitOfWork.GetRepository<ProgressViewDto>().GetPaggingDto(studentProgressDtos, pageNumber, pageSize);
        }

    }
}
