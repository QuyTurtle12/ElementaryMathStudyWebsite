using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppChapterServices
    {
        Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize);

        Task<ChapterAdminViewDto> CreateChapterAsync(ChapterDto chapterDTO);

        Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterDto subjectDTO);

        Task<bool> DeleteChapter(string optionId);

       //Task<ChapterAdminViewDto> DeleteChapterAsync(string chapterId);
        Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize);

        Task<ChapterViewDto?> GetChapterDtoByChapterIdAsync(string Id);

        //Task<bool> IsValidChapterAsync(string Id);

        //Task<string?> IsGenerallyValidated(string subjectId, string studentId, string parentId, double totalPrice);

    }
}
