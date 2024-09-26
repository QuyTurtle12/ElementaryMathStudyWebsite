﻿using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserAnswerServices
    {
        Task<List<UserAnswerWithDetailsDTO>> CreateUserAnswersAsync(UserAnswerCreateDTO userAnswerCreateDTO);
        Task<UserAnswerDTO> UpdateUserAnswerAsync(string id, UserAnswerDTO userAnswerDTO);
        Task<BasePaginatedList<UserAnswerWithDetailsDTO>> GetAllUserAnswersAsync(int pageNumber, int pageSize);
        Task<UserAnswerDTO> GetUserAnswerByIdAsync(string id);
        Task<BasePaginatedList<UserAnswerWithDetailsDTO>> GetUserAnswersByQuizIdAsync(string quizId);
    }
}
