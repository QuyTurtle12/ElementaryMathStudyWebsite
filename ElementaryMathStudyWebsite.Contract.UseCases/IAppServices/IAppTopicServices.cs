using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppTopicServices
    {
        Task<TopicViewDto?> GetTopicByIdAsync(string id); // Tìm kiếm Topic bằng Topic's Name ( Lấy các thông tin cần thiết )
        Task<TopicAdminViewDto?> GetTopicAllByIdAsync(string id); // Tìm kiếm Topic bằng ID
        Task<bool> AddTopicAsync(TopicCreateDto topicCreateDto); // Tạo chủ đề
        Task<bool> UpdateTopicAsync(string id, TopicCreateDto topicCreateDto); // Cập nhật chủ đề
        Task<TopicCreateDto> DeleteTopicAsync(string id);
        Task<List<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId); // Lấy danh sách chủ đề theo ChapterId
        Task<BasePaginatedList<object>> SearchTopicByNameAsync(string searchTerm, int pageNumber, int pageSize);

        Task<BasePaginatedList<Topic?>> GetAllExistTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic
        Task<bool> CanAccessTopicAsync(string topicId);
        Task<string> GetTopicNameAsync(string topicId);

        Task<BasePaginatedList<TopicViewDto>> GetAllTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic
        //Task<IEnumerable<object>> GetTopicsAsync(int pageNumber, int pageSize);
        //Task<IEnumerable<TopicViewDTO>> SearchTopicsAsync(string searchTerm, int pageNumber, int pageSize);
        //Task<bool> TopicExistsAsync(string id);
    }
}
