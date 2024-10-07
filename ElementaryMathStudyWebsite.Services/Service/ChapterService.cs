using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SendGrid.Helpers.Mail;
using System.Runtime.Intrinsics.X86;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ChapterService : IAppChapterServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userServices;
        private readonly IAppTopicServices _topicService;
        private readonly IAppQuizServices _quizService;
        private readonly IMapper _mapper;


        // Constructor
        public ChapterService(IUnitOfWork unitOfWork, IAppUserServices userServices, IAppTopicServices topicService, IAppQuizServices quizService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userServices = userServices;
            _topicService = topicService;
            _quizService = quizService;
            _mapper = mapper;
        }

        private void ValidateChapter(ChapterDto chapterDTO)
        {
            //if (chapterDTO.Number == null)
            //{
            //    throw new ArgumentException("Number is required and cannot be empty.");
            //}
            if (string.IsNullOrWhiteSpace(chapterDTO.ChapterName))
            {
                throw new BaseException.BadRequestException("invalid", "Chapter name is required and cannot be empty");
            }
            //if (!chapterDTO.Status)
            //{
            //    throw new ArgumentException("Status is required and cannot be false.");
            //}
            //if (string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            //{
            //    BaseException.BadRequestException("announcement", "Subject Id is required and cannot be empty.");
            //}
        }

        public async Task<int> CountChaptersInSubjectAsync(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new BaseException.BadRequestException("invalid", "Subject ID cannot be null or empty");
            }

            return await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.SubjectId == subjectId)
                .CountAsync();
        }
        public async Task<bool> IsQuizIdInChaptersAsync(string quizId)
        {
            var quizExists = await _unitOfWork.GetRepository<Chapter>().Entities
                .AnyAsync(c => c.QuizId == quizId);

            return quizExists;
        }
        /// <summary>
        /// for admin-content to create a chapter
        /// </summary>
        /// <param name="chapterDTO"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<ChapterViewDto> CreateChapterAsync(ChapterDto chapterDTO)
        {
            ValidateChapter(chapterDTO);

            // Check if another chapter with the same name already exists
            Chapter? existingChapter = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName)
                .FirstOrDefaultAsync();

            if (existingChapter != null)
            {
                throw new BaseException.BadRequestException("value_duplicate_error", "This chapter name was used");
            }

            //Đếm số lượng Chapter trong Subject
            int chapterCount = await CountChaptersInSubjectAsync(chapterDTO.SubjectId);

            //Kiểm tra sự tồn tại của Subject
            if (!string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            {
                bool subjectExists = await _unitOfWork.GetRepository<Subject>().Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new BaseException.NotFoundException("not_found", "Subject with Id does not exist");
                }
            }

            //Kiểm tra sự tồn tại của Quiz và tính duy nhất của QuizId
            if (!string.IsNullOrWhiteSpace(chapterDTO.QuizId))
            {
                bool quizExists = await _unitOfWork.GetRepository<Quiz>().Entities
                    .AnyAsync(q => q.Id == chapterDTO.QuizId);

                if (!quizExists)
                {
                    throw new BaseException.NotFoundException("not_found", "Quiz with Id does not exist");
                }
                bool isQuizId = await IsQuizIdInChaptersAsync(chapterDTO.QuizId);
                if (isQuizId)
                {
                    throw new BaseException.BadRequestException("value_duplicate_error", "This Quiz Id was used");
                }
            }


            Chapter chapter = new Chapter
            {
                Number = chapterCount + 1,
                ChapterName = chapterDTO.ChapterName,
                Status = true,
                SubjectId = chapterDTO.SubjectId,
                QuizId = string.IsNullOrWhiteSpace(chapterDTO.QuizId) ? null : chapterDTO.QuizId,
            };

            _userServices.AuditFields(chapter, isCreating: true);

            //Chèn Chapter mới vào repository và lưu thay đổi
            _unitOfWork.GetRepository<Chapter>().Insert(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            //Lấy thông tin Subject và Quiz liên quan
            Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            Quiz? quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

            ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
            {
                opts.Items["Subject"] = subject;
                opts.Items["Quiz"] = quiz;
            });

            return chapterViewDTO;
        }

        /// <summary>
        /// for admin-content edit chapter
        /// </summary>
        /// <param name="id"></param>
        /// <param name="chapterDTO"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        /// <exception cref="BaseException.BadRequestException"></exception>
        public async Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterUpdateDto chapterDTO)
        {
            Chapter chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(id) ?? throw new BaseException.NotFoundException("not_found", "Chapter with ID not found");
            if (string.IsNullOrWhiteSpace(chapterDTO.ChapterName))
            {
                throw new BaseException.BadRequestException("invalid", "Chapter name is required and cannot be empty.");
            }
            // Check if another subject with the same name already exists
            Chapter? existingSubject = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName) // Exclude the current subject by its ID
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new BaseException.BadRequestException("value_duplicate_error", "This chapter name was used");
            }

            chapter.ChapterName = chapterDTO.ChapterName;

            _userServices.AuditFields(chapter, isCreating: false);

            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            Quiz? quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);

            ChapterAdminViewDto chapterAdminViewDTO = _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Subject"] = subject;
                opts.Items["Quiz"] = quiz;
            });

            return chapterAdminViewDTO;
        }
        /// <summary>
        /// for admin-content to delete chapter
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<bool> DeleteChapterAsync(string chapterId)
        {
            // Validate chapter ID
            if (!_unitOfWork.IsValid<Chapter>(chapterId))
            {
                throw new BaseException.NotFoundException("not_found", "Chapter ID not found");
            }

            // Retrieve the chapter
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId);
            if (chapter == null)
            {
                throw new BaseException.NotFoundException("not_found", "Chapter does not exist.");
            }

            // Audit fields
            _userServices.AuditFields(chapter, false, true);

            // Save changes
            await _unitOfWork.SaveAsync();

            // Delete the corresponding topics
            var topics = _unitOfWork.GetRepository<Topic>().GetEntitiesWithCondition(
                t => t.ChapterId == chapterId && string.IsNullOrWhiteSpace(t.DeletedBy)
            );

            foreach (Topic topic in topics)
            {
                await _topicService.DeleteTopicAsync(topic.Id);
            }

            // Delete the corresponding quiz
            if (chapter.QuizId != null)
            {
                await _quizService.DeleteQuizAsync(chapter.QuizId);
            }

            return true;
        }

        /// <summary>
        /// for admin-content rollback to deleted chapter
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        /// <exception cref="BaseException.BadRequestException"></exception>
        public async Task<ChapterAdminViewDto> rollbackChapterDeletedAsync(string chapterId)
        {
            Chapter chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId) ?? throw new BaseException.NotFoundException("not_found", "Chapter ID not found");
            if (chapter.DeletedBy == null)
            {
                throw new BaseException.BadRequestException("invalid", "This chapter was rollback");
            }
            chapter.DeletedBy = null;
            chapter.DeletedTime = null;

            _userServices.AuditFields(chapter);
            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            Quiz? quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);


            ChapterAdminViewDto chapterAdminViewDTO = _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Subject"] = subject;
                opts.Items["Quiz"] = quiz;
            });

            return chapterAdminViewDTO;
        }

        /// <summary>
        /// Changing the Order Number of a Chapter
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="chapterNumberDto"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<bool> UpdateChapterNumbersAsync(string subjectId, ChapterNumberDto chapterNumberDto)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new BaseException.BadRequestException("invalid_subject_id", "Subject Id cannot be null or empty.");
            }

            if (chapterNumberDto == null)
            {
                throw new BaseException.BadRequestException("invalid", "The chapterNumberDto field is required.");
            }

            var chaptersToUpdate = chapterNumberDto.ChapterNumbersOrder.ToList();
            var chapterIds = chaptersToUpdate.Select(c => c.Id).ToList();

            // Check for duplicate IDs
            var duplicateIds = chapterIds.GroupBy(id => id).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateIds.Any())
            {
                throw new BaseException.BadRequestException("duplicate_id", $"Duplicate chapter ID(s): {string.Join(", ", duplicateIds)}");
            }

            // Check for empty Ids or invalid Numbers
            chaptersToUpdate.Select(chapter =>
            {
                if (string.IsNullOrWhiteSpace(chapter.Id))
                {
                    throw new BaseException.BadRequestException("invalid_id", "Chapter Id cannot be null or empty.");
                }

                if (!chapter.Number.HasValue || chapter.Number.Value < 1)
                {
                    throw new BaseException.BadRequestException("invalid_number", "Chapter Number must be 1 or greater.");
                }

                return chapter; // Trả về chapter để duy trì cấu trúc Select
            }).ToList(); // Chuyển đổi kết quả thành danh sách để thực hiện các hành động

            var chapters = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.SubjectId == subjectId && chapterIds.Contains(c.Id))
                .ToListAsync();

            if (!chapters.Any())
            {
                throw new BaseException.BadRequestException("invalid", "Invalid Subject Id");
            }

            // Check for missing Ids
            var missingIds = chapterIds.Except(chapters.Select(c => c.Id)).ToList();
            if (missingIds.Any())
            {
                throw new BaseException.NotFoundException("not_found", $"Chapter ID(s) not found: {string.Join(", ", missingIds)}");
            }

            // Get the total number of chapters in the subject
            var totalChaptersInSubject = await _unitOfWork.GetRepository<Chapter>().Entities
                .CountAsync(c => c.SubjectId == subjectId);

            var existingNumbers = new HashSet<int>();

            var currentNumbers = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.SubjectId == subjectId && !chapterIds.Contains(c.Id))
                .Select(c => c.Number)
                .ToListAsync();

            chaptersToUpdate.Select(dto =>
            {
                var chapter = chapters.FirstOrDefault(c => c.Id == dto.Id);
                if (chapter != null && dto.Number.HasValue)
                {
                    // Check if the new number is within the valid range
                    if (dto.Number.Value > totalChaptersInSubject)
                    {
                        throw new BaseException.BadRequestException("invalid_number", $"Chapter Number must be less than or equal to the total number of chapters in the subject ({totalChaptersInSubject}).");
                    }

                    if (chapter.Number != dto.Number.Value)
                    {
                        var duplicateNumber = _unitOfWork.GetRepository<Chapter>().Entities
                            .AnyAsync(c => c.SubjectId == subjectId && c.Number == dto.Number.Value && !chapterIds.Contains(c.Id)).Result;

                        if (duplicateNumber)
                        {
                            throw new BaseException.BadRequestException("duplicate_number", $"Duplicate chapter number: {dto.Number.Value}");
                        }
                    }

                    if (existingNumbers.Contains(dto.Number.Value))
                    {
                        throw new BaseException.BadRequestException("duplicate_number", $"Duplicate chapter number: {dto.Number.Value}");
                    }

                    existingNumbers.Add(dto.Number.Value);
                    chapter.Number = dto.Number.Value;
                    _unitOfWork.GetRepository<Chapter>().Update(chapter);
                }

                return dto;
            }).ToList(); 

            existingNumbers.Select(number =>
            {
                if (currentNumbers.Contains(number))
                {
                    throw new BaseException.BadRequestException("duplicate_number", $"Duplicate chapter number: {number}");
                }

                return number; 
            }).ToList();


            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            return true;
        }


        /// <summary>
        /// for admin-content to see all the information of the chapters
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<object> GetChapterByChapterIdAsync(string Id)
        {
            //Lấy Chapter từ repository
            Chapter chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(Id) ?? throw new BaseException.BadRequestException("key_not_found", $"Cannot find product with ID '{Id}'.");

            //Kiểm tra Status và DeletedBy của Chapter
            if ((!chapter.Status) || !String.IsNullOrWhiteSpace(chapter.DeletedBy))
            {
                throw new BaseException.NotFoundException("key_not_found", $"Cannot find product with ID '{Id}'.");
            }

            //Xử lý khi Chapter có Status true
            if (chapter.Status)
            {
                User? createdUser = chapter.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                Quiz? quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                ChapterAdminViewDto chapterAdminViewDTO = _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                {
                    opts.Items["CreatedUser"] = createdUser;  
                    opts.Items["UpdatedUser"] = updatedUser; 
                    opts.Items["Subject"] = subject; 
                    opts.Items["Quiz"] = quiz;  
                });

                return chapterAdminViewDTO;
            }
            else
            {
                //Xử lý khi Chapter có Status != true
                ChapterDto chapterDTO = _mapper.Map<ChapterDto>(chapter);

                return chapterDTO;
            }
        }

        /// <summary>
        /// for users to search for chapter information
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<object> GetChapterDtoByChapterIdAsync(string Id)
        {
            //Lấy Chapter từ repository
            Chapter chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(Id) ?? throw new BaseException.BadRequestException("key_not_found", $"Cannot find product with ID '{Id}'.");

            //Kiểm tra Status và DeletedBy của Chapter
            if ((!chapter.Status) || !String.IsNullOrWhiteSpace(chapter.DeletedBy))
            {
                throw new BaseException.NotFoundException("key_not_found", $"Cannot find product with ID '{Id}'.");
            }

            //Xử lý khi Chapter có Status true
            if (chapter.Status)
            {
                Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                Quiz? quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                {
                    opts.Items["Subject"] = subject; 
                    opts.Items["Quiz"] = quiz; 
                });

                return chapterViewDTO;
            }
            else
            {
                //Xử lý khi Chapter có Status != true
                ChapterDto chapterDTO = _mapper.Map<ChapterDto>(chapter);

                return chapterDTO;
            }
        }

        /// <summary>
        /// View all chapters of a subject by ID
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<BasePaginatedList<object>> GetChaptersBySubjectIdAsync(int pageNumber, int pageSize, string subjectId)
        {
            Subject subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(subjectId) ?? throw new BaseException.NotFoundException("not_found", "Subject ID not found");

            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities
            .Where(c => c.SubjectId == subjectId && String.IsNullOrWhiteSpace(c.DeletedBy) && c.Status == true);


            //Kiểm tra và xử lý phân trang
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                List<Chapter> allChapters = await query.ToListAsync();
                var chapterViewTasks = allChapters.Select(chapter =>
                {
                    if (chapter != null)
                    {
                        Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                        Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                        return _mapper.Map<ChapterViewDto>(chapter, opts =>
                        {
                            opts.Items["Subject"] = subject;
                            opts.Items["Quiz"] = quiz;
                        });
                    }
                    return null;
                }).ToList();

                return new BasePaginatedList<object>(chapterViewTasks!, chapterViewTasks.Count, 1, chapterViewTasks.Count);
            }

            //Thực hiện phân trang tự động
            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var paginatedChapterViewTasks = paginatedChapters.Items.Select(chapter =>
            {
                if (chapter != null)
                {
                    Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                    Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                    return _mapper.Map<ChapterViewDto>(chapter, opts =>
                    {
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                }
                return null;
            }).ToList();

            return new BasePaginatedList<object>(paginatedChapterViewTasks!, paginatedChapterViewTasks.Count, pageNumber, pageSize);
        }
        /// <summary>
        /// Show admin-content all chapter information
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<BasePaginatedList<object>> GetChaptersAsync(int pageNumber, int pageSize)
        {
            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => String.IsNullOrWhiteSpace(c.DeletedBy) && c.Status == true);

            //Kiểm tra và xử lý phân trang
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                List<Chapter> allChapters = query.ToList();
                var chapterViewTasks = allChapters.Select(chapter =>
                {
                    if (chapter != null)
                    {
                        User? createdUser = chapter.CreatedBy != null ?  _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                        User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                        Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                        Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                        return _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                        {
                            opts.Items["CreatedUser"] = createdUser;
                            opts.Items["UpdatedUser"] = updatedUser;
                            opts.Items["Subject"] = subject;
                            opts.Items["Quiz"] = quiz;
                        });
                    }
                    return null;
                }).ToList();

                return new BasePaginatedList<object>(chapterViewTasks!, chapterViewTasks.Count, 1, chapterViewTasks.Count);
            }

            //Thực hiện phân trang tự động
            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var paginatedChapterViewTasks = paginatedChapters.Items.Select(chapter =>
            {
                if (chapter != null)
                {
                    User? createdUser = chapter.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                    User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                    Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                    Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                    return _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;
                        opts.Items["UpdatedUser"] = updatedUser;
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                }
                return null;
            }).ToList();


            return new BasePaginatedList<object>(paginatedChapterViewTasks!, paginatedChapterViewTasks.Count, pageNumber, pageSize);
        }

        /// <summary>
        /// Show admins deleted chapters
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<BasePaginatedList<object>> GetChaptersDeletedAsync(int pageNumber, int pageSize)
        {
            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(s => !String.IsNullOrWhiteSpace(s.DeletedBy));

            //Kiểm tra và xử lý phân trang
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                List<Chapter> allChapters = query.ToList();
                var chapterViewTasks = allChapters.Select(chapter =>
                {
                    if (chapter != null)
                    {
                        User? createdUser = chapter.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                        User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                        Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                        Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                        return _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                        {
                            opts.Items["CreatedUser"] = createdUser;
                            opts.Items["UpdatedUser"] = updatedUser;
                            opts.Items["Subject"] = subject;
                            opts.Items["Quiz"] = quiz;
                        });
                    }
                    return null;
                }).ToList();

                return new BasePaginatedList<object>(chapterViewTasks!, chapterViewTasks.Count, 1, chapterViewTasks.Count);
            }

            //Thực hiện phân trang tự động
            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var paginatedChapterViewTasks = paginatedChapters.Items.Select(chapter =>
            {
                if (chapter != null)
                {
                    User? createdUser = chapter.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                    User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                    Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                    Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                    return _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;
                        opts.Items["UpdatedUser"] = updatedUser;
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                }
                return null;
            }).ToList();


            return new BasePaginatedList<object>(paginatedChapterViewTasks!, paginatedChapterViewTasks.Count, pageNumber, pageSize);
        }

        /// <summary>
        /// for ordinary users to see the chapters
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<BasePaginatedList<object>> GetChapterDtosAsync(int pageNumber, int pageSize)
        {
            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => String.IsNullOrWhiteSpace(c.DeletedBy) && c.Status == true);

            //Kiểm tra và xử lý phân trang
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                List<Chapter> allChapters = query.ToList();
                var chapterViewTasks = allChapters.Select(chapter =>
                {
                    if (chapter != null)
                    {
                        Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                        Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                        return _mapper.Map<ChapterViewDto>(chapter, opts =>
                        {
                            opts.Items["Subject"] = subject;
                            opts.Items["Quiz"] = quiz;
                        });
                    }
                    return null;
                }).ToList();

                return new BasePaginatedList<object>(chapterViewTasks!, chapterViewTasks.Count, 1, chapterViewTasks.Count);
            }

            //Thực hiện phân trang tự động
            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var paginatedChapterViewTasks = paginatedChapters.Items.Select(chapter =>
            {
                if (chapter != null)
                {
                    Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                    Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                    return _mapper.Map<ChapterViewDto>(chapter, opts =>
                    {
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                }
                return null;
            }).ToList();

            return new BasePaginatedList<object>(paginatedChapterViewTasks!, paginatedChapterViewTasks.Count, pageNumber, pageSize);

        }

        /// <summary>
        /// Assign a quizId to an available chapter
        /// </summary>
        /// <param name="chapterId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<bool> AssignQuizIdToChapterAsync(string chapterId, string quizId)
        {
            // nếu chapterId là null hoặc rỗng
            if (string.IsNullOrWhiteSpace(chapterId))
            {
                throw new BaseException.BadRequestException("ChapterId cannot be null or empty.", nameof(chapterId));
            }

            // nếu quizId là null hoặc rỗng
            if (string.IsNullOrWhiteSpace(quizId))
            {
                throw new BaseException.BadRequestException("QuizId cannot be null or empty.", nameof(quizId));
            }

            // Lấy Chapter từ repository bằng chapterId
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId);
            if (chapter == null)
            {
                // nếu Chapter không tồn tại
                throw new BaseException.NotFoundException("not_found", "Chapter does not exist.");
            }

            // nếu Chapter đã có QuizId
            if (chapter.QuizId != null)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Chapter already has a QuizId.");
            }

            // Lấy tất cả các Chapter và Topic từ repository
            var chapters = await _unitOfWork.GetRepository<Chapter>().GetAllAsync();
            var topics = await _unitOfWork.GetRepository<Topic>().GetAllAsync();

            // Lấy Quiz từ repository bằng quizId
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(quizId);
            if (quiz == null)
            {
                // nếu Quiz không tồn tại
                throw new BaseException.NotFoundException("not_found", "QuizId does not exist.");
            }

            // nếu quizId đã được gán cho Chapter hoặc Topic khác
            bool quizIdExists = chapters.Any(c => c.QuizId?.Equals(quizId) ?? false) || topics.Any(t => t.QuizId?.Equals(quizId) ?? false);
            if (quizIdExists)
            {
                // nếu quizId đã được gán cho Chapter hoặc Topic khác
                throw new BaseException.BadRequestException("invalid_argument", "The QuizId has been assigned to another chapter or topic.");
            }

            // Gán quizId cho Chapter và lưu thay đổi
            chapter.QuizId = quizId;
            await _unitOfWork.SaveAsync();
            return true;
        }



        /// <summary>
        /// Change subject status and set LastUpdatedTime to current time
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<ChapterAdminViewDto> ChangeChapterStatusAsync(string id)
        {
            //Lấy Chapter từ repository
            Chapter chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(id) ?? throw new BaseException.NotFoundException("not_found", "Chapter ID not found");

            //Thay đổi trạng thái của Chapter
            chapter.Status = !chapter.Status;

            _userServices.AuditFields(chapter);

            //Lấy thông tin liên quan từ repository
            Subject? subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            Quiz? quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);

            //Cập nhật Chapter trong repository
            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            //Ánh xạ Chapter sang ChapterAdminViewDto
            ChapterAdminViewDto chapterAdminViewDTO = _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Subject"] = subject;
                opts.Items["Quiz"] = quiz;
            });
            return chapterAdminViewDTO;
        }

        /// <summary>
        /// Search for chapters by name for regular users
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        /// <exception cref="BaseException.NotFoundException"></exception>
        public async Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize)
        {
            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => string.IsNullOrWhiteSpace(c.DeletedBy) && c.Status.Equals(true));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{searchTerm}%"));
            }

            //Kiểm tra tính hợp lệ của searchTerm
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term must be at least 2 characters long.");
            }

            //Thực hiện truy vấn và lấy tất cả các Chapter
            var allChapters = await query.ToListAsync();

            //Kiểm tra và xử lý phân trang
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var chapterDtos = allChapters.Select(chapter =>
                {
                    Subject? subject = chapter.SubjectId != null ? _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId) : null;
                    Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                    return _mapper.Map<ChapterViewDto>(chapter, opts =>
                    {
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                }).ToList();

                if (chapterDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("key_not_found", $"No chapter found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            //Thực hiện phân trang tự động
            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var chapterDtosPaginated = paginatedChapters.Items.Select(chapter =>
            {
                Subject? subject = chapter.SubjectId != null ? _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId) : null;
                Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;
                return _mapper.Map<ChapterViewDto>(chapter, opts =>
                {
                    opts.Items["Subject"] = subject;
                    opts.Items["Quiz"] = quiz;
                });
            }).ToList();

            if (chapterDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("key_not_found", $"No chapters found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(chapterDtosPaginated, chapterDtosPaginated.Count, pageNumber, pageSize);
        }

        /// <summary>
        /// Search Chapter by Name for Admin - Content
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="BaseException.BadRequestException"></exception>
        public async Task<BasePaginatedList<object>> SearchChapterForAdminAsync(string searchTerm, int pageNumber, int pageSize)
        {

            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => String.IsNullOrWhiteSpace(c.DeletedBy));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{searchTerm}%"));
            }

            //Kiểm tra tính hợp lệ của searchTerm
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term must be at least 2 characters long.");
            }

            //Kiểm tra và xử lý phân trang
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                List<Chapter> allChapters = query.ToList();
                var chapterViewTasks = allChapters.Select(chapter =>
                {
                    if (chapter != null)
                    {
                        User? createdUser = chapter.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                        User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                        Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                        Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                        return _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                        {
                            opts.Items["CreatedUser"] = createdUser;
                            opts.Items["UpdatedUser"] = updatedUser;
                            opts.Items["Subject"] = subject;
                            opts.Items["Quiz"] = quiz;
                        });
                    }
                    return null;
                }).ToList();

                return new BasePaginatedList<object>(chapterViewTasks!, chapterViewTasks.Count, 1, chapterViewTasks.Count);
            }

            //Thực hiện phân trang tự động
            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var paginatedChapterViewTasks = paginatedChapters.Items.Select(chapter =>
            {
                if (chapter != null)
                {
                    User? createdUser = chapter.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                    User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                    Subject? subject = _unitOfWork.GetRepository<Subject>().GetById(chapter.SubjectId);
                    Quiz? quiz = chapter.QuizId != null ? _unitOfWork.GetRepository<Quiz>().GetById(chapter.QuizId) : null;

                    return _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;
                        opts.Items["UpdatedUser"] = updatedUser;
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                }
                return null;
            }).ToList();

            return new BasePaginatedList<object>(paginatedChapterViewTasks!, paginatedChapterViewTasks.Count, pageNumber, pageSize);
        }

    }
}