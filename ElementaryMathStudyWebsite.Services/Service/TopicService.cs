﻿using ElementaryMathStudyWebsite.Core.Repositories.Entity;

using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Http;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Services.Service.Authentication;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class TopicService : IAppTopicServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppQuizServices _quizService;
        private readonly IAppChapterServices _chapterService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TopicService(IUnitOfWork unitOfWork, IAppQuizServices quizService, IAppChapterServices chapterService, IHttpContextAccessor httpContextAccessor,
                          ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _quizService = quizService;
            _chapterService = chapterService;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        // Lấy danh sách Topic ( Admin )
        public async Task<BasePaginatedList<TopicAdminViewDto>> GetAllExistTopicsAsync(int pageNumber, int pageSize)
        {
            IQueryable<Topic> query = _unitOfWork.GetRepository<Topic>().Entities;
            // Kiểm tra các tham số phân trang
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicAdminViewDtos = new List<TopicAdminViewDto>();

                foreach (var topic in allTopics)
                {
                    string quizName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.QuizId))
                    {
                        quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                    }

                    string chapterName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.ChapterId))
                    {
                        chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
                    }

                    string createdById = topic?.CreatedBy ?? string.Empty;
                    User? createdBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(createdById);

                    string lastUpdatedById = topic?.CreatedBy ?? string.Empty;
                    User? lastUpdatedBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(lastUpdatedById);

                    topicAdminViewDtos.Add(new TopicAdminViewDto
                    {
                        Id = topic!.Id,
                        Number = topic!.Number,
                        TopicName = topic.TopicName,
                        TopicContext = topic.TopicContext,
                        Status = topic.Status,
                        QuizName = quizName,
                        ChapterName = chapterName,
                        CreatedBy = createdBy?.FullName ?? string.Empty,
                        LastUpdatedBy = lastUpdatedBy?.FullName ?? string.Empty,
                        CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                        LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow,
                    });
                }

                // Trả về danh sách không phân trang
                return new BasePaginatedList<TopicAdminViewDto>(topicAdminViewDtos, topicAdminViewDtos.Count, 1, topicAdminViewDtos.Count);
            }
            var totalCount = await query.CountAsync();

            // Thực hiện phân trang
            var paginatedTopics = await _unitOfWork.GetRepository<Topic>().GetPagging(query, pageNumber, pageSize);

            // Chuyển đổi các Topic sang TopicViewDto
            var topicViewDtosPaginated = new List<TopicAdminViewDto>();

            foreach (var topic in paginatedTopics.Items)
            {
                string quizName = string.Empty;
                if (!string.IsNullOrWhiteSpace(topic.QuizId))
                {
                    quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                }

                string chapterName = string.Empty;
                if (!string.IsNullOrWhiteSpace(topic.ChapterId))
                {
                    chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
                }
                string createdById = topic?.CreatedBy ?? string.Empty;
                User? createdBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(createdById);

                string lastUpdatedById = topic?.CreatedBy ?? string.Empty;
                User? lastUpdatedBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(lastUpdatedById);
                topicViewDtosPaginated.Add(new TopicAdminViewDto
                {
                    Id = topic!.Id,
                    Number = topic!.Number,
                    TopicName = topic.TopicName,
                    TopicContext = topic.TopicContext,
                    QuizName = quizName,
                    ChapterName = chapterName,
                    CreatedBy = createdBy?.FullName ?? string.Empty,
                    LastUpdatedBy = lastUpdatedBy?.FullName ?? string.Empty,
                    CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                    LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
                });
            }

            // Trả về danh sách đã phân trang
            return new BasePaginatedList<TopicAdminViewDto>(topicViewDtosPaginated, totalCount, pageNumber, pageSize);
        }

        //Lấy danh sách Topic ( User )
        public async Task<BasePaginatedList<TopicViewDto>> GetAllTopicsAsync(int pageNumber, int pageSize)
        {
            //IQueryable<Topic> query = _topicRepository.Entities;
            IQueryable<Topic> query = _unitOfWork.GetRepository<Topic>().Entities;


            // Kiểm tra các tham số phân trang
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicViewDtos = new List<TopicViewDto>();

                foreach (var topic in allTopics)
                {
                    string quizName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.QuizId))
                    {
                        quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                    }

                    string chapterName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.ChapterId))
                    {
                        chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
                    }

                    topicViewDtos.Add(new TopicViewDto
                    {
                        Id = topic.Id,
                        Number = topic.Number,
                        TopicName = topic.TopicName,
                        TopicContext = topic.TopicContext,
                        QuizName = quizName,
                        ChapterName = chapterName
                    });
                }

                // Trả về danh sách không phân trang
                return new BasePaginatedList<TopicViewDto>(topicViewDtos, topicViewDtos.Count, 1, topicViewDtos.Count);
            }
            var totalCount = await query.CountAsync();

            // Thực hiện phân trang
            var paginatedTopics = await _unitOfWork.GetRepository<Topic>().GetPagging(query, pageNumber, pageSize);

            // Chuyển đổi các Topic sang TopicViewDto
            var topicViewDtosPaginated = new List<TopicViewDto>();

            foreach (var topic in paginatedTopics.Items)
            {
                string quizName = string.Empty;
                if (!string.IsNullOrWhiteSpace(topic.QuizId))
                {
                    quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                }

                string chapterName = string.Empty;
                if (!string.IsNullOrWhiteSpace(topic.ChapterId))
                {
                    chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
                }

                topicViewDtosPaginated.Add(new TopicViewDto
                {
                    Id = topic.Id,
                    Number = topic.Number,
                    TopicName = topic.TopicName,
                    TopicContext = topic.TopicContext,
                    QuizName = quizName,
                    ChapterName = chapterName
                });
            }

            // Trả về danh sách đã phân trang
            return new BasePaginatedList<TopicViewDto>(topicViewDtosPaginated, totalCount, pageNumber, pageSize);
        }
        // Tìm kiếm Topic bằng ID ( Admin )
        public async Task<TopicAdminViewDto?> GetTopicAllByIdAsync(string id)
        {
            ValidateTopicId(id);
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Topic ID cannot be empty.", nameof(id));
            }

            Topic? topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);

            if (topic == null)
            {
                throw new KeyNotFoundException($"Topic with ID '{id}' not found.");
            }

            if (string.IsNullOrWhiteSpace(topic.ChapterId))
            {
                throw new ArgumentException("Chapter ID cannot be empty.", nameof(topic.ChapterId));
            }

            if (string.IsNullOrWhiteSpace(topic.QuizId))
            {
                throw new ArgumentException("Quiz ID cannot be empty.", nameof(topic.QuizId));
            }

            string chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
            string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;

            return new TopicAdminViewDto
            {
                Id = topic.Id,
                Number = topic.Number,
                TopicName = topic.TopicName,
                TopicContext = topic.TopicContext,
                QuizName = quizName,
                ChapterName = chapterName
            };
        }

        // Tìm kiếm Topic bằng Topic Id ( User )
        public async Task<TopicViewDto?> GetTopicByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Topic ID cannot be empty.", nameof(id));
            }

            Topic? topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);

            if (topic == null)
            {
                throw new KeyNotFoundException($"Topic with ID '{id}' not found.");
            }

            if (string.IsNullOrWhiteSpace(topic.ChapterId))
            {
                throw new ArgumentException("Chapter ID cannot be empty.", nameof(topic.ChapterId));
            }

            if (string.IsNullOrWhiteSpace(topic.QuizId))
            {
                throw new ArgumentException("Quiz ID cannot be empty.", nameof(topic.QuizId));
            }

            string chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
            string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;

            return new TopicViewDto
            {
                Id = topic.Id,
                Number = topic.Number,
                TopicName = topic.TopicName,
                TopicContext = topic.TopicContext,
                QuizName = quizName,
                ChapterName = chapterName
            };
        }

        // Tìm kiếm Topic bằng Topic's Name
        public async Task<BasePaginatedList<object>> SearchTopicByNameAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Topic>().Entities.Where(t => t.Status == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => EF.Functions.Like(t.TopicName, $"%{searchTerm}%"));
            }

            // Check if pagination is needed
            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicViewDtos = new List<TopicViewDto>();

                foreach (var t in allTopics)
                {
                    string chapterName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(t.ChapterId))
                    {
                        chapterName = await _chapterService.GetChapterNameAsync(t.ChapterId) ?? string.Empty;
                    }
                    string quizName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(t.QuizId))
                    {
                        quizName = await _quizService.GetQuizNameAsync(t.QuizId) ?? string.Empty;
                    }

                    topicViewDtos.Add(new TopicViewDto
                    {
                        Id = t.Id,
                        Number = t.Number,
                        TopicName = t.TopicName,
                        TopicContext = t.TopicContext,
                        QuizName = quizName,
                        ChapterName = chapterName
                    });
                }

                if (!topicViewDtos.Any())
                {
                    throw new KeyNotFoundException($"No topics found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(topicViewDtos, topicViewDtos.Count, 1, topicViewDtos.Count);
            }

            var paginatedTopics = await _unitOfWork.GetRepository<Topic>().GetPagging(query, pageNumber, pageSize);
            var topicDtosPaginated = new List<TopicViewDto>();

            foreach (var t in paginatedTopics.Items)
            {
                string chapterName = string.Empty;
                if (!string.IsNullOrWhiteSpace(t.ChapterId))
                {
                    chapterName = await _chapterService.GetChapterNameAsync(t.ChapterId) ?? string.Empty;
                }
                string quizName = string.Empty;
                if (!string.IsNullOrWhiteSpace(t.QuizId))
                {
                    quizName = await _quizService.GetQuizNameAsync(t.QuizId) ?? string.Empty;
                }
                topicDtosPaginated.Add(new TopicViewDto
                {
                    Id = t.Id,
                    Number = t.Number,
                    TopicName = t.TopicName,
                    TopicContext = t.TopicContext,
                    QuizName = quizName,
                    ChapterName = chapterName
                });
            }

            if (!topicDtosPaginated.Any())
            {
                throw new KeyNotFoundException($"No topics found with name containing '{searchTerm}'.");
            }

            return new BasePaginatedList<object>(topicDtosPaginated, topicDtosPaginated.Count, pageNumber, pageSize);
        }

        // Lấy danh sách Topic theo ChapterId
        public async Task<List<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId)
        {
            ValidateChapterId(chapterId);

            var topics = await _unitOfWork.GetRepository<Topic>().Entities
                .Where(t => t.ChapterId == chapterId && t.Status)
                .ToListAsync();

            if (!topics.Any())
            {
                throw new KeyNotFoundException($"No topics found for Chapter ID '{chapterId}'.");
            }

            var topicViewDtos = new List<TopicViewDto>();

            foreach (var topic in topics)
            {
                string quizName = string.Empty;
                if (!string.IsNullOrWhiteSpace(topic.QuizId))
                {
                    quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                }

                var chapterName = await _chapterService.GetChapterNameAsync(chapterId) ?? string.Empty;

                topicViewDtos.Add(new TopicViewDto
                {
                    Id = topic.Id,
                    Number = topic.Number,
                    TopicName = topic.TopicName,
                    TopicContext = topic.TopicContext,
                    QuizName = quizName,
                    ChapterName = chapterName
                });
            }
            return topicViewDtos;
        }

        // Tạo 1 Topic mới
        public async Task<bool> AddTopicAsync(TopicCreateDto topicCreateDto)
        {
            ValidateTopicDto(topicCreateDto);
            Topic topic = new Topic
            {
                TopicName = topicCreateDto.TopicName,
                TopicContext = topicCreateDto.TopicContext,
                Number = topicCreateDto.Number,
                ChapterId = topicCreateDto.ChapterId,
                QuizId = topicCreateDto.QuizId,
                Status = true,
                CreatedBy = topicCreateDto.CreatedByUser,
                LastUpdatedBy = topicCreateDto.LastUpdatedByUser,
            };

            await _unitOfWork.GetRepository<Topic>().InsertAsync(topic);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // Cập nhật 1 Topic
        public async Task<bool> UpdateTopicAsync(string id, TopicCreateDto topicCreateDto)
        {
            ValidateTopicId(id);
            Topic? existingTopic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);

            if (existingTopic == null) throw new KeyNotFoundException($"Not Found.");

            existingTopic.TopicName = topicCreateDto.TopicName;
            existingTopic.TopicContext = topicCreateDto.TopicContext;
            existingTopic.Number = topicCreateDto.Number;
            existingTopic.ChapterId = topicCreateDto.ChapterId;
            existingTopic.QuizId = topicCreateDto.QuizId;
            existingTopic.Status = true;
            existingTopic.CreatedBy = topicCreateDto.CreatedByUser;
            existingTopic.LastUpdatedBy = topicCreateDto.LastUpdatedByUser;

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<TopicCreateDto> DeleteTopicAsync(string id)
        {
            // Lấy token từ header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var currentUserId = _tokenService.GetUserIdFromTokenHeader(token);

            // Tìm topic theo ID
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Topic with ID '{id}' not found.");

            // Kiểm tra xem topic đã bị xóa chưa
            if (topic.DeletedBy != null)
            {
                throw new InvalidOperationException("This topic was already deleted.");
            }

            // Đánh dấu topic là đã xóa
            topic.Status = false; // Đặt trạng thái là không hoạt động
            topic.DeletedBy = currentUserId.ToString().ToUpper(); // Lưu ID người đã xóa
            topic.DeletedTime = DateTime.UtcNow; // Ghi lại thời gian xóa

            // Ghi lại thông tin audit nếu cần thiết
            AuditFields(topic);

            // Cập nhật topic trong cơ sở dữ liệu
            _unitOfWork.GetRepository<Topic>().Update(topic);
            await _unitOfWork.GetRepository<Topic>().SaveAsync();

            // Trả về DTO cho topic đã xóa
            return new TopicCreateDto
            {
                Number = topic.Number,
                TopicName = topic.TopicName ?? string.Empty,
                TopicContext = topic.TopicContext,
                Status = topic.Status,
                QuizId = topic.QuizId,
                ChapterId = topic.ChapterId ?? string.Empty,
                CreatedByUser = topic.CreatedBy ?? string.Empty,
                CreatedTime = topic.CreatedTime,
                LastUpdatedByUser = topic.LastUpdatedBy ?? string.Empty,
                LastUpdatedTime = topic.LastUpdatedTime,
                DeletedById = topic.DeletedBy ?? string.Empty,
                DeletedTime = topic.DeletedTime
            };
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
        public void AuditFields(BaseEntity entity, bool isCreating = false)
        {
            // Retrieve the JWT token from the Authorization header
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var currentUserId = _tokenService.GetUserIdFromTokenHeader(token);

            // If creating a new entity, set the CreatedBy field
            if (isCreating)
            {
                entity.CreatedBy = currentUserId.ToString().ToUpper(); // Set the creator's ID
            }

            // Always set LastUpdatedBy and LastUpdatedTime fields
            entity.LastUpdatedBy = currentUserId.ToString().ToUpper(); // Set the current user's ID

            // If is not created then update LastUpdatedTime
            if (isCreating is false)
            {
                entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            }

        }

    }
}