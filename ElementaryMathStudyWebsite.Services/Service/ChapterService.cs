using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class ChapterService : IChapterService, IAppChapterServices
    {
        private readonly IGenericRepository<Chapter> _detailReposiotry;
        private readonly IGenericRepository<Chapter> _chapterRepository;
        private readonly IUnitOfWork _unitOfWork;

        // Constructor
        public ChapterService(IGenericRepository<Chapter> detailReposiotry, IGenericRepository<Chapter> chapterRepository, IUnitOfWork unitOfWork)
        {
            _detailReposiotry = detailReposiotry ?? throw new ArgumentNullException(nameof(detailReposiotry));
            _chapterRepository = chapterRepository ?? throw new ArgumentNullException(nameof(chapterRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> AddChapterAsync(ChapterCreateDto dto)
        {
            try
            {
                // Kiểm tra các giá trị đầu vào
                if (string.IsNullOrEmpty(dto.ChapterName) || string.IsNullOrEmpty(dto.SubjectId))
                {
                    return false;
                }

                Chapter chapter = new Chapter
                {
                    Number = dto.Number,
                    ChapterName = dto.ChapterName,
                    Status = dto.Status,
                    SubjectId = dto.SubjectId,
                    QuizId = dto.QuizId,
                    CreatedBy = dto.CreatedBy,
                    LastUpdatedBy = dto.LastUpdatedBy,
                    DeletedBy = dto.DeletedBy,
                    CreatedTime = dto.CreatedTime ?? DateTimeOffset.Now, // Sử dụng giá trị hiện tại nếu null
                    LastUpdatedTime = dto.LastUpdatedTime ?? DateTimeOffset.Now, // Sử dụng giá trị hiện tại nếu null
                    DeletedTime = dto.DeletedTime // Có thể null
                };

                await _chapterRepository.InsertAsync(chapter);
                await _unitOfWork.SaveAsync();

                return true; // Show that create chapter process is completed
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error occurred while adding chapter");
                return false;
            }
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

            ChapterViewDto dto = new ChapterViewDto(chapter.Number, chapter.ChapterName);

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
                    ChapterViewDto dto = new ChapterViewDto(chapter.Number, chapter.ChapterName);
                    chapterDtos.Add(dto);
                }
                return new BasePaginatedList<ChapterViewDto>(chapterDtos, chapterDtos.Count, 1, chapterDtos.Count);
            }

            // Show all chapters with pagination
            BasePaginatedList<Chapter> paginatedChapters = await _chapterRepository.GetPagging(query, pageNumber, pageSize);
            foreach (var chapter in paginatedChapters.Items)
            {
                //chapterDtos.Add(new ChapterViewDto(chapter.Number, chapter.ChapterName));
                ChapterViewDto dto = new ChapterViewDto(chapter.Number, chapter.ChapterName);
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


        // General Validation
        //public async Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId, double totalPrice)
        //{
        //    // Check if subject is existed
        //    if (!await _subjectService.IsValidSubjectAsync(subjectId)) return $"The subject Id {subjectId} is not exist";

        //    if (!await _userService.IsCustomerChildren(parentId, studentId)) return "They are not parents and children";

        //    if (totalPrice <= 0) return "Invalid price number";

        //    return null;
        //}

        // Check if order is exist
        public async Task<bool> IsValidChapterAsync(string Id)
        {
            // Return true if order is not null
            return (await _chapterRepository.GetByIdAsync(Id) is not null);
        }

        public Task<BasePaginatedList<ChapterViewDto>> searchChapterDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter)
        {
            throw new NotImplementedException();
        }

    }
}