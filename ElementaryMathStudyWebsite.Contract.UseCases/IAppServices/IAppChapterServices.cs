using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppChapterServices
    {
        Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize);

        Task<ChapterCreateDto> CreateChapterAsync(ChapterDto chapterDTO);

        Task<ChapterCreateDto> UpdateChapterAsync(string id, ChapterDto subjectDTO);
        Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize);

        Task<ChapterViewDto?> GetChapterDtoByChapterIdAsync(string Id);

        Task<bool> IsValidChapterAsync(string Id);

        //Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId, double totalPrice);

    }
}
