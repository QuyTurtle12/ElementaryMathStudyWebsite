﻿using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using AutoMapper;
using System.Collections.Generic;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserAnswerService : IAppUserAnswerServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IMapper _mapper;
        private readonly IAppResultService _resultService;
        private readonly IAppOrderDetailServices _orderDetailService;

        public UserAnswerService(
            IUnitOfWork unitOfWork,
            IAppUserServices userService,
            IAppResultService resultService, 
            IMapper mapper,
            IAppOrderDetailServices orderDetailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _resultService = resultService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _orderDetailService = orderDetailService;
        }

        public async Task<BasePaginatedList<UserAnswerWithDetailsDTO>> GetAllUserAnswersAsync(int pageNumber, int pageSize)
        {
            // Get the queryable for UserAnswer
            IQueryable<UserAnswer> query = _unitOfWork.GetRepository<UserAnswer>().Entities
                .Include(u => u.Question) // Include related Question entity
                .Include(u => u.Option)   // Include related Option entity
                .Include(u => u.User);    // Include related User entity

            // If page size is -1 or pagination parameters are invalid, fetch all data without pagination
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                List<UserAnswer> allUserAnswers = await query.ToListAsync();

                // Map to the detailed DTO using AutoMapper
                List<UserAnswerWithDetailsDTO> result = _mapper.Map<List<UserAnswerWithDetailsDTO>>(allUserAnswers);
                return new BasePaginatedList<UserAnswerWithDetailsDTO>(result, result.Count, 1, result.Count);
            }

            // Fetch paginated data
            BasePaginatedList<UserAnswer> paginatedUserAnswers = await _unitOfWork.GetRepository<UserAnswer>().GetPagging(query, pageNumber, pageSize);

            // Map paginated items to UserAnswerWithDetailsDTO using AutoMapper
            List<UserAnswerWithDetailsDTO> userAnswerDtosPaginated = _mapper.Map<List<UserAnswerWithDetailsDTO>>(paginatedUserAnswers.Items);

            // Return paginated results with DTOs
            return new BasePaginatedList<UserAnswerWithDetailsDTO>(userAnswerDtosPaginated, userAnswerDtosPaginated.Count, pageNumber, pageSize);
        }

        public async Task<UserAnswerDTO> GetUserAnswerByIdAsync(string id)
        {
            UserAnswer userAnswer = await _unitOfWork.GetRepository<UserAnswer>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"Cannot find user answer with ID '{id}'.");
            return _mapper.Map<UserAnswerDTO>(userAnswer);
        }

        public async Task<ResultProgressDto> CreateUserAnswersAsync(UserAnswerCreateDTO userAnswerCreateDTO)
        {
            User currentUser = await _userService.GetCurrentUserAsync();
            string currentUserId = currentUser.Id;

            string? questionId = userAnswerCreateDTO.UserAnswerList.FirstOrDefault()?.QuestionId;

            // Retrieve the quizId from the first question
            string? quizId = await _unitOfWork.GetRepository<Question>()
                .Entities
                .Where(q => q.Id == questionId)
                .Select(q => q.QuizId)
                .FirstOrDefaultAsync();

            // Ensure that all questions belong to the same quiz
            var quizIds = userAnswerCreateDTO.UserAnswerList
                .Select(ua => _unitOfWork.GetRepository<Question>()
                    .Entities
                    .Where(q => q.Id == ua.QuestionId)
                    .Select(q => q.QuizId)
                    .FirstOrDefault())
                .Distinct()
                .ToList();

            if (quizIds.Count > 1)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Cannot answer different quizzes.");
            }

            // Retrieve the subjectId from the quiz, either via Chapter or Topic
            var subjectId = await _unitOfWork.GetRepository<Chapter>()
                .Entities
                .Where(c => c.QuizId == quizId)
                .Select(c => c.SubjectId)
                .FirstOrDefaultAsync();

            if (subjectId == null)
            {
                // If the quiz is not found in Chapter, search in Topic to find Chapter and then Subject
                subjectId = await _unitOfWork.GetRepository<Topic>()
                    .Entities
                    .Where(t => t.QuizId == quizId)
                    .Select(t => t.Chapter!.SubjectId)
                    .FirstOrDefaultAsync();
            }

            // Ensure subjectId is found, otherwise throw error
            if (subjectId == null)
            {
                throw new BaseException.NotFoundException("subject_not_found", "Subject associated with the quiz not found.");
            }

            // Check if the student is assigned to the subject in the OrderDetail table
            var isAssigned = await _unitOfWork.GetRepository<OrderDetail>()
                .Entities
                .AnyAsync(od => od.SubjectId == subjectId && od.StudentId == currentUserId);

            if (!isAssigned)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Cannot answer a quiz of a subject not assigned to you.");
            }

            // Rest of the logic remains the same...

            // Check for duplicate QuestionId entries in the user answer list
            List<string> duplicateQuestionIds = userAnswerCreateDTO.UserAnswerList
                .GroupBy(ua => ua.QuestionId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateQuestionIds.Any())
            {
                string duplicateIds = string.Join(", ", duplicateQuestionIds);
                throw new BaseException.BadRequestException("duplicate_questions", $"Duplicate Question IDs found: {duplicateIds}");
            }

            List<string> questionIds = userAnswerCreateDTO.UserAnswerList.Select(ua => ua.QuestionId).ToList();
            List<string> optionIds = userAnswerCreateDTO.UserAnswerList.Select(ua => ua.OptionId).ToList();

            // Check if all question IDs exist
            List<string> existingQuestions = await _unitOfWork.GetRepository<Question>()
                .Entities
                .Where(q => questionIds.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();

            if (existingQuestions.Count != questionIds.Count)
            {
                string? missingQuestionId = questionIds.Except(existingQuestions).FirstOrDefault();
                throw new BaseException.NotFoundException("question_not_found", $"Question with ID '{missingQuestionId}' not found.");
            }

            // Check if all option IDs exist
            List<string> existingOptions = await _unitOfWork.GetRepository<Option>()
                .Entities
                .Where(o => optionIds.Contains(o.Id))
                .Select(o => o.Id)
                .ToListAsync();

            if (existingOptions.Count != optionIds.Count)
            {
                string? missingOptionId = optionIds.Except(existingOptions).FirstOrDefault();
                throw new BaseException.NotFoundException("option_not_found", $"Option with ID '{missingOptionId}' not found.");
            }

            // Check if all questions of the quiz are answered
            var totalQuestions = await _unitOfWork.GetRepository<Question>()
                .Entities
                .Where(q => q.QuizId == quizId)
                .CountAsync();

            if (userAnswerCreateDTO.UserAnswerList.Count() != totalQuestions)
            {
                throw new BaseException.BadRequestException("invalid_argument", "You need to answer all the questions.");
            }

            // Pre-fetch existing user answers to avoid querying inside the loop
            List<UserAnswer> userAnswers = await _unitOfWork.GetRepository<UserAnswer>()
                .Entities
                .Where(ua => questionIds.Contains(ua.QuestionId) && ua.UserId == currentUserId)
                .ToListAsync();

            foreach (var userAnswerDTO in userAnswerCreateDTO.UserAnswerList)
            {
                UserAnswer? existingUserAnswer = userAnswers
                    .Where(ua => ua.QuestionId == userAnswerDTO.QuestionId)
                    .OrderByDescending(ua => ua.AttemptNumber)
                    .FirstOrDefault();

                int attemptNumber = existingUserAnswer != null ? existingUserAnswer.AttemptNumber + 1 : 1;

                UserAnswer userAnswer = new UserAnswer
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

            ResultCreateDto resultCreateDto = new ResultCreateDto()
            {
                QuizId = quizId ?? string.Empty
            };

            return await _resultService.AddStudentResultAsync(resultCreateDto);
        }

        public async Task<ResultProgressDto> CreateUserAnswersUserAsync(UserAnswerCreateDTO userAnswerCreateDTO, string currentUserId)
        {
            //User currentUser = await _userService.GetCurrentUserAsync();
            //string currentUserId = userId;

            string? questionId = userAnswerCreateDTO.UserAnswerList.FirstOrDefault()?.QuestionId;

            // Retrieve the quizId from the first question
            string? quizId = await _unitOfWork.GetRepository<Question>()
                .Entities
                .Where(q => q.Id == questionId)
                .Select(q => q.QuizId)
                .FirstOrDefaultAsync();

            // Ensure that all questions belong to the same quiz
            var quizIds = userAnswerCreateDTO.UserAnswerList
                .Select(ua => _unitOfWork.GetRepository<Question>()
                    .Entities
                    .Where(q => q.Id == ua.QuestionId)
                    .Select(q => q.QuizId)
                    .FirstOrDefault())
                .Distinct()
                .ToList();

            if (quizIds.Count > 1)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Cannot answer different quizzes.");
            }

            // Retrieve the subjectId from the quiz, either via Chapter or Topic
            var subjectId = await _unitOfWork.GetRepository<Chapter>()
                .Entities
                .Where(c => c.QuizId == quizId)
                .Select(c => c.SubjectId)
                .FirstOrDefaultAsync();

            if (subjectId == null)
            {
                // If the quiz is not found in Chapter, search in Topic to find Chapter and then Subject
                subjectId = await _unitOfWork.GetRepository<Topic>()
                    .Entities
                    .Where(t => t.QuizId == quizId)
                    .Select(t => t.Chapter!.SubjectId)
                    .FirstOrDefaultAsync();
            }

            // Ensure subjectId is found, otherwise throw error
            if (subjectId == null)
            {
                throw new BaseException.NotFoundException("subject_not_found", "Subject associated with the quiz not found.");
            }

            // Check if the student is assigned to the subject in the OrderDetail table
            //var isAssigned = await _unitOfWork.GetRepository<OrderDetail>()
            //    .Entities
            //    .AnyAsync(od => od.SubjectId == subjectId && od.StudentId == currentUserId);
            var isAssigned = await _orderDetailService.IsOrderDetailExistsAsync(currentUserId, subjectId);

            if (!isAssigned)
            {
                throw new BaseException.BadRequestException("invalid_argument", "Cannot answer a quiz of a subject not assigned to you.");
            }

            // Check for duplicate QuestionId entries in the user answer list
            List<string> duplicateQuestionIds = userAnswerCreateDTO.UserAnswerList
                .GroupBy(ua => ua.QuestionId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateQuestionIds.Any())
            {
                string duplicateIds = string.Join(", ", duplicateQuestionIds);
                throw new BaseException.BadRequestException("duplicate_questions", $"Duplicate Question IDs found: {duplicateIds}");
            }

            List<string> questionIds = userAnswerCreateDTO.UserAnswerList.Select(ua => ua.QuestionId).ToList();
            List<string> optionIds = userAnswerCreateDTO.UserAnswerList.Select(ua => ua.OptionId).ToList();

            // Check if all question IDs exist
            List<string> existingQuestions = await _unitOfWork.GetRepository<Question>()
                .Entities
                .Where(q => questionIds.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();

            if (existingQuestions.Count != questionIds.Count)
            {
                string? missingQuestionId = questionIds.Except(existingQuestions).FirstOrDefault();
                throw new BaseException.NotFoundException("question_not_found", $"Question with ID '{missingQuestionId}' not found.");
            }

            // Check if all option IDs exist
            List<string> existingOptions = await _unitOfWork.GetRepository<Option>()
                .Entities
                .Where(o => optionIds.Contains(o.Id))
                .Select(o => o.Id)
                .ToListAsync();

            if (existingOptions.Count != optionIds.Count)
            {
                string? missingOptionId = optionIds.Except(existingOptions).FirstOrDefault();
                throw new BaseException.NotFoundException("option_not_found", $"Option with ID '{missingOptionId}' not found.");
            }

            // Check if all questions of the quiz are answered
            var totalQuestions = await _unitOfWork.GetRepository<Question>()
                .Entities
                .Where(q => q.QuizId == quizId)
                .CountAsync();

            if (userAnswerCreateDTO.UserAnswerList.Count() != totalQuestions)
            {
                throw new BaseException.BadRequestException("invalid_argument", "You need to answer all the questions.");
            }

            // Pre-fetch existing user answers to avoid querying inside the loop
            List<UserAnswer> userAnswers = await _unitOfWork.GetRepository<UserAnswer>()
                .Entities
                .Where(ua => questionIds.Contains(ua.QuestionId) && ua.UserId == currentUserId)
                .ToListAsync();

            foreach (var userAnswerDTO in userAnswerCreateDTO.UserAnswerList)
            {
                UserAnswer? existingUserAnswer = userAnswers
                    .Where(ua => ua.QuestionId == userAnswerDTO.QuestionId)
                    .OrderByDescending(ua => ua.AttemptNumber)
                    .FirstOrDefault();

                int attemptNumber = existingUserAnswer != null ? existingUserAnswer.AttemptNumber + 1 : 1;

                UserAnswer userAnswer = new UserAnswer
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

            ResultCreateDto resultCreateDto = new ResultCreateDto()
            {
                QuizId = quizId ?? string.Empty
            };

            return await _resultService.AddStudentResultUserAsync(resultCreateDto, currentUserId);
        }

        public async Task<UserAnswerDTO> UpdateUserAnswerAsync(string id, UserAnswerDTO userAnswerDTO)
        {
            UserAnswer userAnswer = await _unitOfWork.GetRepository<UserAnswer>().GetByIdAsync(id) ?? throw new KeyNotFoundException($"User Answer with ID '{id}' not found.");

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
            string currentUserId = currentUser.Id;

            // Check if quiz exists
            var quizExists = await _unitOfWork.GetRepository<Quiz>()
                .Entities
                .AnyAsync(q => q.Id == quizId);

            if (!quizExists)
            {
                throw new BaseException.NotFoundException("quiz_not_found", "Quiz Id not existed.");
            }

            // Fetch user answers for the given quizId and current user
            List<UserAnswer> userAnswers = await _unitOfWork.GetRepository<UserAnswer>().Entities
                                .Where(ua => ua.Question != null &&
                                             ua.Question.QuizId != null &&
                                             ua.Question.QuizId.ToLower() == quizId.ToLower() && // Use ToLower for case-insensitive comparison
                                             ua.UserId != null &&
                                             ua.UserId.ToLower() == currentUserId.ToLower()) // Use ToLower for case-insensitive comparison
                                .ToListAsync();

            // Return a list of user answers with contextual information
            List<UserAnswerWithDetailsDTO> result = new List<UserAnswerWithDetailsDTO>();
            foreach (var userAnswer in userAnswers)
            {
                Question? question = await _unitOfWork.GetRepository<Question>().GetByIdAsync(userAnswer.QuestionId);
                User? user = await _unitOfWork.GetRepository<User>().GetByIdAsync(userAnswer.UserId);
                Option? option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(userAnswer.OptionId);

                UserAnswerWithDetailsDTO userAnswerWithDetails = _mapper.Map<UserAnswerWithDetailsDTO>(userAnswer);
                userAnswerWithDetails.QuestionContent = question?.QuestionContext ?? "Unknown Question";
                userAnswerWithDetails.UserFullName = currentUser.FullName ?? "Unknown User";
                userAnswerWithDetails.OptionAnswer = option?.Answer ?? "Unknown Answer";
                userAnswerWithDetails.AttemptNumber = userAnswer.AttemptNumber;

                result.Add(userAnswerWithDetails);
            }

            if(result.Count == 0)
            {
                throw new BaseException.NotFoundException("answer_not_found", "There is no answer in the user answer");
            }

            return new BasePaginatedList<UserAnswerWithDetailsDTO>(result.OrderByDescending(x => x.AttemptNumber).ToList(), result.Count, 1, result.Count);
        }

        public async Task<List<UserAnswerDTO>> GetUserAnswersByUserAndQuestionsAsync(string userId, List<string> questionIds)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");

            if (questionIds == null || !questionIds.Any())
                throw new ArgumentException("Question IDs list cannot be null or empty.", nameof(questionIds));

            // Fetch UserAnswers based on userId and questionIds
            List<UserAnswer> userAnswers = await _unitOfWork.GetRepository<UserAnswer>()
                .Entities
                .Where(ua => ua.UserId == userId && questionIds.Contains(ua.QuestionId))
                .ToListAsync();

            // Map to DTO and return
            return _mapper.Map<List<UserAnswerDTO>>(userAnswers);
        }
    }
}
