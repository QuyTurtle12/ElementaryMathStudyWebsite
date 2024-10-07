﻿using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Utils;
using ElementaryMathStudyWebsite.Core.Store;
using System.Text.RegularExpressions;
using ElementaryMathStudyWebsite.Core.Entity;
using AutoMapper;
using EllipticCurve.Utils;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class TopicService : IAppTopicServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IMapper _mapper;

        public TopicService(IUnitOfWork unitOfWork, IAppUserServices userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;

        }

        // Lấy danh sách Topic ( Admin )
        public async Task<BasePaginatedList<TopicAdminViewDto>> GetAllExistTopicsAsync(int pageNumber, int pageSize)
        {
            // Fetch all active topics asynchronously without invalid Include statements
            var topics = await _unitOfWork.GetRepository<Topic>()
                .Entities
                .Where(t => t.Status == true)
                .Include(t => t.Quiz)
                .Include(t => t.Chapter)
                .ToListAsync();

            // If no topics found, throw an exception
            if (!topics.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Fetch related users manually
            var createdUserIds = topics.Where(t => t.CreatedBy != null).Select(t => t.CreatedBy).Distinct();
            var updatedUserIds = topics.Where(t => t.LastUpdatedBy != null).Select(t => t.LastUpdatedBy).Distinct();

            var createdUsers = await _unitOfWork.GetRepository<User>().Entities
                .Where(u => createdUserIds.Contains(u.Id))
                .ToListAsync();

            var updatedUsers = await _unitOfWork.GetRepository<User>().Entities
                .Where(u => updatedUserIds.Contains(u.Id))
                .ToListAsync();

            // Create a list to hold TopicAdminViewDto objects
            var topicAdminViewDtos = topics.Select(topic =>
            {
                var createdUser = createdUsers.FirstOrDefault(u => u.Id == topic.CreatedBy);
                var updatedUser = updatedUsers.FirstOrDefault(u => u.Id == topic.LastUpdatedBy);

                return _mapper.Map<TopicAdminViewDto>(topic, opts =>
                {
                    opts.Items["CreatedUser"] = createdUser;
                    opts.Items["UpdatedUser"] = updatedUser;
                });
            }).Where(dto => dto != null).ToList();

            // Check if any valid TopicAdminViewDto was created
            if (!topicAdminViewDtos.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Validate and adjust pagination parameters
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<TopicAdminViewDto>(topicAdminViewDtos, topicAdminViewDtos.Count, 1, topicAdminViewDtos.Count);
            }

            // Adjust page number for valid pagination
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, topicAdminViewDtos.Count, pageSize);

            // Return paginated results
            var paginatedTopics = topicAdminViewDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new BasePaginatedList<TopicAdminViewDto>(paginatedTopics, topicAdminViewDtos.Count, pageNumber, pageSize);
        }


        public async Task<BasePaginatedList<TopicAdminViewDto>> GetAllDeleteTopicsAsync(int pageNumber, int pageSize)
        {
            // Fetch all deleted topics asynchronously
            var topics = await _unitOfWork.GetRepository<Topic>()
                .Entities
                .Where(c => c.Status == false && c.DeletedBy != null && c.DeletedTime != null)
                .Include(t => t.Quiz)
                .Include(t => t.Chapter)
                .ToListAsync();

            // If no topics found, throw an exception
            if (!topics.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Fetch related users manually
            var createdUserIds = topics.Where(t => t.CreatedBy != null).Select(t => t.CreatedBy).Distinct();
            var updatedUserIds = topics.Where(t => t.LastUpdatedBy != null).Select(t => t.LastUpdatedBy).Distinct();

            var createdUsers = await _unitOfWork.GetRepository<User>().Entities
                .Where(u => createdUserIds.Contains(u.Id))
                .ToListAsync();

            var updatedUsers = await _unitOfWork.GetRepository<User>().Entities
                .Where(u => updatedUserIds.Contains(u.Id))
                .ToListAsync();

            // Create a list to hold TopicAdminViewDto objects
            var topicAdminViewDtos = topics.Select(topic =>
            {
                var createdUser = createdUsers.FirstOrDefault(u => u.Id == topic.CreatedBy);
                var updatedUser = updatedUsers.FirstOrDefault(u => u.Id == topic.LastUpdatedBy);

                return _mapper.Map<TopicAdminViewDto>(topic, opts =>
                {
                    opts.Items["CreatedUser"] = createdUser;
                    opts.Items["UpdatedUser"] = updatedUser;
                });
            }).Where(dto => dto != null).ToList();

            // Check if any valid TopicAdminViewDto was created
            if (!topicAdminViewDtos.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Validate and adjust pagination parameters
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<TopicAdminViewDto>(topicAdminViewDtos, topicAdminViewDtos.Count, 1, topicAdminViewDtos.Count);
            }

            // Adjust page number for valid pagination
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, topicAdminViewDtos.Count, pageSize);

            // Return paginated results
            var paginatedTopics = topicAdminViewDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new BasePaginatedList<TopicAdminViewDto>(paginatedTopics, topicAdminViewDtos.Count, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<TopicViewDto>> GetAllTopicsAsync(int pageNumber, int pageSize)
        {
            // Fetch all active topics asynchronously
            var topics = await _unitOfWork.GetRepository<Topic>()
                .Entities
                .Where(t => t.Status == true)
                .Include(t => t.Quiz)
                .Include(t => t.Chapter)
                .ToListAsync();

            // If no topics found, throw an exception
            if (!topics.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Map Topic to TopicViewDto
            var topicViewDtos = topics.Select(topic => _mapper.Map<TopicViewDto>(topic)).ToList();

            // Filter out null entries (if any)
            var validTopicViewDtos = topicViewDtos.Where(dto => dto != null).ToList();

            // Check if any valid TopicViewDto was created
            if (!validTopicViewDtos.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Validate and adjust pagination parameters
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<TopicViewDto>(validTopicViewDtos, validTopicViewDtos.Count, 1, validTopicViewDtos.Count);
            }

            // Adjust page number for valid pagination
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, validTopicViewDtos.Count, pageSize);

            // Paginate the result
            var paginatedTopics = validTopicViewDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Return paginated results
            return new BasePaginatedList<TopicViewDto>(paginatedTopics, validTopicViewDtos.Count, pageNumber, pageSize);
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
                throw new BaseException.NotFoundException("not_found", $"Topic with ID '{id}' not found.");
            }

            //if (string.IsNullOrWhiteSpace(topic.ChapterId))
            //{
            //    throw new BaseException.BadRequestException("Chapter ID cannot be empty.", nameof(topic.ChapterId));
            //}

            //if (string.IsNullOrWhiteSpace(topic.QuizId))
            //{
            //    throw new BaseException.BadRequestException("Quiz ID cannot be empty.", nameof(topic.QuizId));
            //}

            //// Lấy tên chương và tên quiz
            //string chapterName = await _chapterService.GetChapterNameAsync(topic.ChapterId) ?? string.Empty;
            //string quizName = await _quizService.GetQuizNameAsync(topic.QuizId) ?? string.Empty;

            //// Lấy thông tin người tạo và người cập nhật
            //User? creator = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
            //User? lastUpdatedPerson = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);

            //// Ánh xạ topic sang TopicAdminViewDto
            //var topicAdminViewDto = _mapper.Map<TopicAdminViewDto>(topic);

            //// Thiết lập các thuộc tính bổ sung
            //topicAdminViewDto.QuizName = quizName;
            //topicAdminViewDto.ChapterName = chapterName;
            //topicAdminViewDto.CreatorName = creator?.FullName ?? string.Empty;
            //topicAdminViewDto.CreatorPhone = creator?.PhoneNumber ?? string.Empty;
            //topicAdminViewDto.LastUpdatedPersonName = lastUpdatedPerson?.FullName ?? string.Empty;
            //topicAdminViewDto.LastUpdatedPersonPhone = lastUpdatedPerson?.PhoneNumber ?? string.Empty;

            //return topicAdminViewDto;

            Quiz? quiz = topic.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(topic.QuizId) : null;
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);

            TopicAdminViewDto dto = _mapper.Map<TopicAdminViewDto>(topic, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Chapter"] = chapter;
                opts.Items["Quiz"] = quiz;
            });

            return dto;
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
                throw new BaseException.NotFoundException("not_found", $"Topic with ID '{id}' not found.");
            }
            Quiz? quiz = topic.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(topic.QuizId) : null;
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);

            TopicViewDto dto = _mapper.Map<TopicViewDto>(topic, opts =>
            {
                opts.Items["Chapter"] = chapter;
                opts.Items["Quiz"] = quiz;
            });

            return dto;
        }

        // Tìm kiếm Topic bằng Topic's Name
        public async Task<BasePaginatedList<TopicViewDto>> SearchTopicByNameAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Topic>()
                .Entities
                .Where(t => t.Status == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => EF.Functions.Like(t.TopicName, $"%{searchTerm}%"));
            }
            var allTopics = await query
                .Include(t => t.Quiz)
                .Include(t => t.Chapter)
                .ToListAsync();
            // If no topics found, throw an exception
            if (!allTopics.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Map Topic to TopicViewDto
            var topicViewDtos = allTopics.Select(topic => _mapper.Map<TopicViewDto>(topic)).ToList();

            // Filter out null entries (if any)
            var validTopicViewDtos = topicViewDtos.Where(dto => dto != null).ToList();

            // Check if any valid TopicViewDto was created
            if (!validTopicViewDtos.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Validate and adjust pagination parameters
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<TopicViewDto>(validTopicViewDtos, validTopicViewDtos.Count, 1, validTopicViewDtos.Count);
            }

            // Adjust page number for valid pagination
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, validTopicViewDtos.Count, pageSize);

            // Paginate the result
            var paginatedTopics = validTopicViewDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Return paginated results
            return new BasePaginatedList<TopicViewDto>(paginatedTopics, validTopicViewDtos.Count, pageNumber, pageSize);
        }

        // Lấy danh sách Topic theo ChapterId
        public async Task<List<TopicViewDto>> GetTopicsByChapterIdAsync(string chapterId)
        {
            ValidateChapterId(chapterId);

            // Fetch all active topics asynchronously
            var topics = await _unitOfWork.GetRepository<Topic>()
                .Entities
                .Where(t => t.ChapterId == chapterId && t.Status)
                .Include(t => t.Quiz)
                .Include(t => t.Chapter)
                .ToListAsync();

            // If no topics found, throw an exception
            if (!topics.Any())
            {
                throw new BaseException.NotFoundException("not_found", "cannot find any topics");
            }

            // Create a list to hold TopicViewDto objects
            var tp = topics.Select(topic => _mapper.Map<TopicViewDto>(topic)).ToList();
            return tp;
        }

        // Tạo 1 Topic ( Nếu New Topic trùng Number với Old Topic thì Number từ Old Topic trở về sau sẽ tự động +1) 
        public async Task<TopicAdminViewDto> AddTopicAsync(TopicCreateDto topicCreateDto)
        {
            ValidateTopicDto(topicCreateDto);

            // Kiểm tra xem tên Topic đã tồn tại chưa
            var existingTopicByName = await _unitOfWork.GetRepository<Topic>().Entities
                .FirstOrDefaultAsync(t => t.TopicName == topicCreateDto.TopicName
                                           && t.ChapterId == topicCreateDto.ChapterId
                                           && t.Status);

            if (existingTopicByName != null)
            {
                throw new BaseException.BadRequestException("Invalid!", $"A topic with the name '{topicCreateDto.TopicName}' already exists in this chapter.");
            }

            // Kiểm tra xem số thứ tự đã tồn tại trong chapter chưa
            var existingTopics = await _unitOfWork.GetRepository<Topic>().Entities
                .Where(t => t.ChapterId == topicCreateDto.ChapterId && t.Status)
                .ToListAsync();

            // Kiểm tra số thứ tự có bị trùng không
            Topic? existingTopicWithSameNumber = existingTopics
                .FirstOrDefault(t => t.Number == topicCreateDto.Number);

            if (existingTopicWithSameNumber != null)
            {
                // Cộng số thứ tự của các topic liền kề và lớn hơn
                foreach (Topic existingTopic in existingTopics.Where(t => t.Number >= topicCreateDto.Number))
                {
                    existingTopic.Number += 1;
                    _unitOfWork.GetRepository<Topic>().Update(existingTopic);
                }

                // Lưu thay đổi trước khi thêm topic mới
                await _unitOfWork.SaveAsync();
            }

            // Kiểm tra QuizId không trùng
            if (topicCreateDto.QuizId != null)
            {
                var existingTopicWithSameQuizId = await _unitOfWork.GetRepository<Topic>().Entities
                    .FirstOrDefaultAsync(t => t.QuizId == topicCreateDto.QuizId
                                               && t.ChapterId == topicCreateDto.ChapterId
                                               && t.Status);

                if (existingTopicWithSameQuizId != null)
                {
                    throw new BaseException.BadRequestException("Invalid!", $"A topic with the same QuizId already exists in this chapter.");
                }
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
            };
            newTopic.CreatedBy = currentUser.Id; //
            newTopic.CreatedTime = DateTime.UtcNow; //
            newTopic.LastUpdatedBy = currentUser.Id; //
            newTopic.LastUpdatedTime = DateTime.UtcNow; //
            _userService.AuditFields(newTopic, false);

            await _unitOfWork.GetRepository<Topic>().InsertAsync(newTopic);
            await _unitOfWork.SaveAsync();

            Quiz? quiz = newTopic.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(newTopic.QuizId) : null;
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(newTopic.ChapterId);
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(newTopic?.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(newTopic?.LastUpdatedBy ?? string.Empty);

            TopicAdminViewDto topicAdminViewDto = _mapper.Map<TopicAdminViewDto>(newTopic, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Chapter"] = chapter;
                opts.Items["Quiz"] = quiz;
            });

            return topicAdminViewDto;
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

            User currentUser = await _userService.GetCurrentUserAsync();

            if (existingTopic == null)
                throw new BaseException.NotFoundException("not_found", $"No topics found for ID '{id}'.");

            // Kiểm tra xem tên Topic đã tồn tại chưa (trừ chính nó)
            var existingTopicByName = await _unitOfWork.GetRepository<Topic>().Entities
                .FirstOrDefaultAsync(t => t.TopicName == topicUpdateDto.TopicName);

            if (existingTopicByName != null)
            {
                throw new BaseException.BadRequestException("Invalid!", $"A topic with the name '{topicUpdateDto.TopicName}' already exists in this chapter.");
            }

            existingTopic.TopicName = topicUpdateDto.TopicName;
            existingTopic.TopicContext = topicUpdateDto.TopicContext;
            existingTopic.Status = true;
            existingTopic.LastUpdatedBy = currentUser.Id; //
            existingTopic.LastUpdatedTime = DateTime.UtcNow; //

            _userService.AuditFields(existingTopic, false);

            await _unitOfWork.SaveAsync();

            Quiz? quiz = existingTopic.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(existingTopic.QuizId) : null;
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(existingTopic.ChapterId);
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(existingTopic?.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(existingTopic?.LastUpdatedBy ?? string.Empty);

            TopicAdminViewDto topicAdminViewDto = _mapper.Map<TopicAdminViewDto>(existingTopic, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Chapter"] = chapter;
                opts.Items["Quiz"] = quiz;
            });
            return topicAdminViewDto;
        }

        public async Task<TopicAdminViewDto> UpdateQuizIdTopicAsync(string id, TopicUpdateQuizIdDto topicUpdateDto)
        {
            ValidateTopicId(id);
            Topic? existingTopic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);

            User currentUser = await _userService.GetCurrentUserAsync();

            if (existingTopic == null)
                throw new BaseException.NotFoundException("not_found", "QuizId is null.");

            // Check if the QuizId is provided
            if (topicUpdateDto.QuizId != null)
            {
                // Check if the QuizId exists
                var quizExists = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(topicUpdateDto.QuizId) != null;
                if (!quizExists)
                {
                    throw new BaseException.BadRequestException("not_found", $"No quiz found for ID '{topicUpdateDto.QuizId}'.");
                }

                // Check for conflicts with other topics (excluding the current topic)
                var existingTopicByQuizId = await _unitOfWork.GetRepository<Topic>().Entities
                    .FirstOrDefaultAsync(t => t.QuizId == topicUpdateDto.QuizId && t.Id != id);
                if (existingTopicByQuizId != null)
                {
                    throw new BaseException.BadRequestException("not_found", $"The Quiz ID '{topicUpdateDto.QuizId}' is already assigned to another topic.");
                }

                existingTopic.QuizId = topicUpdateDto.QuizId;
            }
            else
            {
                // Allow QuizId to be null
                existingTopic.QuizId = null;
            }

            existingTopic.LastUpdatedBy = currentUser.Id; //
            existingTopic.LastUpdatedTime = DateTime.UtcNow; //

            existingTopic.Status = true;

            _userService.AuditFields(existingTopic, false);

            await _unitOfWork.SaveAsync();

            Quiz? quiz = existingTopic.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(existingTopic.QuizId) : null;
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(existingTopic.ChapterId);
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(existingTopic.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(existingTopic.LastUpdatedBy ?? string.Empty);

            TopicAdminViewDto topicAdminViewDto = _mapper.Map<TopicAdminViewDto>(existingTopic, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Chapter"] = chapter;
                opts.Items["Quiz"] = quiz;
            });

            return topicAdminViewDto;
        }

        // Xóa 1 Topic 
        public async Task<TopicDeleteDto> DeleteTopicAsync(string id)
        {
            // Lấy token
            User currentUser = await _userService.GetCurrentUserAsync();

            // Tìm topic theo ID
            Topic topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id) ?? throw new BaseException.NotFoundException("not_found", $"Topic with ID '{id}' not found.");

            // Kiểm tra xem topic đã bị xóa chưa
            if (topic.DeletedBy != null)
            {
                throw new BaseException.BadRequestException("Invalid!", "This topic was already deleted.");
            }

            // Đánh dấu topic là đã xóa
            if (topic.Status == true)
            {
                topic.Status = false;// Đặt trạng thái là không hoạt động
            }

            topic.DeletedBy = currentUser.Id; // Lưu ID người đã xóa
            topic.DeletedTime = DateTime.UtcNow; // Ghi lại thời gian xóa

            // Ghi lại thông tin audit nếu cần thiết
            _userService.AuditFields(topic, true);

            // Cập nhật topic trong cơ sở dữ liệu
            _unitOfWork.GetRepository<Topic>().Update(topic);
            await _unitOfWork.GetRepository<Topic>().SaveAsync();

            Quiz? quiz = topic.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(topic.QuizId) : null;
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);
            User? deletedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.DeletedBy ?? string.Empty);

            TopicDeleteDto topicDeleteDto = _mapper.Map<TopicDeleteDto>(topic, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["DeleteUser"] = deletedUser;
                opts.Items["Chapter"] = chapter;
                opts.Items["Quiz"] = quiz;
            });
            return topicDeleteDto;
        }

        // Rollback Topic đã xóa
        public async Task<TopicDeleteDto> RollBackTopicDeletedAsync(string id)
        {
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id)
                        ?? throw new BaseException.BadRequestException("not_found", "Topic ID not found");

            if (topic.DeletedBy == null)
            {
                throw new BaseException.BadRequestException("Successfully", "This topic was already rolled back");
            }

            if (!topic.Status)
            {
                topic.Status = true;
            }

            topic.DeletedBy = null;
            topic.DeletedTime = null;

            _userService.AuditFields(topic);

            await _unitOfWork.SaveAsync();

            Quiz? quiz = topic.QuizId != null ? await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(topic.QuizId) : null;
            Chapter? chapter = await _unitOfWork.GetRepository<Chapter>().GetByIdAsync(topic.ChapterId);
            User? createdUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.CreatedBy ?? string.Empty);
            User? updatedUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(topic?.LastUpdatedBy ?? string.Empty);
            User currentUser = await _userService.GetCurrentUserAsync();

            TopicDeleteDto topicDeleteDto = _mapper.Map<TopicDeleteDto>(topic, opts =>
            {
                opts.Items["CreatedUser"] = createdUser;
                opts.Items["UpdatedUser"] = updatedUser;
                opts.Items["Chapter"] = chapter;
                opts.Items["Quiz"] = quiz;
            });
            return topicDeleteDto;
        }

        //Chỉ cho phép hoán đổi vị trí number trong cùng 1 chapter
        public async Task SwapTopicNumbersAsync(string topicId1, string topicId2)
        {
            // Lấy thông tin của hai topic
            Topic? topic1 = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(topicId1);
            Topic? topic2 = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(topicId2);

            // Kiểm tra xem topic có tồn tại không
            if (topic1 == null || topic2 == null)
            {
                throw new BaseException.NotFoundException("not_found", "One or both topics not found.");
            }

            // Kiểm tra xem có cùng chapterId không
            if (topic1.ChapterId != topic2.ChapterId)
            {
                throw new BaseException.BadRequestException("Invalid!", "Topics must belong to the same chapter to swap their numbers.");
            }

            // Đổi số thứ tự
            int tempNumber = topic1.Number;
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