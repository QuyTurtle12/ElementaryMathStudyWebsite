using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Core.Services.IDomainService
{
    public interface ITopicService
    {

        Task<BasePaginatedList<Topic?>> GetAllExistTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic

        //Task<IEnumerable<object>> GetTopicsAsync(int pageNumber, int pageSize);
        //Task<IEnumerable<TopicViewDTO>> SearchTopicsAsync(string searchTerm, int pageNumber, int pageSize);
        //Task<bool> TopicExistsAsync(string id);

    }
}