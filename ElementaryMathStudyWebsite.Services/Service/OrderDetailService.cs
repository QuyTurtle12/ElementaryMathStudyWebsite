using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderDetailService : IAppOrderDetailServices
    {
        private readonly IGenericRepository<OrderDetail> _detailReposiotry;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserServices _userService;
        private readonly IAppSubjectServices _subjectService;

        // Constructor
        public OrderDetailService(IGenericRepository<OrderDetail> detailReposiotry, IUnitOfWork unitOfWork, IAppUserServices userService, IAppSubjectServices subjectService)
        {
            _detailReposiotry = detailReposiotry;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _subjectService = subjectService;
        }

        // Add Order Detail to database
        public async Task<bool> AddOrderDetailAsync(OrderDetail detail)
        {
            try
            {
                await _detailReposiotry.InsertAsync(detail);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        // Get Order Detail list by order Id 
        public async Task<BasePaginatedList<OrderDetailViewDto?>> GetOrderDetailDtoListByOrderIdAsync(int pageNumber, int pageSize, string orderId)
        {
            // Get all order details from database
            // If null then return empty collection
            IQueryable<OrderDetail> query = _detailReposiotry.Entities;
            IList<OrderDetailViewDto> detailDtos = new List<OrderDetailViewDto>();

            // If pageNumber or pageSize are 0 or negative, show all order details without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allDetails = query.ToList();
                // Map orders to OrderViewDto
                foreach (var detail in allDetails)
                {

                    if (detail.OrderId == orderId)
                    {
                        string? studentName = await _userService.GetUserNameAsync(detail.StudentId);
                        string? subjectName = await _subjectService.GetSubjectNameAsync(detail.SubjectId);
                        OrderDetailViewDto dto = new OrderDetailViewDto(subjectName, studentName);
                        detailDtos.Add(dto);
                    }
                }
                return new BasePaginatedList<OrderDetailViewDto>((IReadOnlyCollection<OrderDetailViewDto>)detailDtos, detailDtos.Count, 1, detailDtos.Count);
            }

            // Show all order details with pagination
            // Map order detail to OrderDetailViewDto
            BasePaginatedList<OrderDetail>? paginatedOrders = await _detailReposiotry.GetPagging(query, pageNumber, pageSize);
            foreach (var detail in paginatedOrders.Items)
            {


                if (detail.OrderId == orderId)
                {
                    string? studentName = await _userService.GetUserNameAsync(detail.StudentId);
                    string? subjectName = await _subjectService.GetSubjectNameAsync(detail.SubjectId);
                    detailDtos.Add(new OrderDetailViewDto(subjectName, studentName));
                }
            }

            return new BasePaginatedList<OrderDetailViewDto>((IReadOnlyCollection<OrderDetailViewDto>)detailDtos, detailDtos.Count, pageNumber, pageSize);
        }

        // Validate if the subject has been assigned before 
        public async Task<bool> IsValidStudentSubjectBeforeCreateOrder(OrderCreateDto orderCreateDto)
        {
            // Validation process
            foreach (var newSubject in orderCreateDto.SubjectStudents)
            {
                // Get student assigned subject
                IQueryable<OrderDetail> query = _detailReposiotry.Entities.Where(d => d.StudentId.Equals(newSubject.StudentId));
                var studentCurrentLearningSubject = await query.ToListAsync();

                foreach (var studentSubject in studentCurrentLearningSubject)
                {
                    if (studentSubject.SubjectId.Equals(newSubject.SubjectId))
                    {
                        return false; // Subject has been assigned
                    }
                }
            }

            return true; // Subject has not been assigned yet
        }
    }
}
