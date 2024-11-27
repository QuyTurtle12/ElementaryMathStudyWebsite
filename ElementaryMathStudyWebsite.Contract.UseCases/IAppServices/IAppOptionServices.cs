using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOptionServices
    {
        // Get options of a question for general user
        Task<BasePaginatedList<OptionViewDto>> GetOptionDtosByQuestion(int pageNumber, int pageSize, string questionId);

        // Add option to database
        Task<OptionViewDto> AddOption(OptionCreateDto optionCreateDto);
        Task<OptionViewDto> AddOption(string userId, OptionCreateDto optionCreateDto);

        // Edit an option by id
        Task<OptionViewDto> UpdateOption(OptionUpdateDto optionUpdateDto);
        Task<OptionViewDto> UpdateOption(string userId, OptionUpdateDto optionUpdateDto);


        //Delete an option
        Task<bool> DeleteOption(string optionId);
    }
}
