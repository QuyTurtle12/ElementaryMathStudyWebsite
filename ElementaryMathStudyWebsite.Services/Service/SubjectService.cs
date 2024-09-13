using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.IDomainInterface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class SubjectService : ISubjectService, IAppSubjectServices
    {
        private readonly IGenericRepository<Subject> _detailReposiotry;
        private readonly IGenericRepository<Subject> _subjectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IGenericRepository<Subject> detailReposiotry, IGenericRepository<Subject> subjectRepository, IUnitOfWork unitOfWork)
        {
            _detailReposiotry = detailReposiotry ?? throw new ArgumentNullException(nameof(detailReposiotry));
            _subjectRepository = subjectRepository ?? throw new ArgumentNullException(nameof(subjectRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
        public async Task<SubjectAdminViewDTO> CreateSubjectAsync(SubjectDTO subjectDTO)
        {
            ValidateSubject(subjectDTO);

            // Check if another subject with the same name already exists
            var existingSubject = await _subjectRepository.Entities
                .Where(s => s.SubjectName == subjectDTO.SubjectName)
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new InvalidOperationException($"A subject with the name '{subjectDTO.SubjectName}' already exists.");
            }

            var subject = new Subject
            {
                SubjectName = subjectDTO.SubjectName,
                Price = subjectDTO.Price,
                Status = subjectDTO.Status,
                CreatedTime = DateTime.UtcNow,
                LastUpdatedTime = DateTime.UtcNow // Set initial LastUpdatedTime as well
            };

            _subjectRepository.Insert(subject);
            await _subjectRepository.SaveAsync();

            return new SubjectAdminViewDTO
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                Price = subject.Price,
                Status = subject.Status,
                CreatedBy = subject.CreatedBy,
                CreatedTime = subject.CreatedTime,
                LastUpdatedBy = subject.LastUpdatedBy,
                LastUpdatedTime = subject.LastUpdatedTime,
                DeletedBy = subject.DeletedBy,
                DeletedTime = subject.DeletedTime
            };
        }


        // Get all subjects, returning as DTOs
        public async Task<BasePaginatedList<object>> GetAllSubjectsAsync(int pageNumber, int pageSize, bool isAdmin)
        {
            IQueryable<Subject> query = _subjectRepository.Entities;

            if (!isAdmin)
            {
                query = query.Where(s => s.Status == true);
            }

            // Handle pagination
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allSubjects = await query.ToListAsync();
                var subjectDtos = allSubjects.Select(subject => (ISubjectBaseDTO)(isAdmin
                    ? new SubjectAdminViewDTO
                    {
                        Id = subject.Id,
                        SubjectName = subject.SubjectName,
                        Price = subject.Price,
                        Status = subject.Status,
                        CreatedBy = subject.CreatedBy,
                        CreatedTime = subject.CreatedTime,
                        LastUpdatedBy = subject.LastUpdatedBy,
                        LastUpdatedTime = subject.LastUpdatedTime,
                        DeletedBy = subject.DeletedBy,
                        DeletedTime = subject.DeletedTime
                    }
                    : new SubjectDTO
                    {
                        SubjectName = subject.SubjectName,
                        Price = subject.Price,
                        Status = subject.Status
                    })).ToList();

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count(), 1, subjectDtos.Count());
            }

            var paginatedSubjects = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);
            var subjectDtosPaginated = paginatedSubjects.Items.Select(subject => (ISubjectBaseDTO)(isAdmin
                ? new SubjectAdminViewDTO
                {
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status,
                    CreatedBy = subject.CreatedBy,
                    CreatedTime = subject.CreatedTime,
                    LastUpdatedBy = subject.LastUpdatedBy,
                    LastUpdatedTime = subject.LastUpdatedTime,
                    DeletedBy = subject.DeletedBy,
                    DeletedTime = subject.DeletedTime
                }
                : new SubjectDTO
                {
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status
                })).ToList();


            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count(), pageNumber, pageSize);
        }

        // Get a specific subject by ID
        public async Task<object> GetSubjectByIDAsync(string id, bool isAdmin)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Cannot find product with ID '{id}'.");
            }

            if (!isAdmin && !subject.Status)
            {
                throw new KeyNotFoundException($"Cannot find product with ID '{id}'.");
            }

            return isAdmin
                ? new SubjectAdminViewDTO
                {
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status,
                    CreatedBy = subject.CreatedBy,
                    CreatedTime = subject.CreatedTime,
                    LastUpdatedBy = subject.LastUpdatedBy,
                    LastUpdatedTime = subject.LastUpdatedTime,
                    DeletedBy = subject.DeletedBy,
                    DeletedTime = subject.DeletedTime
                }
                : new SubjectDTO
                {
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status
                };
        }

        // Search subjects by name
        public async Task<BasePaginatedList<object>> SearchSubjectAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _subjectRepository.Entities.Where(s => s.Status == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => EF.Functions.Like(s.SubjectName, $"%{searchTerm}%"));
            }

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allSubjects = await query.ToListAsync();
                var subjectDtos = allSubjects.Select(s => new SubjectDTO
                {
                    SubjectName = s.SubjectName,
                    Price = s.Price,
                    Status = s.Status
                }).ToList();

                if (!subjectDtos.Any())
                {
                    throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            var paginatedSubjects = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);
            var subjectDtosPaginated = paginatedSubjects.Items.Select(s => new SubjectDTO
            {
                SubjectName = s.SubjectName,
                Price = s.Price,
                Status = s.Status
            }).ToList();

            if (!subjectDtosPaginated.Any())
            {
                throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count(), pageNumber, pageSize);
        }

        // Update subject and set LastUpdatedTime to current time
        public async Task<SubjectAdminViewDTO> UpdateSubjectAsync(string id, SubjectDTO subjectDTO)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }

            // Check if another subject with the same name already exists
            var existingSubject = await _subjectRepository.Entities
                .Where(s => s.SubjectName == subjectDTO.SubjectName) // Exclude the current subject by its ID
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new InvalidOperationException($"A subject with the name '{subjectDTO.SubjectName}' already exists.");
            }

            ValidateSubject(subjectDTO);

            subject.SubjectName = subjectDTO.SubjectName;
            subject.Price = subjectDTO.Price;
            subject.Status = subjectDTO.Status;
            subject.LastUpdatedTime = DateTime.UtcNow;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveAsync();

            return new SubjectAdminViewDTO
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                Price = subject.Price,
                Status = subject.Status,
                CreatedBy = subject.CreatedBy,
                CreatedTime = subject.CreatedTime,
                LastUpdatedBy = subject.LastUpdatedBy,
                LastUpdatedTime = subject.LastUpdatedTime,
                DeletedBy = subject.DeletedBy,
                DeletedTime = subject.DeletedTime
            };
        }

        // Change subject status and set LastUpdatedTime to current time
        public async Task<SubjectAdminViewDTO> ChangeSubjectStatusAsync(string id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }

            subject.Status = !subject.Status;
            subject.LastUpdatedTime = DateTime.UtcNow;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveAsync();

            return new SubjectAdminViewDTO
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                Price = subject.Price,
                Status = subject.Status,
                CreatedBy = subject.CreatedBy,
                CreatedTime = subject.CreatedTime,
                LastUpdatedBy = subject.LastUpdatedBy,
                LastUpdatedTime = subject.LastUpdatedTime,
                DeletedBy = subject.DeletedBy,
                DeletedTime = subject.DeletedTime
            };
        }

        public async Task<string> GetSubjectNameAsync(string subjectId)
        {
            Subject? subject = await _subjectRepository.GetByIdAsync(subjectId);
            return subject?.SubjectName ?? string.Empty;
        }

        public async Task<bool> IsValidSubjectAsync(string subjectId)
        {
            return await _subjectRepository.GetByIdAsync(subjectId) != null;
        }

        // Get a specific subject by ID (For Order)
        public async Task<Subject> GetSubjectByIDAsync(string id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }
            return subject;
        }
    }
}
