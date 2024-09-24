using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ChapterService: IAppChapterServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userServices;

        public ChapterService(IUnitOfWork unitOfWork, IAppUserServices userServices)
        {
            _unitOfWork = unitOfWork;
            _userServices = userServices;
        }

        private void ValidateChapter(ChapterDto chapterDTO)
        {
            //if (chapterDTO.Number == null)
            //{
            //    throw new ArgumentException("Number is required and cannot be empty.");
            //}
            if (string.IsNullOrWhiteSpace(chapterDTO.ChapterName))
            {
                throw new ArgumentException("Chapter name is required and cannot be empty.");
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

        public async Task<int?> CountChaptersInSubjectAsync(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new ArgumentException("Subject ID cannot be null or empty");
               // BaseException.BadRequestException("announcement", "Subject Id is required and cannot be empty.");
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
        public async Task<ChapterAdminViewDto> CreateChapterAsync(ChapterDto chapterDTO)
        {
            ValidateChapter(chapterDTO);

            // Check if another chapter with the same name already exists
            var existingChapter = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName)
                .FirstOrDefaultAsync();

            if (existingChapter != null)
            {
                throw new BaseException.BadRequestException("Value Duplicate Error", "This chapter name was used");
            }

            var chapterCount = await CountChaptersInSubjectAsync(chapterDTO.SubjectId);

            if (!string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            {
                var subjectExists = await _unitOfWork.GetRepository<Subject>().Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new BaseException.BadRequestException("Not Found", "Subject with Id does not exist");
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
                    throw new BaseException.BadRequestException("Not Found", "Quiz with Id does not exist");
                }
                var isQuizId = await IsQuizIdInChaptersAsync(chapterDTO.QuizId);
                if (isQuizId)
                {
                    throw new BaseException.BadRequestException("Value Duplicate Error", "This Quiz Id was used");
                }
            }


            var chapter = new Chapter
            {
                Number = chapterCount + 1,
                ChapterName = chapterDTO.ChapterName,
                Status = true,
                SubjectId = chapterDTO.SubjectId,
                QuizId = chapterDTO.QuizId,
                //CreatedTime = DateTime.UtcNow,
                //LastUpdatedTime = DateTime.UtcNow // Set initial LastUpdatedTime as well
            };

            _userServices.AuditFields(chapter, isCreating: true);
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
            _unitOfWork.GetRepository<Chapter>().Insert(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();
            return new ChapterAdminViewDto
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                SubjectName = subject?.SubjectName ?? string.Empty,
                QuizId = chapter.QuizId,
                QuizName = quiz?.QuizName ?? string.Empty,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                //DeletedBy = chapter.DeletedBy,
                //DeletedTime = chapter.DeletedTime
            };
        }

        public async Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterDto chapterDTO)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("Not Found", "Chapter with ID not found");

            // Check if another subject with the same name already exists
            var existingSubject = await _unitOfWork.GetRepository<Chapter>().Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName) // Exclude the current subject by its ID
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new BaseException.BadRequestException("Value Duplicate Error", "This chapter name was used");
            }

            if (!string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            {
                var subjectExists = await _unitOfWork.GetRepository<Subject>().Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new BaseException.BadRequestException("Not Found", "Subject with Id does not exist");
                }
            }

            if (!string.IsNullOrWhiteSpace(chapterDTO.QuizId))
            {
                var quizExists = await _unitOfWork.GetRepository<Quiz>().Entities
                    .AnyAsync(q => q.Id == chapterDTO.QuizId);

                if (!quizExists)
                {
                    throw new BaseException.BadRequestException("Value Duplicate Error", "This Quiz Id was used");
                }
                //var isQuizId = await IsQuizIdInChaptersAsync(chapterDTO.QuizId);
                //if (isQuizId)
                //{
                //    throw new ArgumentException($"Subject with Id '{chapterDTO.SubjectId}' was used.");
                //}
            }

            ValidateChapter(chapterDTO);

            chapter.ChapterName = chapterDTO.ChapterName;
            //chapter.Status = chapterDTO.Status;
            chapter.SubjectId = chapterDTO.SubjectId;
            chapter.QuizId = chapterDTO.QuizId;
            //chapter.LastUpdatedTime = DateTime.UtcNow;

            _userServices.AuditFields(chapter, isCreating: false);

            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            return new ChapterAdminViewDto
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                SubjectName = subject?.SubjectName ?? string.Empty,
                QuizId = chapter.QuizId,
                QuizName = quiz?.QuizName ?? string.Empty,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                //DeletedBy = chapter.DeletedBy,
                //DeletedTime = chapter.DeletedTime
            };
        }

        public async Task<bool> ChangeChapterOrderAsync(int currentChapterNumber, int newChapterNumber)
        {

            if (currentChapterNumber <= 0 || newChapterNumber <= 0)
            {
                throw new ArgumentException("Chapter numbers must be greater than zero.");
            }
            var currentChapter = await _unitOfWork.GetRepository<Chapter>().Entities
           .FirstOrDefaultAsync(c => c.Number == currentChapterNumber);

            if (currentChapter == null)
            {
                throw new InvalidOperationException($"Chapter with number {currentChapterNumber} not found.");
            }

            // Lấy chương mới
            var newChapter = await _unitOfWork.GetRepository<Chapter>().Entities
                .FirstOrDefaultAsync(c => c.Number == newChapterNumber);

            if (newChapter == null)
            {
                throw new InvalidOperationException($"Chapter with number {newChapterNumber} not found.");
            }

            if (currentChapterNumber < newChapterNumber)
            {
                // Đổi từ nhỏ sang lớn
                var chaptersToUpdate = await _unitOfWork.GetRepository<Chapter>().Entities
                    .Where(c => c.Number > currentChapterNumber && c.Number <= newChapterNumber)
                    .OrderBy(c => c.Number)
                    .ToListAsync();

                foreach (var chapter in chaptersToUpdate)
                {
                    chapter.Number -= 1;
                    _unitOfWork.GetRepository<Chapter>().Update(chapter);
                }
            }
            else if (currentChapterNumber > newChapterNumber)
            {
                // Đổi từ lớn sang nhỏ
                var chaptersToUpdate = await _unitOfWork.GetRepository<Chapter>().Entities
                    .Where(c => c.Number >= newChapterNumber && c.Number < currentChapterNumber)
                    .OrderBy(c => c.Number)
                    .ToListAsync();

                foreach (var chapter in chaptersToUpdate)
                {
                    chapter.Number += 1;
                    _unitOfWork.GetRepository<Chapter>().Update(chapter);
                }
            }

            // Cập nhật số thứ tự của chương hiện tại
            currentChapter.Number = newChapterNumber;
            _unitOfWork.GetRepository<Chapter>().Update(currentChapter);

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            return true;
        }

        public async Task<ChapterAdminDelete> DeleteChapterAsync(string chapterId)
        {
            
            //var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            //var currentUserId = _tokenService.GetUserIdFromTokenHeader(token);
            User currentUserId = await _userServices.GetCurrentUserAsync();
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId) ?? throw new BaseException.BadRequestException("Not Found", "Chapter ID not found");
            if (chapter.DeletedBy != null)
            {
                throw new BaseException.BadRequestException("announcement", "This chapter was deleted");
            }
            if (chapter.Status == true)
            {
                chapter.Status = false;
            }
            chapter.DeletedBy = currentUserId.Id.ToString().ToUpper();
            chapter.DeletedTime = DateTime.UtcNow;

            _userServices.AuditFields(chapter);

            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
            User? deleteBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.DeletedBy ?? string.Empty);

            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            return new ChapterAdminDelete
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                SubjectName = subject?.SubjectName ?? string.Empty,
                QuizId = chapter.QuizId,
                QuizName = quiz?.QuizName ?? string.Empty,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                DeletedBy = chapter.DeletedBy,
                DeleteChapterBy = deleteBy?.FullName ?? string.Empty,
                DeletedTime = chapter.DeletedTime,
            };
        }

        public async Task<ChapterAdminDelete> rollbackChapterDeletedAsync(string chapterId)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId) ?? throw new BaseException.BadRequestException("Not Found", "Chapter ID not found");
            if (chapter.DeletedBy == null)
            {
                throw new BaseException.BadRequestException("announcement", "This chapter was rollback");
            }
            if (chapter.Status == false)
            {
                chapter.Status = true;
            }
            chapter.DeletedBy = null;
            chapter.DeletedTime = null;

            _userServices.AuditFields(chapter);

            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
            User? deleteBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.DeletedBy ?? string.Empty);
            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            return new ChapterAdminDelete
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                SubjectName = subject?.SubjectName ?? string.Empty,
                QuizId = chapter.QuizId,
                QuizName = quiz?.QuizName ?? string.Empty,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                //DeletedBy = chapter.DeletedBy,
                //DeleteChapterBy = deleteBy?.FullName ?? string.Empty,
                //DeletedTime = chapter.DeletedTime,
            };
        }

        // Get one order with all properties
        public async Task<ChapterAdminViewDto?> GetChapterByChapterIdAsync(string Id)
        {
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(Id);

            if (chapter == null || chapter.Status == false) return null;

            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            ChapterAdminViewDto dto = new ChapterAdminViewDto {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                SubjectName = subject?.SubjectName ?? string.Empty,
                QuizId = chapter.QuizId,
                QuizName = quiz?.QuizName ?? string.Empty,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
            };

            return dto;
        }

        public async Task<ChapterViewDto?> GetChapterDtoByChapterIdAsync(string Id)
        {
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(Id);

            if (chapter == null || chapter.Status == false) return null;

            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            ChapterViewDto dto = new ChapterViewDto { 
                Id = chapter.Id, 
                Number = chapter.Number, 
                ChapterName = chapter.ChapterName, 
                Status = chapter.Status, 
                SubjectId = chapter.SubjectId, 
                SubjectName = subject?.SubjectName ?? string.Empty, 
                QuizId = chapter.QuizId, 
                QuizName = quiz?.QuizName ?? string.Empty 
            };

            return dto;
        }
        public async Task<BasePaginatedList<ChapterViewDto>> GetChaptersBySubjectIdAsync(int pageNumber, int pageSize, string subjectId)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(subjectId) ?? throw new BaseException.BadRequestException("Not Found", "Subject ID not found");

            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => c.SubjectId == subjectId && c.DeletedBy == null);

            List<ChapterViewDto> chapterView = [];

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();

                foreach (var chapter in allChapters)
                {
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                    ChapterViewDto dto = new()
                        {
                            Id = chapter.Id,
                            Number = chapter.Number,
                            ChapterName = chapter.ChapterName,
                            Status = chapter.Status,
                            SubjectId = chapter.SubjectId,
                            SubjectName = subject.SubjectName,
                            QuizId = chapter.QuizId,
                            QuizName = quiz?.QuizName ?? string.Empty,
                        //CreatedBy = chapter.CreatedBy,
                        //CreatedTime = chapter.CreatedTime,
                        //LastUpdatedBy = chapter.LastUpdatedBy,
                        //LastUpdatedTime = chapter.LastUpdatedTime,
                        //DeletedBy = chapter.DeletedBy,
                        //DeletedTime = chapter.DeletedTime,
                    };
                        chapterView.Add(dto);

                }
                return new BasePaginatedList<ChapterViewDto>(chapterView, chapterView.Count, 1, chapterView.Count);
            }

            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            foreach (var chapter in paginatedChapters.Items)
            {
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                ChapterViewDto dto = new()
                    {
                        Id = chapter.Id,
                        Number = chapter.Number,
                        ChapterName = chapter.ChapterName,
                        Status = chapter.Status,
                        SubjectId = chapter.SubjectId,
                        SubjectName = subject.SubjectName,
                        QuizId = chapter.QuizId,
                        QuizName = quiz?.QuizName ?? string.Empty,
                    //CreatedBy = chapter.CreatedBy,
                    //CreatedTime = chapter.CreatedTime,
                    //LastUpdatedBy = chapter.LastUpdatedBy,
                    //LastUpdatedTime = chapter.LastUpdatedTime,
                    //DeletedBy = chapter.DeletedBy,
                    //DeletedTime = chapter.DeletedTime,
                };
                    chapterView.Add(dto);
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
                        User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                        User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                        var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                        var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                        ChapterAdminViewDto dto = new()
                        {
                            Id = chapter.Id,
                            Number = chapter.Number,
                            ChapterName = chapter.ChapterName,
                            Status = chapter.Status,
                            SubjectId = chapter.SubjectId,
                            SubjectName = subject?.SubjectName ?? string.Empty,
                            QuizId = chapter.QuizId,
                            QuizName = quiz?.QuizName ?? string.Empty,
                            CreatedBy = chapter.CreatedBy,
                            CreatedTime = chapter.CreatedTime,
                            CreatorName = creator?.FullName ?? string.Empty,
                            CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                            LastUpdatedBy = chapter.LastUpdatedBy,
                            LastUpdatedTime = chapter.LastUpdatedTime,
                            LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                            LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                            //DeletedBy = chapter.DeletedBy,
                            //DeletedTime = chapter.DeletedTime,
                        };
                        chapterView.Add(dto);
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
                    User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                    User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                    ChapterAdminViewDto dto = new()
                    {
                        Id = chapter.Id,
                        Number = chapter.Number,
                        ChapterName = chapter.ChapterName,
                        Status = chapter.Status,
                        SubjectId = chapter.SubjectId,
                        QuizId = chapter.QuizId,
                        CreatedBy = chapter.CreatedBy,
                        CreatedTime = chapter.CreatedTime,
                        CreatorName = creator?.FullName ?? string.Empty,
                        CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                        LastUpdatedBy = chapter.LastUpdatedBy,
                        LastUpdatedTime = chapter.LastUpdatedTime,
                        LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                        LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                        //DeletedBy = chapter.DeletedBy,
                        //DeletedTime = chapter.DeletedTime,
                    };
                    chapterView.Add(dto);
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
                    User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                    User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                    User? deleteBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.DeletedBy ?? string.Empty);
                    ChapterAdminDelete dto = new()
                    {
                        Id = chapter.Id,
                        Number = chapter.Number,
                        ChapterName = chapter.ChapterName,
                        Status = chapter.Status,
                        SubjectId = chapter.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        QuizId = chapter.QuizId,
                        QuizName = quiz?.QuizName ?? string.Empty,
                        CreatedBy = chapter.CreatedBy,
                        CreatedTime = chapter.CreatedTime,
                        CreatorName = creator?.FullName ?? string.Empty,
                        CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                        LastUpdatedBy = chapter.LastUpdatedBy,
                        LastUpdatedTime = chapter.LastUpdatedTime,
                        LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                        LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                        DeletedBy = chapter.DeletedBy,
                        DeleteChapterBy = deleteBy?.FullName ?? string.Empty,
                        DeletedTime = chapter.DeletedTime,
                    };
                    chapterView.Add(dto);
                }
                return new BasePaginatedList<ChapterAdminDelete?>(chapterView, chapterView.Count, 1, chapterView.Count);
            }



            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);

            foreach (var chapter in paginatedChapters.Items)
            {

                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                ChapterAdminDelete dto = new()
                {
                    Id = chapter.Id,
                    Number = chapter.Number,
                    ChapterName = chapter.ChapterName,
                    Status = chapter.Status,
                    SubjectId = chapter.SubjectId,
                    SubjectName = subject?.SubjectName ?? string.Empty,
                    QuizId = chapter.QuizId,
                    QuizName = quiz?.QuizName ?? string.Empty,
                    CreatedBy = chapter.CreatedBy,
                    CreatedTime = chapter.CreatedTime,
                    CreatorName = creator?.FullName ?? string.Empty,
                    CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                    LastUpdatedBy = chapter.LastUpdatedBy,
                    LastUpdatedTime = chapter.LastUpdatedTime,
                    LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                    LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                    DeletedBy = chapter.DeletedBy,
                    DeletedTime = chapter.DeletedTime,
                };
                chapterView.Add(dto);
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
                    ChapterViewDto dto = new ChapterViewDto { 
                        Id = chapter.Id, 
                        Number = chapter.Number, 
                        ChapterName = chapter.ChapterName, 
                        Status = chapter.Status, 
                        SubjectId = chapter.SubjectId, 
                        SubjectName = subject?.SubjectName ?? string.Empty, 
                        QuizId = chapter.QuizId, 
                        QuizName = quiz?.QuizName ?? string.Empty
                    };

                    chapterDtos.Add(dto);
                }

                return new BasePaginatedList<ChapterViewDto?>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            // Show all chapters with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);
            foreach (var chapter in paginatedChapters.Items)
            {
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                ChapterViewDto dto = new ChapterViewDto { 
                    Id = chapter.Id, 
                    Number = chapter.Number, 
                    ChapterName = chapter.ChapterName, 
                    Status = chapter.Status, 
                    SubjectId = chapter.SubjectId, 
                    SubjectName = subject?.SubjectName ?? string.Empty, 
                    QuizId = chapter.QuizId, 
                    QuizName = quiz?.QuizName ?? string.Empty 
                };

                chapterDtos.Add(dto);
            }

            return new BasePaginatedList<ChapterViewDto?>(chapterDtos, paginatedChapters.TotalItems, pageNumber, pageSize);
        }

        // Change subject status and set LastUpdatedTime to current time
        public async Task<ChapterAdminViewDto> ChangeChapterStatusAsync(string id)
        {
            var chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(id) ?? throw new BaseException.BadRequestException("Not Found", "Chapter ID not found"); 
            chapter.Status = !chapter.Status;
            //subject.LastUpdatedTime = DateTime.UtcNow;

            _userServices.AuditFields(chapter);
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
            var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
            User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);

            _unitOfWork.GetRepository<Chapter>().Update(chapter);
            await _unitOfWork.GetRepository<Chapter>().SaveAsync();

            return new ChapterAdminViewDto
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                SubjectName = subject?.SubjectName ?? string.Empty,
                QuizId = chapter.QuizId,
                QuizName = quiz?.QuizName ?? string.Empty,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                //DeletedBy = chapter.DeletedBy,
                //DeletedTime = chapter.DeletedTime
            };
        }


        public async Task<bool> IsValidChapterAsync(string Id)
        {
            // Return true if chapter is not null
            return (await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(Id) is not null);
        }

        public async Task<BasePaginatedList<ChapterViewDto>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Chapter>().Entities.Where(c => c.Status == true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{searchTerm}%"));
            }

            List<ChapterViewDto> chapterView = [];

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();

                foreach (var chapter in allChapters)
                {
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                    ChapterViewDto dto = new()
                    {
                        Id = chapter.Id,
                        Number = chapter.Number,
                        ChapterName = chapter.ChapterName,
                        Status = chapter.Status,
                        SubjectId = chapter.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        QuizId = chapter.QuizId,
                        QuizName = quiz?.QuizName ?? string.Empty

                        //DeletedBy = chapter.DeletedBy,
                        //DeletedTime = chapter.DeletedTime,
                    };
                    chapterView.Add(dto);
                }
                return new BasePaginatedList<ChapterViewDto>(chapterView, chapterView.Count, 1, chapterView.Count);
            }

            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);

            foreach (var chapter in paginatedChapters.Items)
            {
                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                ChapterViewDto dto = new()
                {
                    Id = chapter.Id,
                    Number = chapter.Number,
                    ChapterName = chapter.ChapterName,
                    Status = chapter.Status,
                    SubjectId = chapter.SubjectId,
                    SubjectName = subject?.SubjectName ?? string.Empty,
                    QuizId = chapter.QuizId,
                    QuizName = quiz?.QuizName ?? string.Empty
                    //DeletedBy = chapter.DeletedBy,
                    //DeletedTime = chapter.DeletedTime,
                };
                chapterView.Add(dto);
            }

            return new BasePaginatedList<ChapterViewDto>(chapterView, paginatedChapters.TotalItems, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<ChapterAdminViewDto>> SearchChapterForAdminAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Chapter>().Entities;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{searchTerm}%"));
            }

            List<ChapterAdminViewDto> chapterView = [];

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();

                foreach (var chapter in allChapters)
                {
                    var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                    var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                    User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                    User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                    ChapterAdminViewDto dto = new()
                    {
                        Id = chapter.Id,
                        Number = chapter.Number,
                        ChapterName = chapter.ChapterName,
                        Status = chapter.Status,
                        SubjectId = chapter.SubjectId,
                        SubjectName = subject?.SubjectName ?? string.Empty,
                        QuizId = chapter.QuizId,
                        QuizName = quiz?.QuizName ?? string.Empty,
                        CreatedBy = chapter.CreatedBy,
                        CreatedTime = chapter.CreatedTime,
                        CreatorName = creator?.FullName ?? string.Empty,
                        CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                        LastUpdatedBy = chapter.LastUpdatedBy,
                        LastUpdatedTime = chapter.LastUpdatedTime,
                        LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                        LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                        //DeletedBy = chapter.DeletedBy,
                        //DeletedTime = chapter.DeletedTime,
                    };
                    chapterView.Add(dto);
                }
                return new BasePaginatedList<ChapterAdminViewDto>(chapterView, chapterView.Count, 1, chapterView.Count);
            }

            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);

            foreach (var chapter in paginatedChapters.Items)
            {
                var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(chapter.SubjectId);
                var quiz = chapter.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(chapter.QuizId) : null;
                User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.CreatedBy ?? string.Empty);
                User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(chapter.LastUpdatedBy ?? string.Empty);
                ChapterAdminViewDto dto = new()
                {
                    Id = chapter.Id,
                    Number = chapter.Number,
                    ChapterName = chapter.ChapterName,
                    Status = chapter.Status,
                    SubjectId = chapter.SubjectId,
                    SubjectName = subject?.SubjectName ?? string.Empty,
                    QuizId = chapter.QuizId,
                    QuizName = quiz?.QuizName ?? string.Empty,
                    CreatedBy = chapter.CreatedBy,
                    CreatedTime = chapter.CreatedTime,
                    CreatorName = creator?.FullName ?? string.Empty,
                    CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                    LastUpdatedBy = chapter.LastUpdatedBy,
                    LastUpdatedTime = chapter.LastUpdatedTime,
                    LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                    LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                    //DeletedBy = chapter.DeletedBy,
                    //DeletedTime = chapter.DeletedTime,
                };
                chapterView.Add(dto);
            }

            return new BasePaginatedList<ChapterAdminViewDto>(chapterView, paginatedChapters.TotalItems, pageNumber, pageSize);
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