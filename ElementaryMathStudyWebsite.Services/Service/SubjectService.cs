using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Base;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<BasePaginatedList<object>> GetAllSubjectsAsync(int pageNumber, int pageSize, bool isAdmin)
        {
            // Get all subjects from the repository
            IQueryable<Subject> query = _subjectRepository.Entities;

            // If isAdmin is false, filter by Status == true
            if (!isAdmin)
            {
                query = query.Where(s => s.Status == true);
            }

            IList<object> subjectDtos = new List<object>();

            // If pageSize is -1 or if pageNumber/pageSize are 0 or negative, show all subjects without pagination
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allSubjects = await query.ToListAsync(); // Fetch all subjects without pagination

                // Map subjects based on isAdmin
                foreach (var subject in allSubjects)
                {
                    if (isAdmin)
                    {
                        var dto = new
                        {
                            subject.Id,
                            subject.SubjectName,
                            subject.Price,
                            subject.Status,
                            subject.CreatedBy,
                            subject.CreatedTime,
                            subject.LastUpdatedBy,
                            subject.LastUpdatedTime,
                            subject.DeletedBy,
                            subject.DeletedTime
                        };
                        subjectDtos.Add(dto);
                    }
                    else
                    {
                        var dto = new
                        {
                            subject.SubjectName,
                            subject.Price,
                            subject.Status
                        };
                        subjectDtos.Add(dto);
                    }
                }
                return new BasePaginatedList<object>(
                    (IReadOnlyCollection<object>)subjectDtos,
                    subjectDtos.Count,
                    1,
                    subjectDtos.Count // Return total count as "page size" when showing all
                );
            }

            // Apply pagination using GetPagging
            BasePaginatedList<Subject>? paginatedSubjects = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);

            // Map paginated subjects based on isAdmin
            foreach (var subject in paginatedSubjects.Items)
            {
                if (isAdmin)
                {
                    var dto = new
                    {
                        subject.Id,
                        subject.SubjectName,
                        subject.Price,
                        subject.Status,
                        subject.CreatedBy,
                        subject.CreatedTime,
                        subject.LastUpdatedBy,
                        subject.LastUpdatedTime,
                        subject.DeletedBy,
                        subject.DeletedTime
                    };
                    subjectDtos.Add(dto);
                }
                else
                {
                    var dto = new
                    {
                        subject.SubjectName,
                        subject.Price,
                        subject.Status
                    };
                    subjectDtos.Add(dto);
                }
            }

            // Return paginated list of DTOs
            return new BasePaginatedList<object>(
                (IReadOnlyCollection<object>)subjectDtos,
                subjectDtos.Count(),
                pageNumber,
                pageSize
            );
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
                // If the user is not an admin and the subject status is false, treat it as "not found"
                throw new KeyNotFoundException($"Cannot find product with ID '{id}'.");
            }

            if (isAdmin)
            {
                // Return full subject details for admin users
                return new
                {
                    subject.Id,
                    subject.SubjectName,
                    subject.Price,
                    subject.Status,
                    subject.CreatedBy,
                    subject.CreatedTime,
                    subject.LastUpdatedBy,
                    subject.LastUpdatedTime,
                    subject.DeletedBy,
                    subject.DeletedTime
                };
            }
            else
            {
                // Return limited subject details for non-admin users
                return new
                {
                    subject.SubjectName,
                    subject.Price,
                    subject.Status
                };
            }
        }



        // Search subjects by name
        public async Task<BasePaginatedList<object>> SearchSubjectAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _subjectRepository.Entities.Where(s => s.Status == true);

            // Apply search term if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => EF.Functions.Like(s.SubjectName, $"%{searchTerm}%"));
            }

            // If no pagination is requested (pageSize = -1 or invalid pageNumber/pageSize), show all matching subjects
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allSubjects = await query.ToListAsync(); // Fetch all matching subjects
                var subjectDtos = allSubjects.Select(s => new
                {
                    s.SubjectName,
                    s.Price,
                    s.Status
                }).ToList();

                if (!subjectDtos.Any())
                {
                    throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(
                    (IReadOnlyCollection<object>)subjectDtos,
                    subjectDtos.Count,
                    1,
                    subjectDtos.Count // Return the total count as the "page size"
                );
            }

            // Otherwise, apply pagination using GetPagging
            BasePaginatedList<Subject> paginatedSubjects = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);

            var subjectDtosPaginated = paginatedSubjects.Items.Select(s => new
            {
                s.SubjectName,
                s.Price,
                s.Status
            }).ToList();

            if (!subjectDtosPaginated.Any())
            {
                throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
            }

            // Return paginated results
            return new BasePaginatedList<object>(
                (IReadOnlyCollection<object>)subjectDtosPaginated,
                subjectDtosPaginated.Count(),
                pageNumber,
                pageSize
            );
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
