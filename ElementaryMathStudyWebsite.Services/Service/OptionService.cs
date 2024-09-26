using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OptionService : IAppOptionServices
    {
        private readonly IAppUserServices _userService;
        private readonly IUnitOfWork _unitOfWork;

        public OptionService(IAppUserServices userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        // Add option to database
        public async Task<OptionViewDto> AddOption(OptionCreateDto createDto)
        {
            if (!_unitOfWork.IsValid<Question>(createDto.QuestionId)) throw new BaseException.NotFoundException("not_found", "Question ID not found");

            Option option = new()
            {
                QuestionId = createDto.QuestionId,
                Answer = createDto.Answer,
                IsCorrect = createDto.IsCorrect,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };

            var userService = _userService;
            userService.AuditFields(option, true);

            await _unitOfWork.GetRepository<Option>().InsertAsync(option);
            await _unitOfWork.SaveAsync();

            return new OptionViewDto
            {
                OptionId = option.Id,
                Answer = option.Answer,
                IsCorrect = option.IsCorrect
            };
        }

        //Delete an option
        public async Task<bool> DeleteOption(string optionId)
        {
            Option? option;

            if (_unitOfWork.IsValid<Option>(optionId))
                option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(optionId);
            else throw new BaseException.NotFoundException("not_found", "Option ID not found");

            _userService.AuditFields(option!, false, true);

            await _unitOfWork.SaveAsync();

            return true;
        }

        // Get an option with all properties
        public async Task<Option> GetOptionById(string optionId)
        {
            Option? option;

            if (_unitOfWork.IsValid<Option>(optionId))
                option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(optionId);
            else throw new BaseException.NotFoundException("not_found", "Option ID not found");

            return option!;
        }

        // Get options of a question for general user
        public async Task<BasePaginatedList<OptionViewDto>> GetOptionDtosByQuestion(int pageNumber, int pageSize, string questionId)
        {

            if (!_unitOfWork.IsValid<Question>(questionId)) throw new BaseException.NotFoundException("not_found", "Question ID not found");

            IQueryable<Option> query = _unitOfWork.GetRepository<Option>().Entities.Where(q => q.QuestionId == questionId && q.DeletedBy == null);
            List<OptionViewDto> optionViewDtos = [];

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allOptions = await query.ToListAsync();

                foreach (var option in allOptions)
                {
                    OptionViewDto dto = new()
                    {
                        OptionId = option.Id,
                        Answer = option.Answer,
                        IsCorrect = option.IsCorrect
                    };
                    optionViewDtos.Add(dto);
                }
                return new BasePaginatedList<OptionViewDto>(optionViewDtos, optionViewDtos.Count, 1, optionViewDtos.Count);
            }

            // Show with pagination
            BasePaginatedList<Option> paginatedOptions = await _unitOfWork.GetRepository<Option>().GetPagging(query, pageNumber, pageSize);

            foreach (var option in paginatedOptions.Items)
            {
                OptionViewDto dto = new()
                {
                    OptionId = option.Id,
                    Answer = option.Answer,
                    IsCorrect = option.IsCorrect
                };
                optionViewDtos.Add(dto);
            }

            return new BasePaginatedList<OptionViewDto>(optionViewDtos, paginatedOptions.TotalItems, pageNumber, pageSize);
        }

        // Get options with all properties
        public async Task<BasePaginatedList<Option>> GetOptions(int pageNumber, int pageSize)
        {
            IQueryable<Option> query = _unitOfWork.GetRepository<Option>().Entities.Where(q => q.DeletedBy == null);

            // Negative params = show all 
            if (pageNumber <= 0 || pageSize <= 0)
            {
                List<Option> allOptions = query.ToList();
                return new BasePaginatedList<Option>(allOptions, allOptions.Count, 1, allOptions.Count);
            }

            // Show with pagination
            return await _unitOfWork.GetRepository<Option>().GetPagging(query, pageNumber, pageSize);
        }

        // Update an option
        public async Task<OptionViewDto> UpdateOption(string optionId, OptionUpdateDto optionUpdateDto)
        {
            Option? option;

            if (_unitOfWork.IsValid<Option>(optionId))
                option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(optionId);
            else throw new BaseException.NotFoundException("not_found", "Option ID not found");

            option!.Answer = optionUpdateDto.Answer;
            option.IsCorrect = optionUpdateDto.IsCorrect;

            var userService = _userService;
            userService.AuditFields(option, false);

            await _unitOfWork.SaveAsync();

            return new OptionViewDto
            {
                OptionId = option.Id,
                Answer = option.Answer,
                IsCorrect = option.IsCorrect,
            };
        }
    }
}
