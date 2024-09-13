using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class OrderDetailService : IOrderDetailService, IAppOrderDetailServices
    {
        private readonly IGenericRepository<OrderDetail> _detailReposiotry;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ISubjectService _subjectService;

        // Constructor
        public OrderDetailService(IGenericRepository<OrderDetail> detailReposiotry, IUnitOfWork unitOfWork, IUserService userService, ISubjectService subjectService)
        {
            _detailReposiotry = detailReposiotry ?? throw new ArgumentNullException(nameof(detailReposiotry));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _subjectService = subjectService ?? throw new ArgumentNullException(nameof(subjectService));
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

    }
}
