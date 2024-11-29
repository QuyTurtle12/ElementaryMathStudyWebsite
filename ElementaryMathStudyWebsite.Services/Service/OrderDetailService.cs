using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Store;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderDetailService : IAppOrderDetailServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // Constructor
        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Add Order Detail to database
        public async Task<bool> AddOrderDetailAsync(OrderDetail detail)
        {
            try
            {
                await _unitOfWork.GetRepository<OrderDetail>().InsertAsync(detail);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        // Get Order Detail list by order Id 
        public async Task<BasePaginatedList<OrderDetailViewDto>> GetOrderDetailDtoListByOrderIdAsync(int pageNumber, int pageSize, string orderId)
        {
            // Validate the orderId input
            if (string.IsNullOrWhiteSpace(orderId))
            {
                throw new BaseException.BadRequestException("invalid_order_id", "Order ID cannot be null or empty.");
            }

            // Get all order details from database
            // If null then return empty collection
            IQueryable<OrderDetail> query = _unitOfWork.GetRepository<OrderDetail>()
                .GetEntitiesWithCondition(
                    od => od.OrderId.Equals(orderId),   // Condition
                    od => od.Subject!,                  // Include the Subject
                    od => od.User!                      // Include the User
                    );

            IEnumerable<OrderDetail> allDetails = await query.ToListAsync();

            // Throw error if nothing was found
            if (!allDetails.Any()) throw new BaseException.NotFoundException("not_found", $"the system didn't find any detail of order {orderId}");

            // Map orders to OrderDetailViewDto
            ICollection<OrderDetailViewDto> detailDtos = allDetails.Select(detail => _mapper.Map<OrderDetailViewDto>(detail)).ToList();

            // If pageNumber or pageSize are 0 or negative, show all order details without pagination
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BasePaginatedList<OrderDetailViewDto>((IReadOnlyCollection<OrderDetailViewDto>)detailDtos, detailDtos.Count, 1, detailDtos.Count);
            }

            // validate and adjust page number
            pageNumber = PaginationHelper.ValidateAndAdjustPageNumber(pageNumber, detailDtos.Count, pageSize);

            // Return the paginated DTOs without reapplying pagination
            return _unitOfWork.GetRepository<OrderDetailViewDto>().GetPaggingDto(detailDtos, pageNumber, pageSize);
        }

        // Validate if the subject has been assigned before 
        public async Task<bool> IsValidStudentSubjectBeforeCreateOrder(CartCreateDto orderCreateDto)
        {
            // No need for a duplicate check since each CartCreateDto now represents a single Subject-Student pair

            // Validation process
            // Get student assigned subject
            IQueryable<OrderDetail> query = _unitOfWork.GetRepository<OrderDetail>().Entities
                .Include(d => d.Order)
                .Where(d => d.StudentId.Equals(orderCreateDto.StudentId)
                            && d.Order!.Status == PaymentStatusHelper.SUCCESS.ToString());

            List<OrderDetail> studentCurrentLearningSubjects = await query.ToListAsync();

            // Check if the student already has the subject assigned
            foreach (OrderDetail studentSubject in studentCurrentLearningSubjects)
            {
                if (studentSubject.SubjectId.Equals(orderCreateDto.SubjectId))
                {
                    return false; // Subject has already been assigned to the student
                }
            }

            return true; // Subject has not been assigned yet
        }

        // Check if OrderDetail exists by SubjectId and UserId
        public async Task<bool> IsOrderDetailExistsAsync(string userId, string subjectId)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(subjectId))
            {
                throw new BaseException.BadRequestException("invalid_input", "User ID and Subject ID cannot be null or empty.");
            }

            // Query OrderDetail based on SubjectId and UserId
            IQueryable<OrderDetail> query = _unitOfWork.GetRepository<OrderDetail>().Entities
                .Where(od => od.StudentId.Equals(userId) && od.SubjectId.Equals(subjectId));

            // Check if any record matches the query
            bool exists = await query.AnyAsync();

            return exists;
        }

    }
}
