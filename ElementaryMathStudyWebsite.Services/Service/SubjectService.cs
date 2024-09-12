using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IGenericRepository<Subject> subjectRepository, IUnitOfWork unitOfWork)
        {
            _subjectRepository = subjectRepository ?? throw new ArgumentNullException(nameof(subjectRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<string> GetSubjectNameAsync(string subjectId)
        {
            Subject? subject = await _subjectRepository.GetByIdAsync(subjectId);

            return subject.SubjectName ?? string.Empty;
        }

        // Check if subject is existed
        public async Task<bool> IsValidSubjectAsync(string subjectId)
        {
            return (await _subjectRepository.GetByIdAsync(subjectId) is not null) ? true : false;
        }
    }
}
