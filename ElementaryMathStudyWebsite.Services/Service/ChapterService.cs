using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ChapterService(IGenericRepository<Chapter> detailReposiotry,
                          IGenericRepository<Chapter> chapterRepository,
                          IGenericRepository<Subject> subjectRepository,
                          IGenericRepository<Quiz> quizRepository,
                          IUnitOfWork unitOfWork,
                          IHttpContextAccessor httpContextAccessor,
                          ITokenService tokenService) : IAppChapterServices
    {
        private readonly IGenericRepository<Chapter> _detailReposiotry = detailReposiotry ?? throw new ArgumentNullException(nameof(detailReposiotry));
        private readonly IGenericRepository<Chapter> _chapterRepository = chapterRepository ?? throw new ArgumentNullException(nameof(chapterRepository));
        private readonly IGenericRepository<Subject> _subjectRepository = subjectRepository ?? throw new ArgumentNullException(nameof(subjectRepository));
        private readonly IGenericRepository<Quiz> _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ITokenService _tokenService = tokenService;
        //private readonly IAppUserServices _userService;

        private void ValidateChapter(ChapterDto chapterDTO)
        {
            if (chapterDTO.Number == null)
            {
                throw new ArgumentException("Number is required and cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(chapterDTO.ChapterName))
            {
                throw new ArgumentException("Chapter name is required and cannot be empty.");
            }
            if (!chapterDTO.Status)
            {
                throw new ArgumentException("Status is required and cannot be false.");
            }
            if (string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            {
                throw new ArgumentException("Subject Id is required and cannot be empty.");
            }
        }


        public async Task<ChapterAdminViewDto> CreateChapterAsync(ChapterDto chapterDTO)
        {
            ValidateChapter(chapterDTO);

            // Check if another chapter with the same name already exists
            var existingChapter = await _chapterRepository.Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName)
                .FirstOrDefaultAsync();

            if (existingChapter != null)
            {
                throw new InvalidOperationException($"A chapter with the name '{chapterDTO.ChapterName}' already exists.");
            }

            if (!string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            {
                var subjectExists = await _subjectRepository.Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new ArgumentException($"Subject with Id '{chapterDTO.SubjectId}' does not exist.");
                }
            }

            if (!string.IsNullOrWhiteSpace(chapterDTO.QuizId))
            {
                var quizExists = await _quizRepository.Entities
                    .AnyAsync(q => q.Id == chapterDTO.QuizId);

                if (!quizExists)
                {
                    throw new ArgumentException($"Quiz with Id '{chapterDTO.QuizId}' does not exist.");
                }
            }

            var chapter = new Chapter
            {
                Number = chapterDTO.Number,
                ChapterName = chapterDTO.ChapterName,
                Status = chapterDTO.Status,
                SubjectId = chapterDTO.SubjectId,
                QuizId = chapterDTO.QuizId,
                //CreatedTime = DateTime.UtcNow,
                //LastUpdatedTime = DateTime.UtcNow // Set initial LastUpdatedTime as well
            };

            AuditFields(chapter, isCreating: true);

            _chapterRepository.Insert(chapter);
            await _chapterRepository.SaveAsync();

            return new ChapterAdminViewDto
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                QuizId = chapter.QuizId,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                DeletedBy = chapter.DeletedBy,
                DeletedTime = chapter.DeletedTime
            };
        }

        public async Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterDto chapterDTO)
        {
            var chapter = await _chapterRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Subject with ID '{id}' not found.");

            // Check if another subject with the same name already exists
            var existingSubject = await _chapterRepository.Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName) // Exclude the current subject by its ID
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new InvalidOperationException($"A chapter with the name '{chapterDTO.ChapterName}' already exists.");
            }

            if (!string.IsNullOrWhiteSpace(chapterDTO.SubjectId))
            {
                var subjectExists = await _subjectRepository.Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new ArgumentException($"Subject with Id '{chapterDTO.SubjectId}' does not exist.");
                }
            }

            if (!string.IsNullOrWhiteSpace(chapterDTO.QuizId))
            {
                var quizExists = await _quizRepository.Entities
                    .AnyAsync(q => q.Id == chapterDTO.QuizId);

                if (!quizExists)
                {
                    throw new ArgumentException($"Quiz with Id '{chapterDTO.QuizId}' does not exist.");
                }
            }

            ValidateChapter(chapterDTO);

            chapter.Number = chapterDTO.Number;
            chapter.ChapterName = chapterDTO.ChapterName;
            chapter.Status = chapterDTO.Status;
            chapter.SubjectId = chapterDTO.SubjectId;
            chapter.QuizId = chapterDTO.QuizId;
            //chapter.LastUpdatedTime = DateTime.UtcNow;

            AuditFields(chapter, isCreating: false);

            _chapterRepository.Update(chapter);
            await _chapterRepository.SaveAsync();

            return new ChapterAdminViewDto
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                QuizId = chapter.QuizId,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                DeletedBy = chapter.DeletedBy,
                DeletedTime = chapter.DeletedTime
            };
        }

        public bool IsValidChapter(string chapterId)
        {
            Chapter? chapter = _unitOfWork.GetRepository<Chapter>().GetById(chapterId);

            return (chapter is not null && chapter.DeletedBy is null);
        }

        public async Task<bool> DeleteChapterAsync(string chapterId)
        {
            //var chapter = await _chapterRepository.GetByIdAsync(chapterId) ?? throw new KeyNotFoundException("Invalid chapter ID");

            //await _chapterRepository.DeleteAsync(chapterId);
            //await _unitOfWork.SaveAsync();
            //return true;
            Chapter? chapter = new();

            if (IsValidChapter(chapterId))
                chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(chapterId);
            else throw new KeyNotFoundException("Invalid chapter ID");

            AuditFields(chapter, isCreating: false);

            await _unitOfWork.SaveAsync();

            return true;
        }


        // Get one order with all properties
        public async Task<Chapter?> GetChapterByChapterIdAsync(string Id)
        {
            Chapter? chapter = await _chapterRepository.GetByIdAsync(Id);
            return chapter;
        }

        public async Task<ChapterViewDto?> GetChapterDtoByChapterIdAsync(string Id)
        {
            Chapter? chapter = await _chapterRepository.GetByIdAsync(Id);

            if (chapter == null) return null;

            ChapterViewDto dto = new ChapterViewDto { Number = chapter.Number, ChapterName = chapter.ChapterName };

            return dto;
        }

        //public async Task<BasePaginatedList<ChapterDto>> GetChaptersBySubjectIdAsync(string subjectId, int pageNumber, int pageSize)
        //{
        //    if (string.IsNullOrWhiteSpace(subjectId))
        //    {
        //        throw new ArgumentException("Chapter ID cannot be empty.");
        //    }

        //    IQueryable<Chapter> query = _chapterRepository.Entities.Where(t => t.SubjectId == subjectId);

        //    int totalItems = await query.CountAsync();

        //    if (pageNumber <= 0 || pageSize <= 0)
        //    {
        //        var allChapters = await query.ToListAsync();
        //        var chapterDtos = await MapToTopicViewDtos(allChapters);
        //        return new BasePaginatedList<ChapterDto>(chapterDtos, totalItems, 1, totalItems);
        //    }

        //    var paginatedTopics = await _chapterRepository.GetPagging(query, pageNumber, pageSize);
        //    var chapterDtosPaginated = await MapToTopicViewDtos(paginatedTopics.Items);

        //    return new BasePaginatedList<TopicViewDto>(chapterDtosPaginated, totalItems, paginatedTopics.CurrentPage, paginatedTopics.PageSize);
        //}

        public async Task<BasePaginatedList<ChapterAdminViewDto>> GetChaptersBySubjectIdAsync(int pageNumber, int pageSize, string subjectId)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().GetByIdAsync(subjectId) ?? throw new KeyNotFoundException("Invalid subject ID");

            IQueryable<Chapter> query = _unitOfWork.GetRepository<Chapter>().Entities.Where(q => q.SubjectId == subjectId && q.DeletedBy == null);
            List<ChapterAdminViewDto> chapterView = [];

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();

                foreach (var chapter in allChapters)
                {
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
                        LastUpdatedBy = chapter.LastUpdatedBy,
                        LastUpdatedTime = chapter.LastUpdatedTime,
                        DeletedBy = chapter.DeletedBy,
                        DeletedTime = chapter.DeletedTime,
                    };
                    chapterView.Add(dto);
                }
                return new BasePaginatedList<ChapterAdminViewDto>(chapterView, chapterView.Count, 1, chapterView.Count);
            }

            // Show with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _unitOfWork.GetRepository<Chapter>().GetPagging(query, pageNumber, pageSize);

            foreach (var chapter in paginatedChapters.Items)
            {
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
                    LastUpdatedBy = chapter.LastUpdatedBy,
                    LastUpdatedTime = chapter.LastUpdatedTime,
                    DeletedBy = chapter.DeletedBy,
                    DeletedTime = chapter.DeletedTime,
                };
                chapterView.Add(dto);
            }

            return new BasePaginatedList<ChapterAdminViewDto>(chapterView, paginatedChapters.TotalItems, pageNumber, pageSize);
        }
        public async Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize)
        {
            // Get all chapters from database
            IQueryable<Chapter> query = _chapterRepository.Entities;
            List<ChapterViewDto> chapterDtos = new List<ChapterViewDto>();

            // If pageNumber or pageSize are 0 or negative, show all chapters without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();
                // Map chapters to ChapterViewDto
                foreach (var chapter in allChapters)
                {
                    ChapterViewDto dto = new ChapterViewDto { Number = chapter.Number, ChapterName = chapter.ChapterName };
                    chapterDtos.Add(dto);
                }
                return new BasePaginatedList<ChapterViewDto?>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            // Show all chapters with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _chapterRepository.GetPagging(query, pageNumber, pageSize);
            foreach (var chapter in paginatedChapters.Items)
            {
                //chapterDtos.Add(new ChapterViewDto(chapter.Number, chapter.ChapterName));
                ChapterViewDto dto = new ChapterViewDto { Number = chapter.Number, ChapterName = chapter.ChapterName };
                chapterDtos.Add(dto);
            }

            return new BasePaginatedList<ChapterViewDto?>(chapterDtos, paginatedChapters.TotalItems, pageNumber, pageSize);
        }

        // Change subject status and set LastUpdatedTime to current time
        public async Task<ChapterAdminViewDto> ChangeChapterStatusAsync(string id)
        {
            var chapter = await _chapterRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Chapter with ID '{id}' not found.");
            chapter.Status = !chapter.Status;
            //subject.LastUpdatedTime = DateTime.UtcNow;

            AuditFields(chapter);

            _chapterRepository.Update(chapter);
            await _chapterRepository.SaveAsync();

            return new ChapterAdminViewDto
            {
                Id = chapter.Id,
                Number = chapter.Number,
                ChapterName = chapter.ChapterName,
                Status = chapter.Status,
                SubjectId = chapter.SubjectId,
                QuizId = chapter.QuizId,
                CreatedBy = chapter.CreatedBy,
                CreatedTime = chapter.CreatedTime,
                LastUpdatedBy = chapter.LastUpdatedBy,
                LastUpdatedTime = chapter.LastUpdatedTime,
                DeletedBy = chapter.DeletedBy,
                DeletedTime = chapter.DeletedTime
            };
        }

        public async Task<BasePaginatedList<Chapter?>> GetChaptersAsync(int pageNumber, int pageSize)
        {
            // Get all chapters from database
            IQueryable<Chapter> query = _chapterRepository.Entities;

            // If pageNumber or pageSize are 0 or negative, show all chapters without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();
                return new BasePaginatedList<Chapter?>(allChapters, allChapters.Count, 1, allChapters.Count);
            }

            // Show all chapters with pagination
            var paginatedChapters = await _chapterRepository.GetPagging(query, pageNumber, pageSize);
            return paginatedChapters;
        }

        public async Task<bool> IsValidChapterAsync(string Id)
        {
            // Return true if chapter is not null
            return (await _chapterRepository.GetByIdAsync(Id) is not null);
        }

        public async Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _chapterRepository.Entities.Where(c => c.Status == true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.ChapterName, $"%{searchTerm}%"));
            }

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allChapter = await query.ToListAsync();
                var chapterDtos = allChapter.Select(c => new ChapterDto
                {
                    Number = c.Number,
                    ChapterName = c.ChapterName,
                    Status = c.Status,
                    SubjectId = c.SubjectId,
                    QuizId = c.QuizId,
                }).ToList();

                if (!chapterDtos.Any())
                {
                    throw new KeyNotFoundException($"No chapter found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            var paginatedChapters = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);
            var chapterDtosPaginated = paginatedChapters.Items.Select(c => new ChapterDto
            {
                Number = c.Number,
                ChapterName = c.ChapterName,
                Status = c.Status,
                SubjectId = c.SubjectId,
                QuizId = c.QuizId,
            }).ToList();

            if (!chapterDtosPaginated.Any())
            {
                throw new KeyNotFoundException($"No chapter found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(chapterDtosPaginated, chapterDtosPaginated.Count(), pageNumber, pageSize);
        }

        public async Task<string?> GetChapterNameAsync(string id)
        {
            try
            {
                Chapter? chapter = await _chapterRepository.GetByIdAsync(id);

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

        public void AuditFields(BaseEntity entity, bool isCreating = false)
        {
            // Retrieve the JWT token from the Authorization header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var currentUserId = _tokenService.GetUserIdFromTokenHeader(token);

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
    }
}