using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using ElementaryMathStudyWebsite.Infrastructure.UOW;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class SubjectService(IUnitOfWork unitOfWork, IAppUserServices userServices, IGenericRepository<Subject> detailRepository) : IAppSubjectServices
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IAppUserServices _userServices = userServices ?? throw new ArgumentNullException(nameof(userServices));
        private readonly IGenericRepository<Subject> _detailReposiotry = detailRepository;

        // Helper method for validation
        private static void ValidateSubject(SubjectDTO subjectDTO)
        {
            if (string.IsNullOrWhiteSpace(subjectDTO.SubjectName))
            {
                throw new ArgumentException("Subject name is required and cannot be empty.");
            }

            if (subjectDTO.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.");
            }
        }

        // Create a new subject
        public async Task<SubjectAdminViewDTO> CreateSubjectAsync(SubjectDTO subjectDTO)
        {
            ValidateSubject(subjectDTO);

            // Check if another subject with the same name already exists
            var existingSubject = await _unitOfWork.GetRepository<Subject>().Entities
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
                //CreatedTime = DateTime.UtcNow,
                //LastUpdatedTime = DateTime.UtcNow // Set initial LastUpdatedTime as well
            };

            AuditFields(subject, isCreating: true);

            _unitOfWork.GetRepository<Subject>().Insert(subject);
            await _unitOfWork.GetRepository<Subject>().SaveAsync();

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
            IQueryable<Subject> query = _unitOfWork.GetRepository<Subject>().Entities;

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
                        Id = subject.Id,
                        SubjectName = subject.SubjectName,
                        Price = subject.Price,
                        Status = subject.Status
                    })).ToList();

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
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
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status
                })).ToList();


            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        // Get a specific subject by ID
        public async Task<object> GetSubjectByIDAsync(string id, bool isAdmin)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"Cannot find product with ID '{id}'.");
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
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status
                };
        }

        // Search subjects by name
        public async Task<BasePaginatedList<object>> SearchSubjectAsync(string searchTerm, double lowestPrice,
                    double highestPrice, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Subject>().Entities.Where(s => s.Status == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => EF.Functions.Like(s.SubjectName, $"%{searchTerm}%"));
            }

            // Search by price range
            if (lowestPrice >= 0)
            {
                query = query.Where(p => p.Price >= lowestPrice);
            }
            if (highestPrice >= 0)
            {
                query = query.Where(p => p.Price <= highestPrice);
            }

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allSubjects = await query.ToListAsync();
                var subjectDtos = allSubjects.Select(s => new SubjectDTO
                {
                    Id = s.Id,
                    SubjectName = s.SubjectName,
                    Price = s.Price,
                    Status = s.Status
                }).ToList();

                if (subjectDtos.Count == 0)
                {
                    throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            var paginatedSubjects = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);
            var subjectDtosPaginated = paginatedSubjects.Items.Select(s => new SubjectDTO
            {
                Id = s.Id,
                SubjectName = s.SubjectName,
                Price = s.Price,
                Status = s.Status
            }).ToList();

            if (subjectDtosPaginated.Count == 0)
            {
                throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<object>> SearchSubjectAdminAsync(string searchTerm, double lowestPrice,
                    double highestPrice, bool? status, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Subject>().Entities.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => EF.Functions.Like(s.SubjectName, $"%{searchTerm}%"));
            }

            // Search by price range
            if (lowestPrice >= 0)
            {
                query = query.Where(p => p.Price >= lowestPrice);
            }
            if (highestPrice >= 0)
            {
                query = query.Where(p => p.Price <= highestPrice);
            }

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allSubjects = await query.ToListAsync();
                var subjectDtos = allSubjects.Select(subject => new SubjectAdminViewDTO
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
                }).ToList();

                if (subjectDtos.Count == 0)
                {
                    throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            var paginatedSubjects = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);
            var subjectDtosPaginated = paginatedSubjects.Items.Select(subject => new SubjectAdminViewDTO
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
            }).ToList();

            if (subjectDtosPaginated.Count == 0)
            {
                throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        // Update subject and set LastUpdatedTime to current time
        public async Task<SubjectAdminViewDTO> UpdateSubjectAsync(string id, SubjectDTO subjectDTO)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"Subject with ID '{id}' not found.");

            // Check if another subject with the same name already exists
            var existingSubject = await _unitOfWork.GetRepository<Subject>().Entities
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

            AuditFields(subject, isCreating: false);

            _unitOfWork.GetRepository<Subject>().Update(subject);
            await _unitOfWork.SaveAsync();

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
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            subject.Status = !subject.Status;
            //subject.LastUpdatedTime = DateTime.UtcNow;

            AuditFields(subject);

            _unitOfWork.GetRepository<Subject>().Update(subject);
            await _unitOfWork.GetRepository<Subject>().SaveAsync();

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
            Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(subjectId);
            return subject?.SubjectName ?? string.Empty;
        }

        public async Task<bool> IsValidSubjectAsync(string subjectId)
        {
            return await _unitOfWork.GetRepository<Subject>().GetByIdAsync(subjectId) != null;
        }

        // Get a specific subject by ID (For Order)
        public async Task<Subject> GetSubjectByIDAsync(string id)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            return subject;
        }

        public async void AuditFields(BaseEntity entity, bool isCreating = false)
        {
            // Get current logged in user info
            User currentUser = await _userServices.GetCurrentUserAsync();
            var currentUserId = currentUser.Id;

            // If creating a new entity, set the CreatedBy field
            if (isCreating)
            {
                entity.CreatedBy = currentUserId.ToString().ToUpper(); // Set the creator's ID
            }

            // Always set LastUpdatedBy and LastUpdatedTime fields
            entity.LastUpdatedBy = currentUserId.ToString().ToUpper(); // Set the current user's ID

            // If is not created then update LastUpdatedTime
            if (isCreating is false)
            {
                entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            }

        }

        public async Task<bool> CheckCompleteQuizExistAsync(string subjectId, string quizId)
        {
            // Get current logged in user info
            User currentUser = await _userServices.GetCurrentUserAsync();
            var currentUserId = currentUser.Id;

            // Query the Progress table for a record with the specified SubjectId and QuizId
            var progressRecord = await _unitOfWork.GetRepository<Progress>().Entities
                .Where(p => p.StudentId.Equals(currentUserId.ToString(), StringComparison.CurrentCultureIgnoreCase) && p.SubjectId == subjectId && p.QuizId == quizId)
                .FirstOrDefaultAsync();

            // If the record is found, return true; otherwise, return false
            return progressRecord != null;
        }
    }
}
