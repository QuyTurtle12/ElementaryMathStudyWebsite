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
    public class SubjectService : IAppSubjectServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;

        public SubjectService(IUnitOfWork unitOfWork, IAppUserServices userService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService;
        }

        // Helper method for validation
        private static void ValidateSubject(SubjectDTO subjectDTO)
        {
            if (string.IsNullOrWhiteSpace(subjectDTO.SubjectName))
            {
                throw new BaseException.BadRequestException("empty_subject_name", "Subject name is required and cannot be empty.");
            }

            if (subjectDTO.Price <= 0)
            {
                throw new BaseException.BadRequestException("price_range_error", "Price must be greater than zero.");
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
                throw new BaseException.BadRequestException("duplicate_name", $"A subject with the name '{subjectDTO.SubjectName}' already exists.");
            }

            var subject = new Subject
            {
                SubjectName = subjectDTO.SubjectName,
                Price = subjectDTO.Price,
                Status = subjectDTO.Status,
            };

            AuditFields(subject, isCreating: true);

            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.SaveAsync();

            User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
            User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

            return new SubjectAdminViewDTO
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                Price = subject.Price,
                Status = subject.Status,
                CreatedBy = subject.CreatedBy,
                CreaterName = createdUser?.FullName,
                CreaterPhone = createdUser?.PhoneNumber,
                CreatedTime = subject.CreatedTime,
                LastUpdatedBy = subject.LastUpdatedBy,
                UpdaterName = createdUser?.FullName,
                UpdaterPhone = createdUser?.PhoneNumber,
                LastUpdatedTime = subject.LastUpdatedTime
            };
        }


        // Get all subjects, returning as DTOs
        public async Task<BasePaginatedList<object>> GetAllSubjectsAsync(int pageNumber, int pageSize, bool isAdmin)
        {
            IQueryable<Subject> query = _unitOfWork.GetRepository<Subject>().Entities;

            //Not get soft deleted item
            query = query.Where(s => String.IsNullOrWhiteSpace(s.DeletedBy));

            if (!isAdmin)
            {
                query = query.Where(s => s.Status == true);
            }

            // Handle pagination
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allSubjects = await query.ToListAsync();
                var subjectDtos = allSubjects.Select(subject =>
                {
                    if (isAdmin)
                    {
                        // Fetch the created and updated users
                        User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                        User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                        return (ISubjectBaseDTO)new SubjectAdminViewDTO
                        {
                            Id = subject.Id,
                            SubjectName = subject.SubjectName,
                            Price = subject.Price,
                            Status = subject.Status,
                            CreatedBy = subject.CreatedBy,
                            CreaterName = createdUser?.FullName,
                            CreaterPhone = createdUser?.PhoneNumber,
                            CreatedTime = subject.CreatedTime,
                            LastUpdatedBy = subject.LastUpdatedBy,
                            UpdaterName = updatedUser?.FullName,
                            UpdaterPhone = updatedUser?.PhoneNumber,
                            LastUpdatedTime = subject.LastUpdatedTime
                        };
                    }
                    else
                    {
                        return (ISubjectBaseDTO)new SubjectDTO
                        {
                            Id = subject.Id,
                            SubjectName = subject.SubjectName,
                            Price = subject.Price,
                            Status = subject.Status
                        };
                    }
                }).ToList();

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            var paginatedSubjects = await _unitOfWork.GetRepository<Subject>().GetPagging(query, pageNumber, pageSize);
            var subjectDtosPaginated = paginatedSubjects.Items.Select(subject =>
            {
                if (isAdmin)
                {
                    // Fetch the created and updated users
                    User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                    User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                    return (ISubjectBaseDTO)new SubjectAdminViewDTO
                    {
                        Id = subject.Id,
                        SubjectName = subject.SubjectName,
                        Price = subject.Price,
                        Status = subject.Status,
                        CreatedBy = subject.CreatedBy,
                        CreaterName = createdUser?.FullName,
                        CreaterPhone = createdUser?.PhoneNumber,
                        CreatedTime = subject.CreatedTime,
                        LastUpdatedBy = subject.LastUpdatedBy,
                        UpdaterName = updatedUser?.FullName,
                        UpdaterPhone = updatedUser?.PhoneNumber,
                        LastUpdatedTime = subject.LastUpdatedTime
                    };
                }
                else
                {
                    return (ISubjectBaseDTO)new SubjectDTO
                    {
                        Id = subject.Id,
                        SubjectName = subject.SubjectName,
                        Price = subject.Price,
                        Status = subject.Status
                    };
                }
            }).ToList();


            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        // Get a specific subject by ID
        public async Task<object> GetSubjectByIDAsync(string id, bool isAdmin)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("key_not_found", $"Cannot find product with ID '{id}'.");
            if ((!isAdmin && !subject.Status) || !String.IsNullOrWhiteSpace(subject.DeletedBy))
            {
                throw new BaseException.BadRequestException("key_not_found", $"Cannot find product with ID '{id}'.");
            }

            if (isAdmin)
            {
                User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                return new SubjectAdminViewDTO
                {
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status,
                    CreatedBy = subject.CreatedBy,
                    CreaterName = createdUser?.FullName,
                    CreaterPhone = createdUser?.PhoneNumber,
                    CreatedTime = subject.CreatedTime,
                    LastUpdatedBy = subject.LastUpdatedBy,
                    UpdaterName = createdUser?.FullName,
                    UpdaterPhone = createdUser?.PhoneNumber,
                    LastUpdatedTime = subject.LastUpdatedTime
                };
            }
            else
            {
                return new SubjectDTO
                {
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status
                };
            }
        }

        // Search subjects by name
        public async Task<BasePaginatedList<object>> SearchSubjectAsync(string searchTerm, double lowestPrice,
                    double highestPrice, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Subject>().Entities.Where(s => s.Status == true);

            //Not get soft deleted item
            query = query.Where(s => String.IsNullOrWhiteSpace(s.DeletedBy));

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
                    throw new BaseException.BadRequestException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            var paginatedSubjects = await _unitOfWork.GetRepository<Subject>().GetPagging(query, pageNumber, pageSize);
            var subjectDtosPaginated = paginatedSubjects.Items.Select(s => new SubjectDTO
            {
                Id = s.Id,
                SubjectName = s.SubjectName,
                Price = s.Price,
                Status = s.Status
            }).ToList();

            if (subjectDtosPaginated.Count == 0)
            {
                throw new BaseException.BadRequestException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<object>> SearchSubjectAdminAsync(string searchTerm, double lowestPrice,
                    double highestPrice, bool? status, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Subject>().Entities.AsQueryable();

            //Not get soft deleted item
            query = query.Where(s => String.IsNullOrWhiteSpace(s.DeletedBy));

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
                var subjectDtos = allSubjects.Select(subject =>
                {
                    // Fetch the created and updated users
                    User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                    User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                    return (ISubjectBaseDTO)new SubjectAdminViewDTO
                    {
                        Id = subject.Id,
                        SubjectName = subject.SubjectName,
                        Price = subject.Price,
                        Status = subject.Status,
                        CreatedBy = subject.CreatedBy,
                        CreaterName = createdUser?.FullName,
                        CreaterPhone = createdUser?.PhoneNumber,
                        CreatedTime = subject.CreatedTime,
                        LastUpdatedBy = subject.LastUpdatedBy,
                        UpdaterName = updatedUser?.FullName,
                        UpdaterPhone = updatedUser?.PhoneNumber,
                        LastUpdatedTime = subject.LastUpdatedTime
                    };
                }).ToList();

                if (subjectDtos.Count == 0)
                {
                    throw new BaseException.BadRequestException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            var paginatedSubjects = await _unitOfWork.GetRepository<Subject>().GetPagging(query, pageNumber, pageSize);
            var subjectDtosPaginated = paginatedSubjects.Items.Select(subject =>
            {
                // Fetch the created and updated users
                User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                return (ISubjectBaseDTO)new SubjectAdminViewDTO
                {
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    Price = subject.Price,
                    Status = subject.Status,
                    CreatedBy = subject.CreatedBy,
                    CreaterName = createdUser?.FullName,
                    CreaterPhone = createdUser?.PhoneNumber,
                    CreatedTime = subject.CreatedTime,
                    LastUpdatedBy = subject.LastUpdatedBy,
                    UpdaterName = updatedUser?.FullName,
                    UpdaterPhone = updatedUser?.PhoneNumber,
                    LastUpdatedTime = subject.LastUpdatedTime
                }; 
            }).ToList();

            if (subjectDtosPaginated.Count == 0)
            {
                throw new BaseException.BadRequestException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        // Update subject and set LastUpdatedTime to current time
        public async Task<SubjectAdminViewDTO> UpdateSubjectAsync(string id, SubjectUpdateDTO subjectUpdateDTO)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("key_not_found", $"Subject with ID '{id}' not found.");

            // Check if another subject with the same name already exists
            var existingSubject = await _unitOfWork.GetRepository<Subject>().Entities
                .Where(s => s.SubjectName == subjectUpdateDTO.SubjectName) // Exclude the current subject by its ID
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new BaseException.BadRequestException("duplicate_name", $"A subject with the name '{subjectUpdateDTO.SubjectName}' already exists.");
            }

            if (string.IsNullOrWhiteSpace(subjectUpdateDTO.SubjectName))
            {
                throw new BaseException.BadRequestException("empty_subject_name", "Subject name is required and cannot be empty.");
            }

            if (subjectUpdateDTO.Price <= 0)
            {
                throw new BaseException.BadRequestException("price_range_error", "Price must be greater than zero.");
            }

            subject.SubjectName = subjectUpdateDTO.SubjectName;
            subject.Price = subjectUpdateDTO.Price;
            subject.Status = subjectUpdateDTO.Status;

            AuditFields(subject, isCreating: false);

            _unitOfWork.GetRepository<Subject>().Update(subject);
            await _unitOfWork.SaveAsync();

            User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
            User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

            return new SubjectAdminViewDTO
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                Price = subject.Price,
                Status = subject.Status,
                CreatedBy = subject.CreatedBy,
                CreaterName = createdUser?.FullName,
                CreaterPhone = createdUser?.PhoneNumber,
                CreatedTime = subject.CreatedTime,
                LastUpdatedBy = subject.LastUpdatedBy,
                UpdaterName = createdUser?.FullName,
                UpdaterPhone = createdUser?.PhoneNumber,
                LastUpdatedTime = subject.LastUpdatedTime
            };
        }

        // Change subject status and set LastUpdatedTime to current time
        public async Task<SubjectAdminViewDTO> ChangeSubjectStatusAsync(string id)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("key_not_found", $"Subject with ID '{id}' not found.");

            subject.Status = !subject.Status;
            AuditFields(subject);

            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.SaveAsync();

            User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
            User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

            return new SubjectAdminViewDTO
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName,
                Price = subject.Price,
                Status = subject.Status,
                CreatedBy = subject.CreatedBy,
                CreaterName = createdUser?.FullName,
                CreaterPhone = createdUser?.PhoneNumber,
                CreatedTime = subject.CreatedTime,
                UpdaterName = updatedUser?.FullName,
                UpdaterPhone = updatedUser?.PhoneNumber,
                LastUpdatedBy = subject.LastUpdatedBy,
                LastUpdatedTime = subject.LastUpdatedTime
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
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("key_not_found", $"Subject with ID '{id}' not found.");
            return subject;
        }

        public async void AuditFields(BaseEntity entity, bool isCreating = false)
        {
            // Get current logged in user info
            User currentUser = await _userService.GetCurrentUserAsync();
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
            User currentUser = await _userService.GetCurrentUserAsync();
            var currentUserId = currentUser.Id;

            // Query the Progress table for a record with the specified SubjectId and QuizId
            var progressRecord = await _unitOfWork.GetRepository<Progress>().Entities
                .Where(p => p.StudentId.Equals(currentUserId.ToString(), StringComparison.CurrentCultureIgnoreCase) && p.SubjectId == subjectId && p.QuizId == quizId)
                .FirstOrDefaultAsync();

            // If the record is found, return true; otherwise, return false
            return progressRecord != null;
        }

        //soft deleted
        public async Task SoftDeleteSubjectAsync(string id)
        {
            // Retrieve the subject
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id);

            if (subject == null || !string.IsNullOrWhiteSpace(subject.DeletedBy))
            {
                throw new BaseException.BadRequestException("key_not_found", $"Cannot find subject with ID '{id}' or it is already deleted.");
            }

            User currentUser = await _userService.GetCurrentUserAsync();
            var currentUserId = currentUser.Id;

            // Mark subject as deleted
            subject.DeletedBy = currentUserId;
            subject.DeletedTime = DateTime.UtcNow;

            // Update and save
            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.GetRepository<Subject>().SaveAsync();
        }

        //restore soft deleted
        public async Task RestoreSubjectAsync(string id)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id);

            if (subject == null || string.IsNullOrWhiteSpace(subject.DeletedBy))
            {
                throw new BaseException.BadRequestException("key_not_found", $"Cannot find deleted subject with ID '{id}'.");
            }

            // Undo soft delete
            subject.DeletedBy = null;
            subject.DeletedTime = null;

            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.GetRepository<Subject>().SaveAsync();
        }
    }
}
