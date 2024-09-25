﻿using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using Microsoft.Extensions.Options;
using ElementaryMathStudyWebsite.Infrastructure.UOW;
using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Http;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Services.Service.Authentication;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserAnswerService : IAppUserAnswerServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;

        public UserAnswerService(
            IUnitOfWork unitOfWork,
            IAppUserServices userService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<BasePaginatedList<object>> GetAllUserAnswersAsync(int pageNumber, int pageSize)
        {
            IQueryable<UserAnswer> query = _unitOfWork.GetRepository<UserAnswer>().Entities;

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allUserAnswers = await query.ToListAsync();
                var userAnswerDtos = allUserAnswers.Select(userAnswer => new UserAnswerDTO
                {
                    QuestionId = userAnswer.QuestionId,
                    UserId = userAnswer.UserId,
                    OptionId = userAnswer.OptionId,
                    AttemptNumber = userAnswer.AttemptNumber,
                }).ToList();

                return new BasePaginatedList<object>(userAnswerDtos, userAnswerDtos.Count, 1, userAnswerDtos.Count);
            }

            var paginatedUserAnswers = await _unitOfWork.GetRepository<UserAnswer>().GetPagging(query, pageNumber, pageSize);
            var userAnswerDtosPaginated = paginatedUserAnswers.Items.Select(userAnswer => new UserAnswerDTO
            {
                QuestionId = userAnswer.QuestionId,
                UserId = userAnswer.UserId,
                OptionId = userAnswer.OptionId,
                AttemptNumber = userAnswer.AttemptNumber,
            }).ToList();


            return new BasePaginatedList<object>(userAnswerDtosPaginated, userAnswerDtosPaginated.Count, pageNumber, pageSize);
        }

        public async Task<UserAnswerDTO> GetUserAnswerByIdAsync(string id)
        {
            var userAnswer = await _unitOfWork.GetRepository<UserAnswer>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"Cannot find user answer with ID '{id}'.");
            return new UserAnswerDTO
            {
                QuestionId = userAnswer.QuestionId,
                UserId = userAnswer.UserId,
                OptionId = userAnswer.OptionId,
                AttemptNumber = userAnswer.AttemptNumber,
            };
        }

        public async Task<UserAnswerDTO> CreateUserAnswerAsync(UserAnswerDTO userAnswerDTO)
        {
            // Check if QuestionId exists
            if (!await _unitOfWork.GetRepository<Question>().Entities.AnyAsync(q => q.Id == userAnswerDTO.QuestionId))
            {
                throw new KeyNotFoundException($"Question with ID '{userAnswerDTO.QuestionId}' not found.");
            }

            // Check if UserId exists
            if (!await _unitOfWork.GetRepository<User>().Entities.AnyAsync(u => u.Id == userAnswerDTO.UserId))
            {
                throw new KeyNotFoundException($"User with ID '{userAnswerDTO.UserId}' not found.");
            }

            // Check if OptionId exists
            if (!await _unitOfWork.GetRepository<Option>().Entities.AnyAsync(o => o.Id == userAnswerDTO.OptionId))
            {
                throw new KeyNotFoundException($"Option with ID '{userAnswerDTO.OptionId}' not found.");
            }

            var userAnswer = new UserAnswer
            {
                QuestionId = userAnswerDTO.QuestionId,
                UserId = userAnswerDTO.UserId,
                OptionId = userAnswerDTO.OptionId,
                AttemptNumber = userAnswerDTO.AttemptNumber,
            };

            await _unitOfWork.GetRepository<UserAnswer>().InsertAsync(userAnswer);
            await _unitOfWork.GetRepository<UserAnswer>().SaveAsync();
            return userAnswerDTO;
        }

        public async Task<UserAnswerDTO> UpdateUserAnswerAsync(string id, UserAnswerDTO userAnswerDTO)
        {
            var userAnswer = await _unitOfWork.GetRepository<UserAnswer>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"User Answer with ID '{id}' not found.");

            // Check if QuestionId exists
            if (!await _unitOfWork.GetRepository<Question>().Entities.AnyAsync(q => q.Id == userAnswerDTO.QuestionId))
            {
                throw new KeyNotFoundException($"Question with ID '{userAnswerDTO.QuestionId}' not found.");
            }

            // Check if UserId exists
            if (!await _unitOfWork.GetRepository<User>().Entities.AnyAsync(u => u.Id == userAnswerDTO.UserId))
            {
                throw new KeyNotFoundException($"User with ID '{userAnswerDTO.UserId}' not found.");
            }

            // Check if OptionId exists
            if (!await _unitOfWork.GetRepository<Option>().Entities.AnyAsync(o => o.Id == userAnswerDTO.OptionId))
            {
                throw new KeyNotFoundException($"Option with ID '{userAnswerDTO.OptionId}' not found.");
            }

            userAnswer.QuestionId = userAnswerDTO.QuestionId;
            userAnswer.UserId = userAnswerDTO.UserId;
            userAnswer.OptionId = userAnswerDTO.OptionId;

            await _unitOfWork.GetRepository<UserAnswer>().UpdateAsync(userAnswer);
            await _unitOfWork.GetRepository<UserAnswer>().SaveAsync();
            return userAnswerDTO;
        }

        public async Task<List<UserAnswerWithDetailsDTO>> GetUserAnswersByQuizIdAsync(string quizId)
        {
            User currentUser = await _userService.GetCurrentUserAsync();
            var currentUserId = currentUser.Id;

            // Fetch user answers for the given quizId and current user
            var userAnswers = await _unitOfWork.GetRepository<UserAnswer>().Entities
            .Where(ua => ua.Question != null &&
                         ua.Question.QuizId != null &&
                         ua.Question.QuizId.Equals(quizId.ToString(), StringComparison.OrdinalIgnoreCase) &&
                         ua.UserId != null &&
                         ua.UserId.Equals(currentUserId.ToString(), StringComparison.OrdinalIgnoreCase))
            .ToListAsync();

            // Return a list of user answers with contextual information
            var result = new List<UserAnswerWithDetailsDTO>();
            foreach (var userAnswer in userAnswers)
            {
                var question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(userAnswer.QuestionId);
                var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(userAnswer.UserId);
                var option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(userAnswer.OptionId);

                var userAnswerWithDetails = new UserAnswerWithDetailsDTO
                {
                    QuestionId = userAnswer.QuestionId,
                    QuestionContent = question?.QuestionContext ?? "Unknown Question",
                    UserId = userAnswer.UserId,
                    UserFullName = user?.FullName ?? "Unknown User",
                    OptionId = userAnswer.OptionId,
                    OptionAnswer = option?.Answer ?? "Unknown Answer",
                    AttemptNumber = userAnswer.AttemptNumber
                };
                result.Add(userAnswerWithDetails);
            }

            return result;
        }
    }
}
