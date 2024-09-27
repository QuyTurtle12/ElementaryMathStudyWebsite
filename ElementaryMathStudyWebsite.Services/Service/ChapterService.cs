using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Store;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        public async Task<ChapterViewDto> CreateChapterAsync(ChapterDto chapterDTO)
        {
            ValidateChapter(chapterDTO);

            // Check if another chapter with the same name already exists
            var existingChapter = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName)
                .FirstOrDefaultAsync();

            if (existingChapter != null)
            {
                throw new BaseException.BadRequestException("value_duplicate_error", "This chapter name was used");
            }

            var chapterCount = await CountChaptersInSubjectAsync(chapterDTO.SubjectId);

            if (!string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            {
                var subjectExists = await _unitOfWork.GetRepository<Subject>().Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new BaseException.NotFoundException("not_found", "Subject with Id does not exist");
                }
            }
            //if (string.IsNullOrWhiteSpace(chapterDTO.QuizId)) 
            //{
            //    chapterDTO.QuizId = null;
            //}

            if (!string.IsNullOrWhiteSpace(chapterDTO.QuizId))
            {
                var quizExists = await _unitOfWork.GetRepository<Quiz>().Entities
                    .AnyAsync(q => q.Id == chapterDTO.QuizId);

                if (!quizExists)
                {
                    throw new BaseException.NotFoundException("not_found", "Quiz with Id does not exist");
                }
                var isQuizId = await IsQuizIdInChaptersAsync(chapterDTO.QuizId);
                if (isQuizId)
                {
                    throw new BaseException.BadRequestException("value_duplicate_error", "This Quiz Id was used");
                }
            }


            var chapter = new Chapter
            {
                Number = chapterCount + 1,
                ChapterName = chapterDTO.ChapterName,
                Status = true,
                SubjectId = chapterDTO.SubjectId,
                QuizId = chapterDTO.QuizId,
            };

            _userServices.AuditFields(chapter, isCreating: true);

            _unitOfWork.GetRepository<Chapter>().Insert(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

            ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
            {
                opts.Items["Subject"] = subject;
                opts.Items["Quiz"] = quiz;
            });

            return chapterViewDTO;
        }

        public async Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterUpdateDto chapterDTO)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(id) ?? throw new BaseException.NotFoundException("not_found", "Chapter with ID not found");
            if (string.IsNullOrWhiteSpace(chapterDTO.ChapterName))
            {
                throw new BaseException.BadRequestException("invalid", "Chapter name is required and cannot be empty.");
            }
            // Check if another subject with the same name already exists
            var existingSubject = await _unitOfWork.GetRepository<Chapter>().Entities
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

            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
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
        public async Task<bool> DeleteChapterAsync(string chapterId)
        {
            // Delete the chapter
            Chapter? chapter;

            if (_unitOfWork.IsValid<Chapter>(chapterId))
                chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId);
            else throw new BaseException.NotFoundException("not_found", "Chapter ID not found");

            _userServices.AuditFields(chapter!, false, true);

            await _unitOfWork.SaveAsync();

            // Delete the corresponding topics
            IQueryable<Topic> query = _unitOfWork.GetRepository<Topic>().GetEntitiesWithCondition(
                            t => t.ChapterId == chapterId &&
                            string.IsNullOrWhiteSpace(t.DeletedBy)
                            );

            foreach (var topic in query)
            {
                await _topicService.DeleteTopicAsync(topic.Id);
            }

            // Delete the corresponding quiz
            await _quizService.DeleteQuizAsync(chapter!.QuizId!);

            return true;

        }

        public async Task<ChapterAdminDelete> rollbackChapterDeletedAsync(string chapterId)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId) ?? throw new BaseException.NotFoundException("not_found", "Chapter ID not found");
            if (chapter.DeletedBy == null)
            {
                throw new BaseException.BadRequestException("invalid", "This chapter was rollback");
            }
            if (chapter.Status == false)
            {
                chapter.Status = true;
            }
            chapter.DeletedBy = null;
            chapter.DeletedTime = null;

            _userServices.AuditFields(chapter);
            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
            User? deleteBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.DeletedBy ?? string.Empty);


            ChapterAdminDelete chapterAdminViewDTO = _mapper.Map<ChapterAdminDelete>(chapter, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Subject"] = subject;
                opts.Items["Quiz"] = quiz;
            });

            return chapterAdminViewDTO;
        }


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
            foreach (var chapter in chaptersToUpdate)
            {
                if (string.IsNullOrWhiteSpace(chapter.Id))
                {
                    throw new BaseException.BadRequestException("invalid_id", "Chapter Id cannot be null or empty.");
                }

                if (!chapter.Number.HasValue || chapter.Number.Value < 1)
                {
                    throw new BaseException.BadRequestException("invalid_number", "Chapter Number must be 1 or greater.");
                }
            }

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

            foreach (var chapter in chapters)
            {
                var dto = chaptersToUpdate.FirstOrDefault(c => c.Id == chapter.Id);
                if (dto != null && dto.Number.HasValue)
                {
                    // Check if the new number is within the valid range
                    if (dto.Number.Value > totalChaptersInSubject)
                    {
                        throw new BaseException.BadRequestException("invalid_number", $"Chapter Number must be less than or equal to the total number of chapters in the subject ({totalChaptersInSubject}).");
                    }

                    if (chapter.Number != dto.Number.Value)
                    {
                        var duplicateNumber = await _unitOfWork.GetRepository<Chapter>().Entities
                            .AnyAsync(c => c.SubjectId == subjectId && c.Number == dto.Number.Value && !chapterIds.Contains(c.Id));

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
            }

            foreach (var number in existingNumbers)
            {
                if (currentNumbers.Contains(number))
                {
                    throw new BaseException.BadRequestException("duplicate_number", $"Duplicate chapter number: {number}");
                }
            }

            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            return true;
        }


        // Get one order with all properties
        public async Task<object> GetChapterByChapterIdAsync(string Id)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(Id) ?? throw new BaseException.BadRequestException("key_not_found", $"Cannot find product with ID '{Id}'.");
            if ((!chapter.Status) || !String.IsNullOrWhiteSpace(chapter.DeletedBy))
            {
                throw new BaseException.NotFoundException("key_not_found", $"Cannot find product with ID '{Id}'.");
            }

            if (chapter.Status)
            {
                User? createdUser = chapter.CreatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.CreatedBy) : null;
                User? updatedUser = chapter.LastUpdatedBy != null ? _unitOfWork.GetRepository<User>().GetById(chapter.LastUpdatedBy) : null;
                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

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
                var chapterDTO = _mapper.Map<ChapterDto>(chapter);

                return chapterDTO;
            }
        }


        public async Task<object> GetChapterDtoByChapterIdAsync(string Id)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(Id) ?? throw new BaseException.BadRequestException("key_not_found", $"Cannot find product with ID '{Id}'.");
            if ((!chapter.Status) || !String.IsNullOrWhiteSpace(chapter.DeletedBy))
            {
                throw new BaseException.NotFoundException("key_not_found", $"Cannot find product with ID '{Id}'.");
            }

            if (chapter.Status)
            {
                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                {
                    opts.Items["Subject"] = subject; 
                    opts.Items["Quiz"] = quiz; 
                });

                return chapterViewDTO;
            }
            else
            {
                var chapterDTO = _mapper.Map<ChapterDto>(chapter);

                return chapterDTO;
            }
        }
        public async Task<BasePaginatedList<ChapterViewDto>> GetChaptersBySubjectIdAsync(int pageNumber, int pageSize, string subjectId)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(subjectId) ?? throw new BaseException.NotFoundException("not_found", "Subject ID not found");

            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => c.SubjectId == subjectId && c.DeletedBy == null);

            List<ChapterViewDto> chapterView = [];

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();

                foreach (var chapter in allChapters)
                {
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                    ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                    {
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });

                    chapterView.Add(chapterViewDTO);


                }
                return new BasePaginatedList<ChapterViewDto>(chapterView, chapterView.Count, 1, chapterView.Count);
            }

            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            foreach (var chapter in paginatedChapters.Items)
            {
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                {
                    opts.Items["Subject"] = subject;
                    opts.Items["Quiz"] = quiz;
                });

                chapterView.Add(chapterViewDTO);
            }

            return new BasePaginatedList<ChapterViewDto>(chapterView, paginatedChapters.TotalItems, pageNumber, pageSize);
        }
        public async Task<BasePaginatedList<ChapterAdminViewDto?>> GetChaptersAsync(int pageNumber, int pageSize)
        {
            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => c.DeletedBy == null && c.DeletedTime == null);
            List<ChapterAdminViewDto> chapterView = new List<ChapterAdminViewDto>();

            // If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();

                foreach (var chapter in allChapters)
                {
                    if (chapter != null)
                    {
                        User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                        User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                        var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                        var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                        
                        ChapterAdminViewDto chapterAdminViewDTO = _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                        {
                            opts.Items["CreatedUser"] = createdUser;
                            opts.Items["UpdatedUser"] = updatedUser;
                            opts.Items["Subject"] = subject;
                            opts.Items["Quiz"] = quiz;
                        });
                        chapterView.Add(chapterAdminViewDTO);
                    }
                }
                return new BasePaginatedList<ChapterAdminViewDto?>(chapterView, chapterView.Count, 1, chapterView.Count);
            }

            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);

            foreach (var chapter in paginatedChapters.Items)
            {
                if (chapter != null)
                {
                    User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                    User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                    ChapterAdminViewDto chapterAdminViewDTO = _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;
                        opts.Items["UpdatedUser"] = updatedUser;
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                    chapterView.Add(chapterAdminViewDTO);
                }
            }

            return new BasePaginatedList<ChapterAdminViewDto?>(chapterView, paginatedChapters.TotalItems, pageNumber, pageSize);
        }


        public async Task<BasePaginatedList<ChapterAdminDelete?>> GetChaptersDeletedAsync(int pageNumber, int pageSize)
        {
            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => c.Status == false && c.DeletedBy != null && c.DeletedTime != null);
            List<ChapterAdminDelete> chapterView = [];

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();

                foreach (var chapter in allChapters)
                {
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                    User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                    User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                    User? deleteBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.DeletedBy ?? string.Empty);
                    ChapterAdminDelete chapterAdminViewDTO = _mapper.Map<ChapterAdminDelete>(chapter, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;
                        opts.Items["UpdatedUser"] = updatedUser;
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                    chapterView.Add(chapterAdminViewDTO);
                }
                return new BasePaginatedList<ChapterAdminDelete?>(chapterView, chapterView.Count, 1, chapterView.Count);
            }



            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);

            foreach (var chapter in paginatedChapters.Items)
            {

                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                ChapterAdminDelete chapterAdminViewDTO = _mapper.Map<ChapterAdminDelete>(chapter, opts =>
                {
                    opts.Items["CreatedUser"] = createdUser;
                    opts.Items["UpdatedUser"] = updatedUser;
                    opts.Items["Subject"] = subject;
                    opts.Items["Quiz"] = quiz;
                });
                chapterView.Add(chapterAdminViewDTO);
            }

            return new BasePaginatedList<ChapterAdminDelete?>(chapterView, paginatedChapters.TotalItems, pageNumber, pageSize);
        }
        public async Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize)
        {
            // Get all chapters from database
            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => c.Status == true && c.DeletedBy == null);
            List<ChapterViewDto> chapterDtos = new List<ChapterViewDto>();

            // If pageNumber or pageSize are 0 or negative, show all chapters without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();
                // Map chapters to ChapterViewDto
                foreach (var chapter in allChapters)
                {
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                    ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                    {
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });

                    chapterDtos.Add(chapterViewDTO);
                }

                return new BasePaginatedList<ChapterViewDto?>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            // Show all chapters with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            foreach (var chapter in paginatedChapters.Items)
            {
                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                {
                    opts.Items["Subject"] = subject;
                    opts.Items["Quiz"] = quiz;
                });

                chapterDtos.Add(chapterViewDTO);
            }

            return new BasePaginatedList<ChapterViewDto?>(chapterDtos, paginatedChapters.TotalItems, pageNumber, pageSize);
        }

        public async Task<bool> AssignQuizIdToChapterAsync(string chapterId, string quizId)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId);
            if (chapter == null)
            {
                throw new BaseException.NotFoundException("not_found","Chapter không tồn tại.");
            }

            if (chapter.QuizId != null)
            {
                throw new BaseException.BadRequestException("invalid","Chapter đã có QuizId.");
            }

            if (quizId != null)
            {
                var chapters = await _unitOfWork.GetRepository<Chapter>().GetAllAsync();
                var topics = await _unitOfWork.GetRepository<Topic>().GetAllAsync();

                bool quizIdExists = chapters.Any(c => c.QuizId?.Equals(quizId) ?? false) || topics.Any(t => t.QuizId?.Equals(quizId) ?? false);
                if (quizIdExists)
                {
                    throw new BaseException.BadRequestException("invalid","QuizId đã được gán vào chapter hoặc topic khác.");
                }

                chapter.QuizId = quizId;
                await _unitOfWork.SaveAsync();
                return true;
            }

            throw new BaseException.NotFoundException("not_found","QuizId là null.");
        }



        // Change subject status and set LastUpdatedTime to current time
        public async Task<ChapterAdminViewDto> ChangeChapterStatusAsync(string id)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(id) ?? throw new BaseException.NotFoundException("not_found", "Chapter ID not found");
            chapter.Status = !chapter.Status;
            //subject.LastUpdatedTime = DateTime.UtcNow;

            _userServices.AuditFields(chapter);
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);

            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            ChapterAdminViewDto chapterAdminViewDTO = _mapper.Map<ChapterAdminViewDto>(chapter, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Subject"] = subject;
                opts.Items["Quiz"] = quiz;
            });
            return chapterAdminViewDTO;
        }

        public async Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Chapter>().Entities.Where(s => s.Status == true);

            // Not get soft deleted item
            query = query.Where(s => String.IsNullOrWhiteSpace(s.DeletedBy));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{searchTerm}%"));
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term must be at least 2 characters long.");
            }

            var allChapters = await query.ToListAsync();

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var chapterDtos = new List<ChapterViewDto>();

                foreach (var chapter in allChapters)
                {
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                    ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                    {
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });

                    chapterDtos.Add(chapterViewDTO);
                }

                if (chapterDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("key_not_found", $"No chapter found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var chapterDtosPaginated = new List<ChapterViewDto>();

            foreach (var chapter in paginatedChapters.Items)
            {
                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;

                ChapterViewDto chapterViewDTO = _mapper.Map<ChapterViewDto>(chapter, opts =>
                {
                    opts.Items["Subject"] = subject;
                    opts.Items["Quiz"] = quiz;
                });

                chapterDtosPaginated.Add(chapterViewDTO);
            }

            if (chapterDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("key_not_found", $"No chapters found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(chapterDtosPaginated, chapterDtosPaginated.Count, pageNumber, pageSize);
        }


        public async Task<BasePaginatedList<object>> SearchChapterForAdminAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Chapter>().Entities.AsQueryable();

            // Not get soft deleted item
            query = query.Where(c => String.IsNullOrWhiteSpace(c.DeletedBy));


            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{searchTerm}%"));
            }
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                throw new BaseException.BadRequestException("search_term_error", "Search term must be at least 2 characters long.");
            }
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();
                var chapterDtos = new List<ChapterAdminViewDto>();

                foreach (var chapter in allChapters)
                {
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                    User? createdUser = chapter.CreatedBy != null ? await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy) : null;
                    User? updatedUser = chapter.LastUpdatedBy != null ? await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy) : null;

                    ChapterAdminDelete chapterAdminViewDTO = _mapper.Map<ChapterAdminDelete>(chapter, opts =>
                    {
                        opts.Items["CreatedUser"] = createdUser;
                        opts.Items["UpdatedUser"] = updatedUser;
                        opts.Items["Subject"] = subject;
                        opts.Items["Quiz"] = quiz;
                    });
                    chapterDtos.Add(chapterAdminViewDTO);
                }

                if (chapterDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("key_not_found", $"No chapters found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            var paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            var chapterDtosPaginated = new List<ChapterAdminViewDto>();

            foreach (var chapter in paginatedChapters.Items)
            {
                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                User? createdUser = chapter.CreatedBy != null ? await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy) : null;
                User? updatedUser = chapter.LastUpdatedBy != null ? await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy) : null;

                ChapterAdminDelete chapterAdminViewDTO = _mapper.Map<ChapterAdminDelete>(chapter, opts =>
                {
                    opts.Items["CreatedUser"] = createdUser;
                    opts.Items["UpdatedUser"] = updatedUser;
                    opts.Items["Subject"] = subject;
                    opts.Items["Quiz"] = quiz;
                });
                chapterDtosPaginated.Add(chapterAdminViewDTO);
            }

            if (chapterDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("key_not_found", $"No chapters found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(chapterDtosPaginated, chapterDtosPaginated.Count, pageNumber, pageSize);
        }


        public async Task<string?> GetChapterNameAsync(string id)
        {
            try
            {
                Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(id);

                if (chapter == null)
                {
                    return string.Empty;
                }

                return chapter.ChapterName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

    }
}