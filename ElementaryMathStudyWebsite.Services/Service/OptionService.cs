using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OptionService : IOptionService, IAppOptionServices
    {
        private readonly IGenericRepository<Option> _optionReposiotry;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public OptionService(IGenericRepository<Option> optionReposiotry, IGenericRepository<Question> questionRepository, IUserService userService,IUnitOfWork unitOfWork)
        {
            _optionReposiotry = optionReposiotry;
            _questionRepository = questionRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        // Add option to database
        public async Task<OptionViewDto> AddOption(OptionCreateDto createDto)
        {
            var question = await _questionRepository.GetByIdAsync(createDto.QuestionId) ?? throw new KeyNotFoundException("Invalid question ID");

            Option option = new()
            {
                QuestionId = createDto.QuestionId,
                Answer = createDto.Answer,
                IsCorrect = createDto.IsCorrect,
                CreatedTime = DateTimeOffset.Now,
                LastUpdatedTime = DateTimeOffset.Now
            };

            var appUserService = _userService as IAppUserServices;
            appUserService.AuditFields(option, true);

            await _optionReposiotry.InsertAsync(option);
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
                var option = await _optionReposiotry.GetByIdAsync(optionId) ?? throw new KeyNotFoundException("Invalid option ID");

                await _optionReposiotry.DeleteAsync(optionId);
                await _unitOfWork.SaveAsync();
                return true;
        }

        // Get an option with all properties
        public async Task<Option?> GetOptionById(string optionId)
        {
            Option? option = await _optionReposiotry.GetByIdAsync(optionId) ?? throw new KeyNotFoundException("Invalid option ID");
            return option;
        }

        // Get options of a question for general user
        public async Task<BasePaginatedList<OptionViewDto?>> GetOptionDtosByQuestion(int pageNumber, int pageSize, string questionId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId) ?? throw new KeyNotFoundException("Invalid question ID");
            
            IQueryable<Option> query = _optionReposiotry.Entities.Where(q => q.QuestionId == questionId);
            List<OptionViewDto> optionViewDtos = new();

            //If params negative = show all
            if (pageNumber <= 0|| pageSize <= 0)
            {
                var allOptions = await query.ToListAsync();

                foreach(var option in allOptions)
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
            BasePaginatedList<Option>? paginatedOptions = await _optionReposiotry.GetPagging(query, pageNumber, pageSize);

            foreach(var option in paginatedOptions.Items)
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
        public async Task<BasePaginatedList<Option?>> GetOptions(int pageNumber, int pageSize)
        {
            IQueryable<Option> query = _optionReposiotry.Entities;

            // Negative params = show all 
            if (pageNumber <= 0 || pageSize <= 0)
            {
                List<Option> allOptions = query.ToList();
                return new BasePaginatedList<Option>(allOptions, allOptions.Count, 1, allOptions.Count);
            }

            // Show with pagination
            return await _optionReposiotry.GetPagging(query, pageNumber, pageSize);
        }

        public async Task<bool> IsValidOption(string optionId)
        {
            return (await _optionReposiotry.GetByIdAsync(optionId) is not null);
        }

        // Update an option
        public async Task<OptionViewDto> UpdateOption(string optionId, OptionUpdateDto optionUpdateDto)
        {
            var option = await _optionReposiotry.GetByIdAsync(optionId) ?? throw new KeyNotFoundException("Invalid option ID");

            option.Answer = optionUpdateDto.Answer;
            option.IsCorrect = optionUpdateDto.IsCorrect;

            var appUserService = _userService as IAppUserServices;
            appUserService.AuditFields(option, false);

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
