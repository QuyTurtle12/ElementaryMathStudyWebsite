using AutoMapper;
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
        private readonly IMapper _mapper;

        public OptionService(IAppUserServices userService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            _userService.AuditFields(option, true);

            await _unitOfWork.GetRepository<Option>().InsertAsync(option);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<OptionViewDto>(option);
        }

        public async Task<OptionViewDto> AddOption(string userId, OptionCreateDto createDto)
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

            _userService.AuditFields(userId, option, true);

            await _unitOfWork.GetRepository<Option>().InsertAsync(option);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<OptionViewDto>(option);
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

        // Get options of a question for general user
        public async Task<BasePaginatedList<OptionViewDto>> GetOptionDtosByQuestion(int pageNumber, int pageSize, string questionId)
        {

            if (!_unitOfWork.IsValid<Question>(questionId)) throw new BaseException.NotFoundException("not_found", "Question ID not found");

            IQueryable<Option> query = _unitOfWork.GetRepository<Option>().Entities.Where(q => q.QuestionId == questionId && q.DeletedBy == null);

            BasePaginatedList<Option> resultQuery =
                (pageNumber < 0 || pageSize < 0)
                ? await _unitOfWork.GetRepository<Option>().GetPagging(query, 1, query.Count())
                : await _unitOfWork.GetRepository<Option>().GetPagging(query, pageNumber, pageSize);

            var responseItems = resultQuery.Items.Select(_mapper.Map<OptionViewDto>).ToList();

            return new BasePaginatedList<OptionViewDto>(responseItems, resultQuery.TotalItems, resultQuery.CurrentPage, resultQuery.PageSize);
        }

        // Update an option
        public async Task<OptionViewDto> UpdateOption(OptionUpdateDto optionUpdateDto)
        {
            Option? option;

            if (_unitOfWork.IsValid<Option>(optionUpdateDto.Id))
                option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(optionUpdateDto.Id);
            else throw new BaseException.NotFoundException("not_found", "Option ID not found");

            option!.Answer = optionUpdateDto.Answer;
            option.IsCorrect = optionUpdateDto.IsCorrect;

            _userService.AuditFields(option, false);

            await _unitOfWork.SaveAsync();

            return _mapper.Map<OptionViewDto>(option);

        }

        public async Task<OptionViewDto> UpdateOption(string userId, OptionUpdateDto optionUpdateDto)
        {
            Option? option;
            Console.WriteLine(optionUpdateDto.Id);
            if (_unitOfWork.IsValid<Option>(optionUpdateDto.Id))
                option = await _unitOfWork.GetRepository<Option>().GetByIdAsync(optionUpdateDto.Id);
            else throw new BaseException.NotFoundException("not_found", "Option ID not found");

            option!.Answer = optionUpdateDto.Answer;
            option.IsCorrect = optionUpdateDto.IsCorrect;

            _userService.AuditFields(userId, option, false);

            await _unitOfWork.SaveAsync();

            return _mapper.Map<OptionViewDto>(option);

        }

        // Get an option by ID
        public async Task<OptionViewDto> GetOptionByIdAsync(string optionId)
        {
            // Check if the ID is valid
            if (!_unitOfWork.IsValid<Option>(optionId))
                throw new BaseException.NotFoundException("not_found", "Option ID not found");

            // Fetch the option entity
            Option? option = await _unitOfWork.GetRepository<Option>()
                .Entities
                .FirstOrDefaultAsync(o => o.Id == optionId);

            // Check if the option exists
            if (option == null)
                throw new BaseException.NotFoundException("not_found", "Option not found");

            // Map to DTO and return
            return _mapper.Map<OptionViewDto>(option);
        }

    }
}
