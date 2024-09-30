using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class SubjectService : IAppSubjectServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppChapterServices _chapterService;
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IAppUserServices userService, IAppChapterServices chapterService, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService;
            _chapterService = chapterService;
            _mapper = mapper;
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
            Subject? existingSubject = await _unitOfWork.GetRepository<Subject>().Entities
                .Where(s => s.SubjectName == subjectDTO.SubjectName)
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new BaseException.BadRequestException("duplicate_name", $"A subject with the name '{subjectDTO.SubjectName}' already exists.");
            }

            //var subject = _mapper.Map<Subject>(subjectDTO);
            Subject subject = new Subject
            {
                SubjectName = subjectDTO.SubjectName,
                Price = subjectDTO.Price,
                Status = subjectDTO.Status,
            };

            AuditFields(subject, isCreating: true);

            await _unitOfWork.GetRepository<Subject>().InsertAsync(subject);
            await _unitOfWork.SaveAsync();

            User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
            User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

            var subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;  // Set created user object
                opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
            });

            return subjectAdminViewDTO;
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
                List<Subject> allSubjects = await query.ToListAsync();
                List<ISubjectBaseDTO> subjectDtos = allSubjects.Select(subject =>
                {
                    if (isAdmin)
                    {
                        // Fetch the created and updated users
                        User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                        User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                        SubjectAdminViewDTO subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
                        {
                            opts.Items["CreatedUser"] = createdUser;  // Set created user object
                            opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
                        });

                        return (ISubjectBaseDTO)subjectAdminViewDTO;
                    }
                    else
                    {
                        SubjectDTO subjectDTO = _mapper.Map<SubjectDTO>(subject);

                        return subjectDTO;
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

                    SubjectAdminViewDTO? subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;  // Set created user object
                        opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
                    });

                    return (ISubjectBaseDTO)subjectAdminViewDTO;  // Cast to common interface
                }
                else
                {
                    SubjectDTO subjectDTO = _mapper.Map<SubjectDTO>(subject);

                    return (ISubjectBaseDTO)subjectDTO;  // Cast to common interface
                }
            }).ToList();


            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        // Get a specific subject by ID
        public async Task<object> GetSubjectByIDAsync(string id, bool isAdmin)
        {
            Subject subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("key_not_found", $"Cannot find product with ID '{id}'.");
            if ((!isAdmin && !subject.Status) || !String.IsNullOrWhiteSpace(subject.DeletedBy))
            {
                throw new BaseException.NotFoundException("key_not_found", $"Cannot find product with ID '{id}'.");
            }

            if (isAdmin)
            {
                User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                SubjectAdminViewDTO subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
                {
                    opts.Items["CreatedUser"] = createdUser;  // Set created user object
                    opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
                });

                return subjectAdminViewDTO;
            }
            else
            {
                SubjectDTO subjectDTO = _mapper.Map<SubjectDTO>(subject);

                return subjectDTO;
            }
        }

        // Search subjects by name
        public async Task<BasePaginatedList<object>> SearchSubjectAsync(string searchTerm, double lowestPrice,
                    double highestPrice, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term must be at least 2 characters long.");
            }

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
                List<Subject> allSubjects = await query.ToListAsync();
                List<SubjectDTO> subjectDtos = _mapper.Map<List<SubjectDTO>>(allSubjects);

                if (subjectDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            BasePaginatedList<Subject> paginatedSubjects = await _unitOfWork.GetRepository<Subject>().GetPagging(query, pageNumber, pageSize);
            List<SubjectDTO> subjectDtosPaginated = _mapper.Map<List<SubjectDTO>>(paginatedSubjects.Items);

            if (subjectDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<object>> SearchSubjectAdminAsync(string searchTerm, double lowestPrice,
                    double highestPrice, bool? status, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term must be at least 2 characters long.");
            }

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
                List<Subject> allSubjects = await query.ToListAsync();
                List<ISubjectBaseDTO> subjectDtos = allSubjects.Select(subject =>
                {
                    // Fetch the created and updated users
                    User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                    User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                    SubjectAdminViewDTO subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;  // Set created user object
                        opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
                    });

                    return (ISubjectBaseDTO)subjectAdminViewDTO;
                }).ToList();

                if (subjectDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(subjectDtos, subjectDtos.Count, 1, subjectDtos.Count);
            }

            BasePaginatedList<Subject> paginatedSubjects = await _unitOfWork.GetRepository<Subject>().GetPagging(query, pageNumber, pageSize);
            List<ISubjectBaseDTO> subjectDtosPaginated = paginatedSubjects.Items.Select(subject =>
            {
                // Fetch the created and updated users
                User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
                User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

                SubjectAdminViewDTO subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
                {
                    opts.Items["CreatedUser"] = createdUser;  // Set created user object
                    opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
                });
                return (ISubjectBaseDTO)subjectAdminViewDTO; 
            }).ToList();

            if (subjectDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("key_not_found", $"No subjects found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(subjectDtosPaginated, subjectDtosPaginated.Count, pageNumber, pageSize);
        }

        // Update subject and set LastUpdatedTime to current time
        public async Task<SubjectAdminViewDTO> UpdateSubjectAsync(string id, SubjectUpdateDTO subjectUpdateDTO)
        {
            Subject subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.NotFoundException("key_not_found", $"Subject with ID '{id}' not found.");

            // Check if another subject with the same name already exists
            Subject? existingSubject = await _unitOfWork.GetRepository<Subject>().Entities
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

            SubjectAdminViewDTO subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;  // Set created user object
                opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
            });
            return subjectAdminViewDTO;
        }

        // Change subject status and set LastUpdatedTime to current time
        public async Task<SubjectAdminViewDTO> ChangeSubjectStatusAsync(string id)
        {
            Subject subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("key_not_found", $"Subject with ID '{id}' not found.");

            subject.Status = !subject.Status;
            AuditFields(subject);

            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.SaveAsync();

            User? createdUser = subject.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.CreatedBy) : null;
            User? updatedUser = subject.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(subject.LastUpdatedBy) : null;

            SubjectAdminViewDTO subjectAdminViewDTO = _mapper.Map<SubjectAdminViewDTO>(subject, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;  // Set created user object
                opts.Items["UpdatedUser"] = updatedUser;  // Set updated user object
            });
            return subjectAdminViewDTO;
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
            Subject subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("key_not_found", $"Subject with ID '{id}' not found.");
            return subject;
        }

        public async void AuditFields(BaseEntity entity, bool isCreating = false)
        {
            // Get current logged in user info
            User currentUser = await _userService.GetCurrentUserAsync();
            string currentUserId = currentUser.Id;

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
            string currentUserId = currentUser.Id;

            // Query the Progress table for a record with the specified SubjectId and QuizId
            Progress? progressRecord = await _unitOfWork.GetRepository<Progress>().Entities
                .Where(p => p.StudentId.Equals(currentUserId.ToString(), StringComparison.CurrentCultureIgnoreCase) && p.SubjectId == subjectId && p.QuizId == quizId)
                .FirstOrDefaultAsync();

            // If the record is found, return true; otherwise, return false
            return progressRecord != null;
        }

        //soft deleted
        public async Task SoftDeleteSubjectAsync(string subjectId)
        {
            Subject? subject;

            if (_unitOfWork.IsValid<Subject>(subjectId))
                subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(subjectId);
            else throw new BaseException.NotFoundException("not_found", "Subject ID not found");

            _userService.AuditFields(subject!, false, true);

            await _unitOfWork.SaveAsync();

            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().GetEntitiesWithCondition(
                            c => c.SubjectId == subjectId &&
                            string.IsNullOrWhiteSpace(c.DeletedBy)
                            );

            var tasks = query.Select(chapter => _chapterService.DeleteChapterAsync(chapter.Id));
            await Task.WhenAll(tasks);
        }

        //restore soft deleted
        public async Task RestoreSubjectAsync(string id)
        {
            Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(id);

            if (subject == null || string.IsNullOrWhiteSpace(subject.DeletedBy))
            {
                throw new BaseException.NotFoundException("key_not_found", $"Cannot find deleted subject with ID '{id}'.");
            }

            // Undo soft delete
            subject.DeletedBy = null;
            subject.DeletedTime = null;

            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.GetRepository<Subject>().SaveAsync();
        }
    }
}
