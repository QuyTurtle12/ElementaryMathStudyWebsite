using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppOptionServices
    {
        // Get options of a question for general user
        Task<BasePaginatedList<OptionViewDto>> GetOptionDtosByQuestion(int pageNumber, int pageSize, string questionId);

        // Add option to database
        Task<OptionViewDto> AddOption(OptionCreateDto optionCreateDto);

        // Edit an option by id
        Task<OptionViewDto> UpdateOption(string optionId, OptionUpdateDto optionUpdateDto);

        //Check if the question is valid
        bool IsValidOption(string optionId);
        
        //Delete an option
        Task<bool> DeleteOption(string optionId);

        Task<BasePaginatedList<Option>> GetOptions(int pageNumber, int pageSize);

        // Get option with all properties
        Task<Option?> GetOptionById(string optionId);
    }
}
