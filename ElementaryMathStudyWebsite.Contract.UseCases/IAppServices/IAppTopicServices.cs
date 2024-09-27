using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppTopicServices
    {
        Task<TopicViewDto?> GetTopicByIdAsync(string id); // Tìm kiếm Topic bằng Topic's Name ( Lấy các thông tin cần thiết )
        Task<TopicAdminViewDto?> GetTopicAllByIdAsync(string id); // Tìm kiếm Topic bằng ID
        Task<TopicAdminViewDto> AddTopicAsync(TopicCreateDto topicCreateDto); // Tạo chủ đề
        Task<TopicAdminViewDto> UpdateTopicAsync(string id, TopicUpdateDto topicUpdateDto); // Cập nhật chủ đề
        Task<TopicDeleteDto> DeleteTopicAsync(string id);
        Task<TopicDeleteDto> RollBackTopicDeletedAsync(string Id);
        Task SwapTopicNumbersAsync(string topicId1, string topicId2);
        Task<List<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId); // Lấy danh sách chủ đề theo ChapterId
        Task<BasePaginatedList<object>> SearchTopicByNameAsync(string searchTerm, int pageNumber, int pageSize);
        Task<BasePaginatedList<TopicAdminViewDto>> GetAllExistTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic
        Task<BasePaginatedList<TopicAdminViewDto>> GetAllDeleteTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic đã delete
        Task<BasePaginatedList<TopicViewDto>> GetAllTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic
        //Task<IEnumerable<object>> GetTopicsAsync(int pageNumber, int pageSize);
        //Task<IEnumerable<TopicViewDTO>> SearchTopicsAsync(string searchTerm, int pageNumber, int pageSize);
        //Task<bool> TopicExistsAsync(string id);
    }
}