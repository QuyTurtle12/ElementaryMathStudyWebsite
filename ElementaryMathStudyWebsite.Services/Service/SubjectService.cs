using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.IDomainInterface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class SubjectService : ISubjectService, IAppSubjectServices
    {
        //private readonly DatabaseContext _context;
        private readonly IGenericRepository<Subject> _subjectRepository;

        public SubjectService(IGenericRepository<Subject> subjectRepository)
        {
            _subjectRepository = subjectRepository ?? throw new ArgumentNullException(nameof(subjectRepository));
        }

        // Helper method for validation
        private void ValidateSubject(SubjectDTO subjectDTO)
        {
            if (string.IsNullOrWhiteSpace(subjectDTO.SubjectName))
            {
                throw new ArgumentException("Subject name is required and cannot be empty.");
            }

            if (subjectDTO.Price == null || subjectDTO.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.");
            }
        }

        // Create a new subject
        public async Task<Subject> CreateSubjectAsync(SubjectDTO subjectDTO)
        {
            // Validate input
            ValidateSubject(subjectDTO);

            var subject = new Subject
            {
                SubjectName = subjectDTO.SubjectName,
                Price = subjectDTO.Price,
                Status = subjectDTO.Status,
            };

            _subjectRepository.Insert(subject);
            await _subjectRepository.SaveAsync();

            return subject;
        }

        // Get all subjects, returning as objects
        public async Task<IEnumerable<object>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetAllAsync();
            return subjects.Select(s => new
            {
                s.SubjectName,
                s.Price,
                s.Status
            }).ToList();
        }

        // Get a specific subject by ID
        public async Task<Subject> GetSubjectByIDAsync(string id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }
            return subject;
        }

        // Search subjects by name
        public async Task<IEnumerable<object>> SearchSubjectAsync(string searchTerm)
        {
            var query = _subjectRepository.Entities.Where(s => s.Status == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => EF.Functions.Like(s.SubjectName, $"%{searchTerm}%"));
            }

            var subjects = await query
                .Select(s => new
                {
                    s.SubjectName,
                    s.Price,
                    s.Status
                })
                .ToListAsync();

            if (!subjects.Any())
            {
                throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
            }

            return subjects;
        }



        // Update subject
        public async Task<Subject> UpdateSubjectAsync(string id, SubjectDTO subjectDTO)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }

            // Validate input
            ValidateSubject(subjectDTO);

            // Update properties
            subject.SubjectName = subjectDTO.SubjectName;
            subject.Price = subjectDTO.Price;
            subject.Status = subjectDTO.Status;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveAsync();
            return subject;
        }


        // Change subject status by ID
        public async Task<Subject> ChangeSubjectStatusAsync(string id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }

            subject.Status = !subject.Status;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveAsync();
            return subject;
        }

        // Get subject name
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
