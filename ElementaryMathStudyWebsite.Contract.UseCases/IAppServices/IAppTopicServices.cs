using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppTopicServices
    {
        Task<TopicViewDto?> GetTopicByIdAsync(string id); // Tìm kiếm Topic bằng Topic's Name ( Lấy các thông tin cần thiết )
        Task<Topic> GetTopicAllByIdAsync(string id); // Tìm kiếm Topic bằng ID
        Task<bool> AddTopicAsync(TopicCreateDto topicCreateDto); // Tạo chủ đề
        Task<bool> UpdateTopicAsync(string id, TopicDto topicDto); // Cập nhật chủ đề
        Task<BasePaginatedList<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId, int pageNumber, int pageSize); // Lấy danh sách chủ đề theo ChapterId
        Task<BasePaginatedList<object>> SearchTopicByNameAsync(string searchTerm, int pageNumber, int pageSize);
    }
}
