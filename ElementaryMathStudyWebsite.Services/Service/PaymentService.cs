using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.IDomainInterface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class PaymentService : IPaymentService, IAppPaymentServices
    {
        private readonly IGenericRepository<Payment> _paymentReposiotry;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IOrderService _orderService;
        private readonly ISubjectService _subjectService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IGenericRepository<Payment> paymentRepository, IGenericRepository<Order> orderRepository, IOrderService orderService, ISubjectService subjectService, IUnitOfWork unitOfWork)
        {
            _paymentReposiotry = paymentRepository;
            _orderRepository = orderRepository;
            _orderService = orderService;
            _subjectService = subjectService;
            _unitOfWork = unitOfWork;
        }
        public async Task<PaymentViewDto> Checkout(OrderCreateDto orderCreateDto)
        {

            throw new NotImplementedException();
        }

        public Task<Payment> GetPaymentById(string paymentId)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<PaymentViewDto>> GetPaymentHistory(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<Payment>> GetPayments(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
