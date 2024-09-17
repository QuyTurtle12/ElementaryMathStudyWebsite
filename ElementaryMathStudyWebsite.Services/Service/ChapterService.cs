using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ChapterService : IAppChapterServices
    {
        private readonly IGenericRepository<Chapter> _detailReposiotry;
        private readonly IGenericRepository<Chapter> _chapterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Quiz> _quizRepository;
        private readonly IGenericRepository<Subject> _subjectRepository;
        private readonly ILogger<ChapterService> _logger;

        // Constructor
        public ChapterService(IGenericRepository<Chapter> detailReposiotry, IGenericRepository<Chapter> chapterRepository, IUnitOfWork unitOfWork, IGenericRepository<Quiz> quizRepository, IGenericRepository<Subject> subjectRepository, ILogger<ChapterService> logger)
        {
            _detailReposiotry = detailReposiotry;
            _chapterRepository = chapterRepository;
            _unitOfWork = unitOfWork;
            _quizRepository = quizRepository;
            _subjectRepository = subjectRepository;
            _logger = logger;
        }

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

            if (!string.IsNullOrEmpty(chapterDTO.SubjectId))
            {
                var subjectExists = await _subjectRepository.Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new ArgumentException($"Subject with Id '{chapterDTO.SubjectId}' does not exist.");
                }
            }

            if (!string.IsNullOrEmpty(chapterDTO.QuizId))
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
                CreatedTime = DateTime.UtcNow,
                LastUpdatedTime = DateTime.UtcNow // Set initial LastUpdatedTime as well
            };

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
            var chapter = await _chapterRepository.GetByIdAsync(id);
            if (chapter == null)
            {
                throw new KeyNotFoundException($"Chapter with ID '{id}' not found.");
            }

            // Check if another subject with the same name already exists
            var existingSubject = await _chapterRepository.Entities
                .Where(c => c.ChapterName == chapterDTO.ChapterName) // Exclude the current subject by its ID
                .FirstOrDefaultAsync();

            if (existingSubject != null)
            {
                throw new InvalidOperationException($"A chapter with the name '{chapterDTO.ChapterName}' already exists.");
            }

            if (!string.IsNullOrEmpty(chapterDTO.SubjectId))
            {
                var subjectExists = await _subjectRepository.Entities
                    .AnyAsync(s => s.Id == chapterDTO.SubjectId);

                if (!subjectExists)
                {
                    throw new ArgumentException($"Subject with Id '{chapterDTO.SubjectId}' does not exist.");
                }
            }

            if (!string.IsNullOrEmpty(chapterDTO.QuizId))
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
            chapter.LastUpdatedTime = DateTime.UtcNow;

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


        public async Task<bool> DeleteChapterAsync(string chapterId)
        {
            var chapter = await _chapterRepository.GetByIdAsync(chapterId) ?? throw new KeyNotFoundException("Invalid chapter ID");

            await _chapterRepository.DeleteAsync(chapterId);
            await _unitOfWork.SaveAsync();
            return true;
        }


        // Get one order with all properties
        public async Task<Chapter> GetChapterByChapterIdAsync(string Id)
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
                return new BasePaginatedList<ChapterViewDto>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            // Show all chapters with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _chapterRepository.GetPagging(query, pageNumber, pageSize);
            foreach (var chapter in paginatedChapters.Items)
            {
                //chapterDtos.Add(new ChapterViewDto(chapter.Number, chapter.ChapterName));
                ChapterViewDto dto = new ChapterViewDto { Number = chapter.Number, ChapterName = chapter.ChapterName };
                chapterDtos.Add(dto);
            }

            return new BasePaginatedList<ChapterViewDto>(chapterDtos, paginatedChapters.TotalItems, pageNumber, pageSize);
        }





        public async Task<BasePaginatedList<Chapter?>> GetChaptersAsync(int pageNumber, int pageSize)
        {
            // Get all chapters from database
            IQueryable<Chapter> query = _chapterRepository.Entities;

            // If pageNumber or pageSize are 0 or negative, show all chapters without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allChapters = await query.ToListAsync();
                return new BasePaginatedList<Chapter>(allChapters, allChapters.Count, 1, allChapters.Count);
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

            if (!string.IsNullOrEmpty(searchTerm))
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
                Chapter chapter = await _chapterRepository.GetByIdAsync(id);

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