using ElementaryMathStudyWebsite.Core.Repositories.Entity;

using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class TopicService : IAppTopicServices
    {
        private readonly IGenericRepository<Topic> _topicRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppQuizServices _quizService;
        private readonly IAppChapterServices _chapterService;

        public TopicService(IGenericRepository<Topic> topicRepository, IUnitOfWork unitOfWork, IAppQuizServices quizService, IAppChapterServices chapterService)
        {
            _topicRepository = topicRepository;
            _unitOfWork = unitOfWork;
            _quizService = quizService;
            _chapterService = chapterService;
        }

        // Lấy danh sách Topic có phân trang
        public async Task<BasePaginatedList<Topic?>> GetAllExistTopicsAsync(int pageNumber, int pageSize)
        {
            IQueryable<Topic> query = _topicRepository.Entities;

            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = query.ToList();
                return new BasePaginatedList<Topic>(allTopics, allTopics.Count, 1, allTopics.Count);
            }

            return await _topicRepository.GetPagging(query, pageNumber, pageSize);
        }

        // Lấy danh sách Topic theo ChapterId
        //public async Task<BasePaginatedList<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId, int pageNumber, int pageSize)
        //{
        //    // Kiểm tra ràng buộc chapterId
        //    ValidateChapterId(chapterId);

        //    // Truy vấn các topic theo chapterId
        //    IQueryable<Topic> query = _topicRepository.Entities.Where(t => t.ChapterId == chapterId);

        //    // Tính tổng số topic
        //    int totalItems = await query.CountAsync();

        //    // Nếu pageNumber hoặc pageSize không hợp lệ
        //    if (pageNumber <= 0 || pageSize <= 0)
        //    {
        //        var allTopics = await query.ToListAsync();
        //        var topicDtos = await MapToTopicViewDtos(allTopics);
        //        return new BasePaginatedList<TopicViewDto>(topicDtos, totalItems, 1, totalItems);
        //    }

        //    // Lấy danh sách topic phân trang
        //    var paginatedTopics = await _topicRepository.GetPagging(query, pageNumber, pageSize);
        //    var topicDtosPaginated = await MapToTopicViewDtos(paginatedTopics.Items);

        //    return new BasePaginatedList<TopicViewDto>(topicDtosPaginated, totalItems, paginatedTopics.CurrentPage, paginatedTopics.PageSize);
        //}

        // Phương thức để chuyển đổi Topic sang TopicViewDto
        //private async Task<List<TopicViewDto>> MapToTopicViewDtos(IEnumerable<Topic> topics)
        //{
        //    var topicDtos = new List<TopicViewDto>();

        //    foreach (var topic in topics)
        //    {
        //        var quizName = await _quizService.GetQuizNameAsync(topic.QuizId);
        //        var chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId);

        //        topicDtos.Add(new TopicViewDto
        //        {
        //            Number = topic.Number,
        //            TopicName = topic.TopicName,
        //            QuizName = quizName,
        //            ChapterName = chapterName
        //        });
        //    }

        //    return topicDtos;
        //}

        // Tìm kiếm Topic bằng ID
        public async Task<Topic> GetTopicAllByIdAsync(string id)
        {
            ValidateTopicId(id);
            Topic? topic = await _topicRepository.GetByIdAsync(id);
            return topic;
        }

        // Tìm kiếm Topic bằng Topic Id ( Lấy các thông tin cần thiết )
        public async Task<TopicViewDto?> GetTopicByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Topic Name cannot be empty.");
            }
            Topic? topic = await _topicRepository.GetByIdAsync(id);

            if (topic == null) throw new KeyNotFoundException($"Not Found.");

            string chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId);
            string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;

            return new TopicViewDto { Number = topic.Number, TopicName = topic.TopicName, QuizName = quizName, ChapterName = chapterName };
        }

        public async Task<BasePaginatedList<object>> SearchTopicByNameAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _topicRepository.Entities.Where(t => t.Status == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => EF.Functions.Like(t.TopicName, $"%{searchTerm}%"));
            }

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicDtos = allTopics.Select(t => new TopicDto
                {
                    TopicName = t.TopicName,
                    Status = t.Status,
                    QuizId = t.QuizId,
                    ChapterId = t.ChapterId
                }).ToList();

                if (!topicDtos.Any())
                {
                    throw new KeyNotFoundException($"No topics found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(topicDtos, topicDtos.Count, 1, topicDtos.Count);
            }

            var paginatedTopics = await _topicRepository.GetPagging(query, pageNumber, pageSize);
            var topicDtosPaginated = paginatedTopics.Items.Select(t => new TopicDto
            {
                TopicName = t.TopicName,
                Status = t.Status,
                QuizId = t.QuizId,
                ChapterId = t.ChapterId
            }).ToList();

            if (!topicDtosPaginated.Any())
            {
                throw new KeyNotFoundException($"No topics found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(topicDtosPaginated, topicDtosPaginated.Count, pageNumber, pageSize);
        }

        // Tạo 1 Topic mới
        public async Task<bool> AddTopicAsync(TopicCreateDto topicCreateDto)
        {
            ValidateTopicDto(topicCreateDto);
            Topic topic = new Topic
            {
                TopicName = topicCreateDto.TopicName,
                Number = topicCreateDto.Number,
                ChapterId = topicCreateDto.ChapterId,
                QuizId = topicCreateDto.QuizId,
                Status = true, // Hoặc tùy theo logic của bạn
                CreatedBy = topicCreateDto.CreatedByUser,
                LastUpdatedBy = topicCreateDto.LastUpdatedByUser,
                DeletedBy = topicCreateDto.DeletedByUser
            };

            await _topicRepository.InsertAsync(topic);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // Cập nhật 1 Topic
        public async Task<bool> UpdateTopicAsync(string id, TopicDto topicDto)
        {
            ValidateTopicId(id);
            Topic? existingTopic = await _topicRepository.GetByIdAsync(id);

            if (existingTopic == null) throw new KeyNotFoundException($"Not Found.");

            existingTopic.Number = topicDto.Number;
            existingTopic.TopicName = topicDto.TopicName;
            existingTopic.ChapterId = topicDto.ChapterId;
            existingTopic.QuizId = topicDto.QuizId;

            await _unitOfWork.SaveAsync();
            return true;
        }

        // Các phương thức kiểm tra ràng buộc
        private void ValidateTopicId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Topic ID cannot be empty.");
            }
        }

        private void ValidateTopicDto(TopicCreateDto topicCreateDto)
        {
            if (string.IsNullOrWhiteSpace(topicCreateDto.TopicName))
            {
                throw new ArgumentException("Topic Name cannot be empty.");
            }

            if (topicCreateDto.Number <= 0)
            {
                throw new ArgumentException("Number must be greater than 0.");
            }

            if (string.IsNullOrWhiteSpace(topicCreateDto.ChapterId))
            {
                throw new ArgumentException("Chapter ID cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(topicCreateDto.QuizId))
            {
                throw new ArgumentException("Quiz ID cannot be empty.");
            }

            // Kiểm tra người tạo, nếu cần
            if (string.IsNullOrWhiteSpace(topicCreateDto.CreatedByUser))
            {
                throw new ArgumentException("Created By User cannot be empty.");
            }
        }
        // Phương thức xác thực ChapterId
        private void ValidateChapterId(string chapterId)
        {
            if (string.IsNullOrWhiteSpace(chapterId))
            {
                throw new ArgumentException("Chapter ID cannot be empty.");
            }
        }

        public Task<BasePaginatedList<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}