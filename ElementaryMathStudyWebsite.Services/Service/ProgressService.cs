using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ProgressService : IProgressService, IAppProgressServices
    {
        private readonly IGenericRepository<Progress> _progressRepository;
        private readonly IGenericRepository<OrderDetail> _detailRepository;
        private readonly IGenericRepository<Chapter> _chapterRepository;
        private readonly IGenericRepository<Topic> _topicRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<ProgressViewDto> _progressViewDtoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IQuizService _quizService;
        private readonly IAppSubjectServices _subjectService;

        // Constructor
        public ProgressService(IGenericRepository<Progress> progressRepository, IUnitOfWork unitOfWork, IUserService userService, IQuizService quizService, IAppSubjectServices subjectService, IGenericRepository<OrderDetail> detailRepository, IGenericRepository<Chapter> chapterRepository, IGenericRepository<Topic> topicRepository, IGenericRepository<User> userRepository, IGenericRepository<ProgressViewDto> progressViewDtoRepository)
        {
            _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            _subjectService = subjectService ?? throw new ArgumentNullException(nameof(subjectService));
            _detailRepository = detailRepository ?? throw new ArgumentNullException(nameof(detailRepository));
            _chapterRepository = chapterRepository ?? throw new ArgumentNullException(nameof(chapterRepository));
            _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _progressViewDtoRepository = progressViewDtoRepository ?? throw new ArgumentNullException(nameof(progressViewDtoRepository));
        }

        // Add new progress that student has just assigned to study a subject
        public async Task<bool> AddSubjectProgress(Progress studentProgress)
        {
            try
            {
                await _progressRepository.InsertAsync(studentProgress);
                await _unitOfWork.SaveAsync();

                return true;
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
            IQueryable<OrderDetail> assignedSubjects = _detailRepository.Entities
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

                    ProgressViewDto dto = new ProgressViewDto(studentName, subjectName, subjectPercentage);
                    studentProgressDtos.Add(dto);
                }
                return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, studentProgressDtos.Count, 1, studentProgressDtos.Count);
            }

            // Show paginated progress
            BasePaginatedList<OrderDetail>? paginatedProgress = await _detailRepository.GetPagging(assignedSubjects, pageNumber, pageSize);

            foreach (var prog in paginatedProgress.Items)
            {
                double subjectPercentage = await CalculateSubjectPercentageAsync(studentId, prog.SubjectId);
                string studentName = await userAppService.GetUserNameAsync(prog.StudentId);
                string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);
                ProgressViewDto dto = new ProgressViewDto(studentName, subjectName, subjectPercentage);
                studentProgressDtos.Add(dto);
            }

            // Show all student progresses with pagination
            return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, paginatedProgress.TotalItems, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<ProgressViewDto>> GetAllStudentProgressesDtoAsync(string parentId, int pageNumber, int pageSize)
        {
            // Get the list of children of the mentioned parent
            IQueryable<User> students = _userRepository.Entities
                .Where(u => u.CreatedBy.Equals(parentId));

            var studentList = await students.ToListAsync();

            IList<ProgressViewDto> studentProgressDtos = new List<ProgressViewDto>();

            // Cast domain service to application service
            var userAppService = _userService as IAppUserServices;

            // Get list of progress of children
            foreach (var student in studentList)
            {
                // Get all progresses in the database
                // Filter student progresses directly with LINQ
                IQueryable<OrderDetail> assignedSubjects = _detailRepository.Entities
                    .Where(d => d.StudentId.Equals(student.Id));

                var studentProgresses = await assignedSubjects.ToListAsync();
                foreach (var prog in studentProgresses)
                {
                    double subjectPercentage = await CalculateSubjectPercentageAsync(student.Id, prog.SubjectId);
                    string studentName = await userAppService.GetUserNameAsync(prog.StudentId);
                    string subjectName = await _subjectService.GetSubjectNameAsync(prog.SubjectId);

                    ProgressViewDto dto = new ProgressViewDto(studentName, subjectName, subjectPercentage);
                    studentProgressDtos.Add(dto);
                }
            }

            // If pageNumber or pageSize are 0 or negative, show all progresses without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<ProgressViewDto>((IReadOnlyCollection<ProgressViewDto>)studentProgressDtos, studentProgressDtos.Count, 1, studentProgressDtos.Count);
            }

            // Show paginated progress for all students
            BasePaginatedList<ProgressViewDto> paginatedDtos = await _progressViewDtoRepository.GetPaggingDto(studentProgressDtos, pageNumber, pageSize);

            return paginatedDtos;
        }


        // Calculate subject progress
        private async Task<double> CalculateSubjectPercentageAsync(string studentId, string subjectId)
        {
            // Get all progresses in database for the student and subject
            IQueryable<Progress> progressQuery = _progressRepository.Entities
                .Where(p => p.StudentId.Equals(studentId) && p.SubjectId.Equals(subjectId));

            // Get all completed quizzes that student has done
            var completedQuizProgress = await progressQuery.ToListAsync();
            int completedQuizzes = completedQuizProgress.Count;

            // Count chapters for the subject
            int totalChapters = await _chapterRepository.Entities
                .Where(c => c.SubjectId.Equals(subjectId))
                .CountAsync();

            // Get list of chapters for the specific subject
            var chapters = await _chapterRepository.Entities
                .Where(c => c.SubjectId.Equals(subjectId))
                .ToListAsync();

            int totalTopics = 0;

            // Count topics for each chapter
            foreach (var chapter in chapters)
            {
                int topicCount = await _topicRepository.Entities
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
            IQueryable<Progress> query = _progressRepository.Entities
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
    }
}
