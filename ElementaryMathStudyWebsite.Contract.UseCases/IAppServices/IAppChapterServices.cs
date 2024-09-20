﻿using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppChapterServices
    {
        Task<BasePaginatedList<object>> SearchChapterAsync(string searchTerm, int pageNumber, int pageSize);

        Task<BasePaginatedList<ChapterAdminViewDto>> SearchChapterForAdminAsync(string searchTerm, int pageNumber, int pageSize);

        Task<ChapterAdminViewDto> CreateChapterAsync(ChapterDto chapterDTO);

        Task<ChapterAdminViewDto> UpdateChapterAsync(string id, ChapterDto subjectDTO);

        Task<ChapterAdminViewDto> DeleteChapterAsync(string optionId);
        Task<BasePaginatedList<ChapterViewDto?>> GetChapterDtosAsync(int pageNumber, int pageSize);

        Task<ChapterViewDto?> GetChapterDtoByChapterIdAsync(string Id);

        Task<string?> GetChapterNameAsync(string id);

        Task<BasePaginatedList<ChapterAdminViewDto?>> GetChaptersAsync(int pageNumber, int pageSize);

        Task<Chapter?> GetChapterByChapterIdAsync(string id);

        Task<ChapterAdminViewDto> ChangeChapterStatusAsync(string id);

        Task<BasePaginatedList<ChapterAdminViewDto>> GetChaptersBySubjectIdAsync(int pageNumber, int pageSize, string subjectId);
    }
}