﻿using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserAnswerService(IGenericRepository<UserAnswer> userAnswerRepository) : IAppUserAnswerServices
    {
        private readonly IGenericRepository<UserAnswer> _userAnswerRepository = userAnswerRepository ?? throw new ArgumentNullException(nameof(userAnswerRepository)); // Assuming a generic repository is used

        public async Task<BasePaginatedList<object>> GetAllUserAnswersAsync(int pageNumber, int pageSize)
        {
            IQueryable<UserAnswer> query = _userAnswerRepository.Entities;

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allUserAnswers = await query.ToListAsync();
                var userAnswerDtos = allUserAnswers.Select(userAnswer => new UserAnswerDTO
                {
                    QuestionId = userAnswer.QuestionId,
                    UserId = userAnswer.UserId,
                    OptionId = userAnswer.OptionId
                }).ToList();

                return new BasePaginatedList<object>(userAnswerDtos, userAnswerDtos.Count, 1, userAnswerDtos.Count);
            }

            var paginatedUserAnswers = await _userAnswerRepository.GetPagging(query, pageNumber, pageSize);
            var userAnswerDtosPaginated = paginatedUserAnswers.Items.Select(userAnswer => new UserAnswerDTO
            {
                QuestionId = userAnswer.QuestionId,
                UserId = userAnswer.UserId,
                OptionId = userAnswer.OptionId
            }).ToList();


            return new BasePaginatedList<object>(userAnswerDtosPaginated, userAnswerDtosPaginated.Count, pageNumber, pageSize);
        }

        public async Task<UserAnswerDTO> GetUserAnswerByIdAsync(string id)
        {
            var userAnswer = await _userAnswerRepository.GetByIdAsync(id);
            return userAnswer == null
                ? throw new KeyNotFoundException($"Cannot find user answer with ID '{id}'.")
                : new UserAnswerDTO
            {
                QuestionId = userAnswer.QuestionId,
                UserId = userAnswer.UserId,
                OptionId = userAnswer.OptionId
            };
        }

        public async Task<UserAnswer> CreateUserAnswerAsync(UserAnswer userAnswer)
        {
            await _userAnswerRepository.InsertAsync(userAnswer);
            return userAnswer;
        }

        public async Task UpdateUserAnswerAsync(UserAnswer userAnswer)
        {
            await _userAnswerRepository.UpdateAsync(userAnswer);
        }

        //public async Task DeleteUserAnswerAsync(string id)
        //{
        //    var userAnswer = await _repository.GetByIdAsync(id);
        //    if (userAnswer != null)
        //    {
        //        await _repository.DeleteAsync(userAnswer);
        //    }
        //}
    }
}