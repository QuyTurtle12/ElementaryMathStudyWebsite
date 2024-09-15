using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppChapterServices
    {
        Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize);

        Task<ChapterAdminViewDto> CreateChapterAsync(ChapterDto chapterDTO);

        Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterDto subjectDTO);

        Task<bool> DeleteChapterAsync(string optionId);
        Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize);

        Task<ChapterViewDto?> GetChapterDtoByChapterIdAsync(string Id);

    }
}
