using ElementaryMathStudyWebsite.Contract.Core.IUOW;
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
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Http;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Services.Service.Authentication;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserAnswerService(
        IGenericRepository<UserAnswer> userAnswerRepository,
        IGenericRepository<Question> questionRepository,
        IGenericRepository<User> userRepository,
        IGenericRepository<Option> optionRepository,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService) : IAppUserAnswerServices
    {
        private readonly IGenericRepository<UserAnswer> _userAnswerRepository = userAnswerRepository ?? throw new ArgumentNullException(nameof(userAnswerRepository));
        private readonly IGenericRepository<Question> _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
        private readonly IGenericRepository<User> _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IGenericRepository<Option> _optionRepository = optionRepository ?? throw new ArgumentNullException(nameof(optionRepository));
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ITokenService _tokenService = tokenService;

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
            var userAnswer = await _userAnswerRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Cannot find user answer with ID '{id}'.");
            return new UserAnswerDTO
            {
                QuestionId = userAnswer.QuestionId,
                UserId = userAnswer.UserId,
                OptionId = userAnswer.OptionId
            };
        }

        public async Task<UserAnswerDTO> CreateUserAnswerAsync(UserAnswerDTO userAnswerDTO)
        {
            // Check if QuestionId exists
            if (!await _questionRepository.Entities.AnyAsync(q => q.Id == userAnswerDTO.QuestionId))
            {
                throw new KeyNotFoundException($"Question with ID '{userAnswerDTO.QuestionId}' not found.");
            }

            // Check if UserId exists
            if (!await _userRepository.Entities.AnyAsync(u => u.Id == userAnswerDTO.UserId))
            {
                throw new KeyNotFoundException($"User with ID '{userAnswerDTO.UserId}' not found.");
            }

            // Check if OptionId exists
            if (!await _optionRepository.Entities.AnyAsync(o => o.Id == userAnswerDTO.OptionId))
            {
                throw new KeyNotFoundException($"Option with ID '{userAnswerDTO.OptionId}' not found.");
            }

            var userAnswer = new UserAnswer
            {
                QuestionId = userAnswerDTO.QuestionId,
                UserId = userAnswerDTO.UserId,
                OptionId = userAnswerDTO.OptionId,
            };

            await _userAnswerRepository.InsertAsync(userAnswer);
            await _userAnswerRepository.SaveAsync();
            return userAnswerDTO;
        }

        public async Task<UserAnswerDTO> UpdateUserAnswerAsync(string id, UserAnswerDTO userAnswerDTO)
        {
            var userAnswer = await _userAnswerRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"User Answer with ID '{id}' not found.");

            // Check if QuestionId exists
            if (!await _questionRepository.Entities.AnyAsync(q => q.Id == userAnswerDTO.QuestionId))
            {
                throw new KeyNotFoundException($"Question with ID '{userAnswerDTO.QuestionId}' not found.");
            }

            // Check if UserId exists
            if (!await _userRepository.Entities.AnyAsync(u => u.Id == userAnswerDTO.UserId))
            {
                throw new KeyNotFoundException($"User with ID '{userAnswerDTO.UserId}' not found.");
            }

            // Check if OptionId exists
            if (!await _optionRepository.Entities.AnyAsync(o => o.Id == userAnswerDTO.OptionId))
            {
                throw new KeyNotFoundException($"Option with ID '{userAnswerDTO.OptionId}' not found.");
            }

            userAnswer.QuestionId = userAnswerDTO.QuestionId;
            userAnswer.UserId = userAnswerDTO.UserId;
            userAnswer.OptionId = userAnswerDTO.OptionId;

            await _userAnswerRepository.UpdateAsync(userAnswer);
            await _userAnswerRepository.SaveAsync();
            return userAnswerDTO;
        }

        public async Task<List<UserAnswerDTO>> GetUserAnswersByQuizAsync(string quizId)
        {
            var userId = GetCurrentUserID(); // Assume this returns the current user ID

            // Query user answers by user ID and quiz ID, ensuring Question is included
            IQueryable<UserAnswer> query = _userAnswerRepository.Entities
                .Include(ua => ua.Question) // Ensure questions are eagerly loaded
                .Where(ua => ua.UserId == userId && ua.Question != null && ua.Question.QuizId == quizId);

            var userAnswers = await query.ToListAsync();

            if (userAnswers == null || !userAnswers.Any())
            {
                throw new KeyNotFoundException($"No answers found for User ID '{userId}' in Quiz ID '{quizId}'.");
            }

            // Map the results to UserAnswerDTOs
            var userAnswerDtos = userAnswers.Select(userAnswer => new UserAnswerDTO
            {
                QuestionId = userAnswer.QuestionId,
                UserId = userAnswer.UserId,
                OptionId = userAnswer.OptionId
            }).ToList();

            return userAnswerDtos;
        }

        public string GetCurrentUserID()
        {
            // Retrieve the JWT token from the Authorization header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var currentUserId = _tokenService.GetUserIdFromTokenHeader(token);

            return currentUserId.ToString().ToUpper();
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
