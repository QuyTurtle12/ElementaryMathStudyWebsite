using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserAnswerService : IAppUserAnswerServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppResultService _resultService;

        public UserAnswerService(
            IUnitOfWork unitOfWork,
            IAppUserServices userService,
            IAppResultService resultService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _resultService = resultService;
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

        //public async Task<UserAnswerDTO> CreateUserAnswerAsync(UserAnswerDTO userAnswerDTO)
        //{
        //    // Check if QuestionId exists
        //    if (!await _unitOfWork.GetRepository<Question>().Entities.AnyAsync(q => q.Id == userAnswerDTO.QuestionId))
        //    {
        //        throw new BaseException.NotFoundException("question_not_found",$"Question with ID '{userAnswerDTO.QuestionId}' not found.");
        //    }

        //    // Check if UserId exists
        //    if (!await _unitOfWork.GetRepository<User>().Entities.AnyAsync(u => u.Id == userAnswerDTO.UserId))
        //    {
        //        throw new BaseException.NotFoundException("user_not_found",$"User with ID '{userAnswerDTO.UserId}' not found.");
        //    }

        //    // Check if OptionId exists
        //    if (!await _unitOfWork.GetRepository<Option>().Entities.AnyAsync(o => o.Id == userAnswerDTO.OptionId))
        //    {
        //        throw new BaseException.NotFoundException("option_not_found",$"Option with ID '{userAnswerDTO.OptionId}' not found.");
        //    }

        //    var userAnswer = new UserAnswer
        //    {
        //        QuestionId = userAnswerDTO.QuestionId,
        //        UserId = userAnswerDTO.UserId,
        //        OptionId = userAnswerDTO.OptionId,
        //        AttemptNumber = userAnswerDTO.AttemptNumber,
        //    };

        //    await _unitOfWork.GetRepository<UserAnswer>().InsertAsync(userAnswer);
        //    await _unitOfWork.GetRepository<UserAnswer>().SaveAsync();
        //    return userAnswerDTO;
        //}

        public async Task<ResultProgressDto> CreateUserAnswersAsync(UserAnswerCreateDTO userAnswerCreateDTO)
        {
            User currentUser = await _userService.GetCurrentUserAsync();
            var currentUserId = currentUser.Id;
            


            string? questionId = userAnswerCreateDTO.UserAnswerList.FirstOrDefault()?.QuestionId;

            string? quizId = _unitOfWork.GetRepository<Question>().Entities
                .Where(q => q.Id.Equals(questionId))
                .Select(q => q.QuizId)
                .FirstOrDefault();
            
            // Check for duplicate QuestionId entries in the user answer list
            var duplicateQuestionIds = userAnswerCreateDTO.UserAnswerList
                .GroupBy(ua => ua.QuestionId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateQuestionIds.Any())
            {
                var duplicateIds = string.Join(", ", duplicateQuestionIds);
                throw new BaseException.BadRequestException("duplicate_questions", $"Duplicate Question IDs found: {duplicateIds}");
            }

            var tempAttempNumber = 1;
            foreach (var userAnswerDTO in userAnswerCreateDTO.UserAnswerList)
            {
                // Check if QuestionId exists
                if (!await _unitOfWork.GetRepository<Question>().Entities.AnyAsync(q => q.Id == userAnswerDTO.QuestionId))
                {
                    throw new BaseException.NotFoundException("question_not_found", $"Question with ID '{userAnswerDTO.QuestionId}' not found.");
                }

                // Check if OptionId exists
                if (!await _unitOfWork.GetRepository<Option>().Entities.AnyAsync(o => o.Id == userAnswerDTO.OptionId))
                {
                    throw new BaseException.NotFoundException("option_not_found", $"Option with ID '{userAnswerDTO.OptionId}' not found.");
                }

                // Check if a user answer already exists for this question and user
                var existingUserAnswer = await _unitOfWork.GetRepository<UserAnswer>()
                                                .Entities
                                                .Where(ua => ua.QuestionId == userAnswerDTO.QuestionId && ua.UserId == currentUserId)
                                                .OrderByDescending(ua => ua.AttemptNumber)
                                                .FirstOrDefaultAsync();

                var attemptNumber = existingUserAnswer != null ? existingUserAnswer.AttemptNumber + 1 : 1;
                if(tempAttempNumber < attemptNumber)
                {
                    tempAttempNumber = attemptNumber;
                }

                var userAnswer = new UserAnswer
                {
                    QuestionId = userAnswerDTO.QuestionId,
                    OptionId = userAnswerDTO.OptionId,
                    UserId = currentUserId,
                    AttemptNumber = attemptNumber
                };

                await _unitOfWork.GetRepository<UserAnswer>().InsertAsync(userAnswer);
            }

            // Save all changes after the loop
            await _unitOfWork.GetRepository<UserAnswer>().SaveAsync();

            //var result = new List<UserAnswerWithDetailsDTO>();
            //foreach (var userAnswer in userAnswerCreateDTO.UserAnswerList)
            //{
            //    var question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(userAnswer.QuestionId);
            //    var option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(userAnswer.OptionId);

            //    var userAnswerWithDetails = new UserAnswerWithDetailsDTO
            //    {
            //        QuestionId = userAnswer.QuestionId,
            //        QuestionContent = question?.QuestionContext ?? "Unknown Question",
            //        UserId = currentUserId,
            //        UserFullName = currentUser.FullName ?? "Unknown User",
            //        OptionId = userAnswer.OptionId,
            //        OptionAnswer = option?.Answer ?? "Unknown Answer",
            //        AttemptNumber = tempAttempNumber,
            //    };
            //    result.Add(userAnswerWithDetails);
            //}

            ResultCreateDto resultCreateDto = new ResultCreateDto()
            {
                QuizId = quizId ?? string.Empty
            };

            return await _resultService.AddStudentResultAsync(resultCreateDto);

        }



        public async Task<UserAnswerDTO> UpdateUserAnswerAsync(string id, UserAnswerDTO userAnswerDTO)
        {
            var userAnswer = await _unitOfWork.GetRepository<UserAnswer>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"User Answer with ID '{id}' not found.");

            // Check if QuestionId exists
            if (!await _unitOfWork.GetRepository<Question>().Entities.AnyAsync(q => q.Id == userAnswerDTO.QuestionId))
            {
                throw new BaseException.NotFoundException("question_not_found", $"Question with ID '{userAnswerDTO.QuestionId}' not found.");
            }

            // Check if UserId exists
            if (!await _unitOfWork.GetRepository<User>().Entities.AnyAsync(u => u.Id == userAnswerDTO.UserId))
            {
                throw new BaseException.NotFoundException("user_not_found", $"User with ID '{userAnswerDTO.UserId}' not found.");
            }

            // Check if OptionId exists
            if (!await _unitOfWork.GetRepository<Option>().Entities.AnyAsync(o => o.Id == userAnswerDTO.OptionId))
            {
                throw new BaseException.NotFoundException("option_not_found", $"Option with ID '{userAnswerDTO.OptionId}' not found.");
            }

            userAnswer.QuestionId = userAnswerDTO.QuestionId;
            userAnswer.UserId = userAnswerDTO.UserId;
            userAnswer.OptionId = userAnswerDTO.OptionId;

            await _unitOfWork.GetRepository<UserAnswer>().UpdateAsync(userAnswer);
            await _unitOfWork.GetRepository<UserAnswer>().SaveAsync();
            return userAnswerDTO;
        }

        public async Task<BasePaginatedList<UserAnswerWithDetailsDTO>> GetUserAnswersByQuizIdAsync(string quizId)
        {
            User currentUser = await _userService.GetCurrentUserAsync();
            var currentUserId = currentUser.Id;

            // Fetch user answers for the given quizId and current user
            var userAnswers = await _unitOfWork.GetRepository<UserAnswer>().Entities
                                .Where(ua => ua.Question != null &&
                                             ua.Question.QuizId != null &&
                                             ua.Question.QuizId.ToLower() == quizId.ToLower() && // Use ToLower for case-insensitive comparison
                                             ua.UserId != null &&
                                             ua.UserId.ToLower() == currentUserId.ToLower()) // Use ToLower for case-insensitive comparison
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

            if(result.Count == 0)
            {
                throw new BaseException.NotFoundException("answer_not_found", "There is no answer in the user answer");
            }

            return new BasePaginatedList<UserAnswerWithDetailsDTO>(result, result.Count, 1, result.Count);
        }
    }
}
