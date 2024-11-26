using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppTopicServices
    {
        Task<TopicViewDto?> GetTopicByIdAsync(string id); // Tìm kiếm Topic bằng Topic's Name ( Lấy các thông tin cần thiết )
        Task<TopicAdminViewDto?> GetTopicAllByIdAsync(string id); // Tìm kiếm Topic bằng ID
        Task<TopicAdminViewDto> AddTopicAsync(TopicCreateDto topicCreateDto); // Tạo chủ đề
        Task<TopicAdminViewDto> UpdateTopicAsync(string id, TopicUpdateDto topicUpdateDto); // Cập nhật chủ đề
        Task<TopicAdminViewDto> UpdateQuizIdTopicAsync(string id, TopicUpdateQuizIdDto topicUpdateDto); // Cập nhật QuizId chủ đề
        Task<TopicDeleteDto> DeleteTopicAsync(string id);
        Task<TopicDeleteDto> RollBackTopicDeletedAsync(string Id);
        Task SwapTopicNumbersAsync(string topicId1, string topicId2);
        Task<List<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId); // Lấy danh sách chủ đề theo ChapterId
        Task<BasePaginatedList<TopicViewDto>> SearchTopicByNameAsync(string searchTerm, int pageNumber, int pageSize);
        Task<BasePaginatedList<TopicViewDto>> GetTopicsByChapterNameAsync(string chapterName, int pageNumber, int pageSize);
        Task<BasePaginatedList<TopicAdminViewDto>> GetAllExistTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic
        Task<BasePaginatedList<TopicAdminViewDto>> GetAllDeleteTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic đã delete
        Task<BasePaginatedList<TopicViewDto>> GetAllTopicsAsync(int pageNumber, int pageSize); // Lấy danh sách Topic

        Task<List<ChapterIdNameDto>> GetChaptersAllAsync();
        Task<List<string>> GetChapterNamesAsync();
        Task<List<Quiz>> GetQuizzesWithoutChapterOrTopicAsync();

        Task<Topic> AddTopicAllAsync(TopicCreateAllDto topicCreateAllDto);

        Task<TopicAdminViewDto> UpdateTopicAllAsync(string id, TopicCreateAllDto topicCreateAllDto);
        Task<TopicAdminViewDto> DeleteTopicRazorAsync(string id, TopicCreateAllDto topicCreateAllDto);
        //Task<IEnumerable<object>> GetTopicsAsync(int pageNumber, int pageSize);
        //Task<IEnumerable<TopicViewDTO>> SearchTopicsAsync(string searchTerm, int pageNumber, int pageSize);
        //Task<bool> TopicExistsAsync(string id);
    }
}