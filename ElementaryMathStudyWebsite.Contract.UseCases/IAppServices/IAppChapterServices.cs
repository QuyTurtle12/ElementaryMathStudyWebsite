using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppChapterServices
    {
        Task<BasePaginatedList<ChapterViewDto?>> searchChapterDtosAsync(int pageNumber, int pageSize, string? firstInputValue, string? secondInputValue, string filter);

        Task<bool> AddChapterAsync(ChapterCreateDto dto);

        Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize);

        Task<ChapterViewDto?> GetChapterDtoByChapterIdAsync(string Id);

        Task<bool> IsValidChapterAsync(string Id);

        //Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId, double totalPrice);

    }
}
