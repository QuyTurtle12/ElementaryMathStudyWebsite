using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppChapterServices
    {
        Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize);

        Task<BasePaginatedList<object>> SearchChapterForAdminAsync(string searchTerm, int pageNumber, int pageSize);

        Task<ChapterViewDto> CreateChapterAsync(ChapterDto chapterDTO);

        Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterUpdateDto subjectDTO);

        Task<bool> DeleteChapterAsync(string optionId);
        Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize);

        Task<object> GetChapterDtoByChapterIdAsync(string Id);

        Task<string?> GetChapterNameAsync(string id);

        Task<BasePaginatedList<ChapterAdminViewDto?>> GetChaptersAsync(int pageNumber, int pageSize);

        Task<object> GetChapterByChapterIdAsync(string id);

        Task<ChapterAdminViewDto> ChangeChapterStatusAsync(string id);

        Task<bool> AssignQuizIdToChapterAsync(string chapterId, string quizId);

        //Task<bool> ChangeChapterOrderAsync(int currentChapterNumber, int newChapterNumber);

        Task<ChapterAdminViewDto> rollbackChapterDeletedAsync(string chapterId);

        Task<BasePaginatedList<ChapterAdminViewDto>> GetChaptersDeletedAsync(int pageNumber, int pageSize);

        Task<BasePaginatedList<ChapterViewDto>> GetChaptersBySubjectIdAsync(int pageNumber, int pageSize, string subjectId);
        Task<bool> UpdateChapterNumbersAsync(string subjectId, ChapterNumberDto chapterNumberDto);
    }
}