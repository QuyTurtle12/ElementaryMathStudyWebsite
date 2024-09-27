using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;


namespace ElementaryMathStudyWebsite.Services.Service
{
    public class TopicService : IAppTopicServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppQuizServices _quizService;
        private readonly IAppUserServices _userService;
        

        public TopicService(IUnitOfWork unitOfWork, IAppQuizServices quizService, IAppUserServices userService)
        {
            _unitOfWork = unitOfWork;
            _quizService = quizService;
            _userService = userService;
        }

        // Lấy danh sách Topic ( Admin )
        public async Task<BasePaginatedList<TopicAdminViewDto>> GetAllExistTopicsAsync(int pageNumber, int pageSize)
        {
            IQueryable<Topic> query = _unitOfWork.GetRepository<Topic>().Entities.Where(t => t.Status == true);

            // Kiểm tra các tham số phân trang
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicAdminViewDtos = new List<TopicAdminViewDto>();

                foreach (var topic in allTopics)
                {
                    if (topic != null)
                    {
                        string quizName = string.Empty;
                        if (!string.IsNullOrWhiteSpace(topic.QuizId))
                        {
                            quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                        }

                        string chapterName = string.Empty;
                        if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                        {
                            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                            chapterName = chapter!.ChapterName;
                        }

                        User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
                        User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);

                        topicAdminViewDtos.Add(new TopicAdminViewDto
                        {
                            Id = topic!.Id,
                            Number = topic.Number,
                            TopicName = topic.TopicName,
                            TopicContext = topic.TopicContext,
                            QuizId = topic.QuizId,
                            QuizName = quizName,
                            ChapterId = topic.ChapterId,
                            CreatedBy = topic?.CreatedBy ?? string.Empty,
                            CreatorName = creator?.FullName ?? string.Empty,
                            CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                            LastUpdatedBy = topic?.LastUpdatedBy ?? string.Empty,
                            LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                            LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                            CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                            LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
                        });
                    }
                }
                if (topicAdminViewDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("Not Found!", "Cannot found any topics");
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
                if (topic != null)
                {
                    string quizName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.QuizId))
                    {
                        quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                    }

                    string chapterName = string.Empty;
                    if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                    {
                        Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                        chapterName = chapter!.ChapterName;
                    }

                    User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
                    User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);
                    topicViewDtosPaginated.Add(new TopicAdminViewDto
                    {
                        Id = topic!.Id,
                        Number = topic.Number,
                        TopicName = topic.TopicName,
                        TopicContext = topic.TopicContext,
                        QuizId = topic.QuizId,
                        QuizName = quizName,
                        ChapterId = topic.ChapterId,
                        CreatedBy = topic?.CreatedBy ?? string.Empty,
                        CreatorName = creator?.FullName ?? string.Empty,
                        CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                        LastUpdatedBy = topic?.LastUpdatedBy ?? string.Empty,
                        LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                        LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                        CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                        LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
                    });
                }
            }
            if (topicViewDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("Not Found!", "Cannot found any topics");
            }
            // Trả về danh sách đã phân trang
            return new BasePaginatedList<TopicAdminViewDto>(topicViewDtosPaginated, totalCount, pageNumber, pageSize);
        }
        public async Task<BasePaginatedList<TopicAdminViewDto>> GetAllDeleteTopicsAsync(int pageNumber, int pageSize)
        {
            IQueryable<Topic> query = _unitOfWork.GetRepository<Topic>().Entities.Where(c => c.Status == false && c.DeletedBy != null && c.DeletedTime != null);

            // Kiểm tra các tham số phân trang
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicAdminViewDtos = new List<TopicAdminViewDto>();

                foreach (var topic in allTopics)
                {
                    if (topic != null)
                    {
                        string quizName = string.Empty;
                        if (!string.IsNullOrWhiteSpace(topic.QuizId))
                        {
                            quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                        }

                        string chapterName = string.Empty;
                        if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                        {
                            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                            chapterName = chapter!.ChapterName;
                        }

                        User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
                        User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);

                        topicAdminViewDtos.Add(new TopicAdminViewDto
                        {
                            Id = topic!.Id,
                            Number = topic.Number,
                            TopicName = topic.TopicName,
                            TopicContext = topic.TopicContext,
                            Status = topic.Status,
                            QuizId = topic.QuizId,
                            QuizName = quizName,
                            ChapterId = topic.ChapterId,
                            CreatedBy = topic?.CreatedBy ?? string.Empty,
                            CreatorName = creator?.FullName ?? string.Empty,
                            CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                            LastUpdatedBy = topic?.LastUpdatedBy ?? string.Empty,
                            LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                            LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                            CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                            LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
                        });
                    }
                }
                if (topicAdminViewDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("Not Found!", "Cannot find any topics that have been deleted.");
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
                if (topic != null)
                {
                    string quizName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.QuizId))
                    {
                        quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                    }

                    string chapterName = string.Empty;
                    if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                    {
                        Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                        chapterName = chapter!.ChapterName;
                    }
                    string createdById = topic?.CreatedBy ?? string.Empty;
                    User? createdBy = await _unitOfWork.GetRepository<User>().GetByIdAsync(createdById);

                    User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
                    User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);
                    topicViewDtosPaginated.Add(new TopicAdminViewDto
                    {
                        Id = topic!.Id,
                        Number = topic.Number,
                        TopicName = topic.TopicName,
                        TopicContext = topic.TopicContext,
                        QuizId = topic.QuizId,
                        QuizName = quizName,
                        ChapterId = topic.ChapterId,
                        CreatedBy = topic?.CreatedBy ?? string.Empty,
                        CreatorName = creator?.FullName ?? string.Empty,
                        CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                        LastUpdatedBy = topic?.LastUpdatedBy ?? string.Empty,
                        LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                        LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                        CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                        LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
                    });
                }
            }
            if (topicViewDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("Not Found!", "Cannot find any topics that have been deleted.");
            }
            // Trả về danh sách đã phân trang
            return new BasePaginatedList<TopicAdminViewDto>(topicViewDtosPaginated, totalCount, pageNumber, pageSize);
        }
        //Lấy danh sách Topic ( User )
        public async Task<BasePaginatedList<TopicViewDto>> GetAllTopicsAsync(int pageNumber, int pageSize)
        {
            //IQueryable<Topic> query = _topicRepository.Entities;
            IQueryable<Topic> query = _unitOfWork.GetRepository<Topic>().Entities.Where(t => t.Status == true);


            // Kiểm tra các tham số phân trang
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicViewDtos = new List<TopicViewDto>();

                foreach (var topic in allTopics)
                {
                    if (topic != null)
                    {
                        string quizName = string.Empty;
                        if (!string.IsNullOrWhiteSpace(topic.QuizId))
                        {
                            quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                        }

                        string chapterName = string.Empty;
                        if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                        {
                            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                            chapterName = chapter!.ChapterName;
                        }

                        topicViewDtos.Add(new TopicViewDto
                        {
                            Id = topic.Id,
                            Number = topic.Number,
                            TopicName = topic.TopicName,
                            TopicContext = topic.TopicContext,
                            QuizId = topic.QuizId,
                            QuizName = quizName,
                            ChapterId = topic.ChapterId,
                            ChapterName = chapterName
                        });
                    }
                }
                if (topicViewDtos.Count == 0)
                {
                    throw new BaseException.NotFoundException("Not Found!", "Cannot found any topics");
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
                if (topic != null)
                {
                    string quizName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.QuizId))
                    {
                        quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                    }

                    string chapterName = string.Empty;
                    if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                    {
                        Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                        chapterName = chapter!.ChapterName;
                    }
                    else chapterName = string.Empty;

                    topicViewDtosPaginated.Add(new TopicViewDto
                    {
                        Id = topic.Id,
                        Number = topic.Number,
                        TopicName = topic.TopicName,
                        TopicContext = topic.TopicContext,
                        QuizId = topic.QuizId,
                        QuizName = quizName,
                        ChapterId = topic.ChapterId,
                        ChapterName = chapterName
                    });
                }
            }
            if (topicViewDtosPaginated.Count == 0)
            {
                throw new BaseException.NotFoundException("Not Found!", "Cannot found any topics");
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
                throw new BaseException.BadRequestException("Topic ID cannot be empty.", nameof(id));
            }

            Topic? topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);

            if (topic == null || topic.Status == false)
            {
                throw new BaseException.NotFoundException("Not Found!", $"Topic with ID '{id}' not found.");
            }

            if (string.IsNullOrWhiteSpace(topic.ChapterId))
            {
                throw new BaseException.BadRequestException("Chapter ID cannot be empty.", nameof(topic.ChapterId));
            }

            if (string.IsNullOrWhiteSpace(topic.QuizId))
            {
                throw new BaseException.BadRequestException("Quiz ID cannot be empty.", nameof(topic.QuizId));
            }

            string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
            User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);

            return new TopicAdminViewDto
            {
                Id = topic!.Id,
                Number = topic.Number,
                TopicName = topic.TopicName,
                TopicContext = topic.TopicContext,
                QuizId = topic.QuizId,
                QuizName = quizName,
                ChapterId = topic.ChapterId,
                CreatedBy = topic?.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = topic?.LastUpdatedBy ?? string.Empty,
                LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty,
                CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
            };
        }

        // Tìm kiếm Topic bằng Topic Id ( User )
        public async Task<TopicViewDto?> GetTopicByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.BadRequestException("Topic ID cannot be empty.", nameof(id));
            }

            Topic? topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);

            if (topic == null || topic.Status == false)
            {
                throw new BaseException.NotFoundException("Not Found!", $"Topic with ID '{id}' not found.");
            }

            if (string.IsNullOrWhiteSpace(topic.ChapterId))
            {
                throw new BaseException.BadRequestException("Chapter ID cannot be empty.", nameof(topic.ChapterId));
            }

            if (string.IsNullOrWhiteSpace(topic.QuizId))
            {
                throw new BaseException.BadRequestException("Quiz ID cannot be empty.", nameof(topic.QuizId));
            }

            string chapterName = string.Empty;
            if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
            {
                Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                chapterName = chapter!.ChapterName;
            }

            string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;

            return new TopicViewDto
            {
                Id = topic.Id,
                Number = topic.Number,
                TopicName = topic.TopicName,
                TopicContext = topic.TopicContext,
                QuizId = topic.QuizId,
                QuizName = quizName,
                ChapterId = topic.ChapterId,
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

            if (pageSize == -1 || pageNumber <= 0 || pageSize <= 0)
            {
                var allTopics = await query.ToListAsync();
                var topicViewDtos = new List<TopicViewDto>();

                foreach (var topic in allTopics)
                {
                    string chapterName = string.Empty;
                    if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                    {
                        Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                        chapterName = chapter!.ChapterName;
                    }
                    string quizName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(topic.QuizId))
                    {
                        quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                    }

                    topicViewDtos.Add(new TopicViewDto
                    {
                        Id = topic.Id,
                        Number = topic.Number,
                        TopicName = topic.TopicName,
                        TopicContext = topic.TopicContext,
                        QuizId = topic.QuizId,
                        QuizName = quizName,
                        ChapterId = topic.ChapterId,
                        ChapterName = chapterName
                    });
                }

                if (!topicViewDtos.Any())
                {
                    throw new BaseException.NotFoundException("Not Found!", $"No topics found with name containing '{searchTerm}'.");
                }

                return new BasePaginatedList<object>(topicViewDtos, topicViewDtos.Count, 1, topicViewDtos.Count);
            }

            var paginatedTopics = await _unitOfWork.GetRepository<Topic>().GetPagging(query, pageNumber, pageSize);
            var topicDtosPaginated = new List<TopicViewDto>();

            foreach (var topic in paginatedTopics.Items)
            {
                string chapterName = string.Empty;
                if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                {
                    Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                    chapterName = chapter!.ChapterName;
                }
                string quizName = string.Empty;
                if (!string.IsNullOrWhiteSpace(topic.QuizId))
                {
                    quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                }
                topicDtosPaginated.Add(new TopicViewDto
                {
                    Id = topic.Id,
                    Number = topic.Number,
                    TopicName = topic.TopicName,
                    TopicContext = topic.TopicContext,
                    QuizId = topic.QuizId,
                    QuizName = quizName,
                    ChapterId = topic.ChapterId,
                    ChapterName = chapterName
                });
            }

            if (!topicDtosPaginated.Any())
            {
                throw new BaseException.NotFoundException("Not Found!", $"No topics found with name containing '{searchTerm}'.");
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
                throw new BaseException.NotFoundException("Not Found!", $"No topics found for Chapter ID '{chapterId}'.");
            }

            var topicViewDtos = new List<TopicViewDto>();

            foreach (var topic in topics)

            {
                string quizName = string.Empty;
                if (!string.IsNullOrWhiteSpace(topic.QuizId))
                {
                    quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
                }

                string chapterName = string.Empty;
                if (_unitOfWork.IsValid<Chapter>(topic.ChapterId))
                {
                    Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

                    chapterName = chapter!.ChapterName;
                }

                topicViewDtos.Add(new TopicViewDto
                {
                    Id = topic.Id,
                    Number = topic.Number,
                    TopicName = topic.TopicName,
                    TopicContext = topic.TopicContext,
                    QuizId = topic.QuizId,
                    QuizName = quizName,
                    ChapterId = topic.ChapterId,
                    ChapterName = chapterName
                });
            }
            if (topicViewDtos.Count == 0)
            {
                throw new BaseException.NotFoundException("Not Found!", "Cannot found any topics");
            }
            return topicViewDtos;
        }

        // Tạo 1 Topic ( Nếu New Topic trùng Number với Old Topic thì Number từ Old Topic trở về sau sẽ tự động +1) 
        public async Task<TopicAdminViewDto> AddTopicAsync(TopicCreateDto topicCreateDto)
        {
            ValidateTopicDto(topicCreateDto);
            // Kiểm tra xem tên Topic đã tồn tại chưa
            var existingTopicByName = await _unitOfWork.GetRepository<Topic>().Entities
                .FirstOrDefaultAsync(t => t.TopicName == topicCreateDto.TopicName && t.ChapterId == topicCreateDto.ChapterId && t.Status);

            if (existingTopicByName != null)
            {
                throw new BaseException.BadRequestException("Invalid!", $"A topic with the name '{topicCreateDto.TopicName}' already exists in this chapter.");
            }
            // Kiểm tra xem số thứ tự đã tồn tại trong chapter chưa
            var existingTopics = await _unitOfWork.GetRepository<Topic>().Entities
                .Where(t => t.ChapterId == topicCreateDto.ChapterId && t.Status)
                .ToListAsync();

            // Kiểm tra số thứ tự có bị trùng không
            var existingTopicWithSameNumber = existingTopics
                .FirstOrDefault(t => t.Number == topicCreateDto.Number);

            if (existingTopicWithSameNumber != null)
            {
                // Cộng số thứ tự của các topic liền kề và lớn hơn
                foreach (var existingTopic in existingTopics.Where(t => t.Number >= topicCreateDto.Number))
                {
                    existingTopic.Number += 1;
                    _unitOfWork.GetRepository<Topic>().Update(existingTopic);
                }

                // Lưu thay đổi trước khi thêm topic mới
                await _unitOfWork.SaveAsync();
            }
            
            User currentUser = await _userService.GetCurrentUserAsync();
            
            // Tạo topic mới
            Topic newTopic = new Topic
            {
                TopicName = topicCreateDto.TopicName,
                TopicContext = topicCreateDto.TopicContext,
                Number = topicCreateDto.Number, // Sử dụng số thứ tự đã được kiểm tra
                ChapterId = topicCreateDto.ChapterId,
                QuizId = topicCreateDto.QuizId,
                Status = true,
                //CreatedBy = currentUser?.Id ?? string.Empty,
                //LastUpdatedBy = currentUser?.LastUpdatedBy ?? string.Empty,
                //CreatedTime = topicCreateDto?.CreatedTime ?? CoreHelper.SystemTimeNow,
                //LastUpdatedTime = topicCreateDto?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
            };

            await _unitOfWork.GetRepository<Topic>().InsertAsync(newTopic);
            await _unitOfWork.SaveAsync();

            if (string.IsNullOrWhiteSpace(newTopic.ChapterId))
            {
                throw new BaseException.BadRequestException("Chapter ID cannot be empty.", nameof(newTopic.ChapterId));
            }


            if (string.IsNullOrWhiteSpace(newTopic.QuizId))
            {
                throw new BaseException.BadRequestException("Quiz ID cannot be empty.", nameof(newTopic.ChapterId));
            }
            string quizName = await _quizService.GetQuizNameAsync(newTopic.QuizId) ?? string.Empty;
           

            return new TopicAdminViewDto
            {
                Id = newTopic!.Id,
                Number = newTopic.Number,
                TopicName = newTopic.TopicName,
                TopicContext = newTopic.TopicContext,
                QuizId = newTopic.QuizId,
                QuizName = quizName,
                ChapterId = newTopic.ChapterId,
                CreatedBy = currentUser?.Id ?? string.Empty,
                CreatorName = currentUser?.FullName ?? string.Empty,
                CreatorPhone = currentUser?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = currentUser?.LastUpdatedBy ?? string.Empty,
                LastUpdatedPersonName = currentUser?.FullName ?? string.Empty,
                LastUpdatedPersonPhone = currentUser?.PhoneNumber ?? string.Empty,
                CreatedTime = newTopic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                LastUpdatedTime = newTopic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
            };
        }


        //// Tạo 1 Topic ( Tăng Number tự động )
        //public async Task<TopicAdminViewDto> AddTopicAsync(TopicCreateDto topicCreateDto)
        //{
        //    ValidateTopicDto(topicCreateDto);
        //    if (string.IsNullOrWhiteSpace(topicCreateDto.ChapterId))
        //    {
        //        throw new ArgumentException("Chapter ID cannot be empty.", nameof(topicCreateDto.ChapterId));
        //    }
        //    var nextNumber = await GetNextNumberAsync(topicCreateDto.ChapterId);

        //    Topic topic = new Topic
        //    {
        //        TopicName = topicCreateDto.TopicName,
        //        TopicContext = topicCreateDto.TopicContext,
        //        Number = nextNumber,
        //        ChapterId = topicCreateDto.ChapterId,
        //        QuizId = topicCreateDto.QuizId,
        //        Status = true,
        //        //CreatedBy = topicCreateDto.CreatedByUser,
        //        //LastUpdatedBy = topicCreateDto.LastUpdatedByUser,
        //    };

        //    //_userService.AuditFields(topic, true);

        //    await _unitOfWork.GetRepository<Topic>().InsertAsync(topic);
        //    await _unitOfWork.SaveAsync();

        //    if (string.IsNullOrWhiteSpace(topic.ChapterId))
        //    {
        //        throw new ArgumentException("Chapter ID cannot be empty.", nameof(topic.ChapterId));
        //    }

        //    if (string.IsNullOrWhiteSpace(topic.QuizId))
        //    {
        //        throw new ArgumentException("Quiz ID cannot be empty.", nameof(topic.QuizId));
        //    }

        //    string chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
        //    string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
        //    User currentUser = await _userService.GetCurrentUserAsync();

        //    return new TopicAdminViewDto
        //    {
        //        Id = topic!.Id,
        //        Number = topic.Number,
        //        TopicName = topic.TopicName,
        //        TopicContext = topic.TopicContext,
        //        QuizId = topic.QuizId,
        //        QuizName = quizName,
        //        ChapterId = topic.ChapterId,
        //        CreatedBy = topic?.CreatedBy ?? string.Empty,
        //        CreatorName = currentUser?.FullName ?? string.Empty,
        //        CreatorPhone = currentUser?.PhoneNumber ?? string.Empty,
        //        LastUpdatedBy = currentUser?.LastUpdatedBy ?? string.Empty,
        //        LastUpdatedPersonName = currentUser?.FullName ?? string.Empty,
        //        LastUpdatedPersonPhone = currentUser?.PhoneNumber ?? string.Empty,
        //        CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
        //        LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
        //    };
        //}

        // Cập nhật 1 Topic
        public async Task<TopicAdminViewDto> UpdateTopicAsync(string id, TopicUpdateDto topicUpdateDto)
        {
            ValidateTopicId(id);
            Topic? existingTopic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);

            if (existingTopic == null) throw new BaseException.NotFoundException("Not Found!", $"No topics found for ID '{id}'.");

            // Kiểm tra xem tên Topic đã tồn tại chưa (trừ chính nó)
            var existingTopicByName = await _unitOfWork.GetRepository<Topic>().Entities
                .FirstOrDefaultAsync(t => t.TopicName == topicUpdateDto.TopicName && t.ChapterId == topicUpdateDto.ChapterId && t.Status && t.Id != id);

            if (existingTopicByName != null)
            {
                throw new BaseException.BadRequestException("Invalid!", $"A topic with the name '{topicUpdateDto.TopicName}' already exists in this chapter.");
            }

            existingTopic.TopicName = topicUpdateDto.TopicName;
            existingTopic.TopicContext = topicUpdateDto.TopicContext;
            //existingTopic.Number = topicUpdateDto.Number;
            existingTopic.ChapterId = topicUpdateDto.ChapterId;
            existingTopic.QuizId = topicUpdateDto.QuizId;
            existingTopic.Status = true;
            //existingTopic.CreatedBy = topicUpdateDto.CreatedByUser;
            //existingTopic.LastUpdatedBy = topicUpdateDto.LastUpdatedByUser;

            _userService.AuditFields(existingTopic, false);

            await _unitOfWork.SaveAsync();
            if (string.IsNullOrWhiteSpace(existingTopic.ChapterId))
            {
                throw new BaseException.BadRequestException("Chapter ID cannot be empty.", nameof(existingTopic.ChapterId));
            }

            if (string.IsNullOrWhiteSpace(existingTopic.QuizId))
            {
                throw new BaseException.BadRequestException("Quiz ID cannot be empty.", nameof(existingTopic.QuizId));
            }

            string quizName = await _quizService.GetQuizNameAsync(existingTopic.QuizId) ?? string.Empty;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(existingTopic?.CreatedBy ?? string.Empty);
            User currentUser = await _userService.GetCurrentUserAsync();

            return new TopicAdminViewDto
            {
                Id = existingTopic!.Id,
                Number = existingTopic.Number,
                TopicName = existingTopic.TopicName,
                TopicContext = existingTopic.TopicContext,
                QuizId = existingTopic.QuizId,
                QuizName = quizName,
                ChapterId = existingTopic.ChapterId,
                CreatedBy = existingTopic?.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = currentUser.Id,
                LastUpdatedPersonName = currentUser.FullName,
                LastUpdatedPersonPhone = currentUser.PhoneNumber ?? string.Empty,
                CreatedTime = existingTopic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                LastUpdatedTime = existingTopic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
            };
        }

        public async Task<bool> DeleteTopicAsync(string topicId)
        {
            //Delete the topic
            Topic? topic;

            if (_unitOfWork.IsValid<Topic>(topicId))
                topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(topicId);
            else throw new BaseException.NotFoundException("not_found", "Topic ID not found");

            _userService.AuditFields(topic!, false, true);

            await _unitOfWork.SaveAsync();

            // Delete the corresponding quiz
            await _quizService.DeleteQuizAsync(topic!.QuizId!);

            return true;
        }

        // Rollback Topic đã xóa
        public async Task<TopicDeleteDto> RollBackTopicDeletedAsync(string Id)
        {
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(Id) ?? throw new BaseException.BadRequestException("Not Found", "Topic ID not found");
            if (topic.DeletedBy == null)
            {
                throw new BaseException.BadRequestException("Succecffully", "This chapter was rollback");
            }
            if (topic.Status == false)
            {
                topic.Status = true;
            }
            topic.DeletedBy = null;
            topic.DeletedTime = null;

            _userService.AuditFields(topic);

            await _unitOfWork.SaveAsync();
            if (string.IsNullOrWhiteSpace(topic.ChapterId))
            {
                throw new BaseException.BadRequestException("Chapter ID cannot be empty.", nameof(topic.ChapterId));
            }

            if (string.IsNullOrWhiteSpace(topic.QuizId))
            {
                throw new BaseException.BadRequestException("Quiz ID cannot be empty.", nameof(topic.QuizId));
            }

            string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;
            User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
            User currentUser = await _userService.GetCurrentUserAsync();

            return new TopicDeleteDto
            {
                Id = topic!.Id,
                Number = topic.Number,
                TopicName = topic.TopicName,
                TopicContext = topic.TopicContext,
                Status = topic.Status,
                QuizId = topic.QuizId,
                QuizName = quizName,
                ChapterId = topic.ChapterId,
                CreatedBy = topic?.CreatedBy ?? string.Empty,
                CreatorName = creator?.FullName ?? string.Empty,
                CreatorPhone = creator?.PhoneNumber ?? string.Empty,
                LastUpdatedBy = currentUser.Id,
                LastUpdatedPersonName = currentUser.FullName,
                LastUpdatedPersonPhone = currentUser.PhoneNumber ?? string.Empty,
                CreatedTime = topic?.CreatedTime ?? CoreHelper.SystemTimeNow,
                LastUpdatedTime = topic?.LastUpdatedTime ?? CoreHelper.SystemTimeNow
            };
        }

        //Chỉ cho phép hoán đổi vị trí number trong cùng 1 chapter
        public async Task SwapTopicNumbersAsync(string topicId1, string topicId2)
        {
            // Lấy thông tin của hai topic
            var topic1 = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(topicId1);
            var topic2 = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(topicId2);

            // Kiểm tra xem topic có tồn tại không
            if (topic1 == null || topic2 == null)
            {
                throw new BaseException.NotFoundException("Not Found!", "One or both topics not found.");
            }

            // Kiểm tra xem có cùng chapterId không
            if (topic1.ChapterId != topic2.ChapterId)
            {
                throw new BaseException.BadRequestException("Invalid!", "Topics must belong to the same chapter to swap their numbers.");
            }

            // Đổi số thứ tự
            var tempNumber = topic1.Number;
            topic1.Number = topic2.Number;
            topic2.Number = tempNumber;

            // Cập nhật thông tin vào cơ sở dữ liệu
            _unitOfWork.GetRepository<Topic>().Update(topic1);
            _unitOfWork.GetRepository<Topic>().Update(topic2);
            await _unitOfWork.SaveAsync();
        }

        //private async Task<int?> GetNextNumberAsync(string chapterId)
        //{
        //    var topics = await _unitOfWork.GetRepository<Topic>().GetAllAsync();
        //    var maxNumber = topics
        //        .Where(t => t.ChapterId == chapterId)
        //        .Select(t => t.Number)
        //        .DefaultIfEmpty(0)
        //        .Max();

        //    return maxNumber + 1;
        //}

        // Các phương thức kiểm tra ràng buộc
        private void ValidateTopicId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BaseException.BadRequestException("Invalid!", "Topic ID cannot be empty.");
            }
        }
        
        private void ValidateTopicDto(TopicCreateDto topicCreateDto)
        {
            if (string.IsNullOrWhiteSpace(topicCreateDto.TopicName))
            {
                throw new BaseException.BadRequestException("Invalid!", "Topic Name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(topicCreateDto.ChapterId))
            {
                throw new BaseException.BadRequestException("Invalid!", "Chapter ID cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(topicCreateDto.QuizId))
            {
                throw new BaseException.BadRequestException("Invalid!", "Quiz ID cannot be empty.");
            }

            // Kiểm tra người tạo, nếu cần
            //if (string.IsNullOrWhiteSpace(topicCreateDto.CreatedByUser))
            //{
            //    throw new ArgumentException("Created By User cannot be empty.");
            //}
        }
        // Phương thức xác thực ChapterId
        private void ValidateChapterId(string chapterId)
        {
            if (string.IsNullOrWhiteSpace(chapterId))
            {
                throw new BaseException.BadRequestException("Invalid!", "Chapter ID cannot be empty.");
            }
        }
    }
}