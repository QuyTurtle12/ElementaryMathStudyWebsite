using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.Services.IDomainInterface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class PaymentService : IPaymentService, IAppPaymentServices
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IOrderService _orderService;
        private readonly ISubjectService _subjectService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IGenericRepository<Payment> paymentRepository, IGenericRepository<Order> orderRepository, IOrderService orderService, ISubjectService subjectService, IUserService userService,IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _orderService = orderService;
            _subjectService = subjectService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentViewDto> Checkout(OptionCreateDto optionCreateDto)
        {

            throw new NotImplementedException();
        }

        public Task<PaymentViewDto> Checkout(OrderCreateDto optionCreateDto)
        {
            throw new NotImplementedException();
        }

        public async Task<Payment> GetPaymentById(string paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId) ?? throw new KeyNotFoundException("Invalid payment ID");
            return payment;
        }

        public async Task<BasePaginatedList<PaymentViewDto>> GetPaymentHistory(int pageNumber, int pageSize)
        {
            var appUserService = _userService as IAppUserServices;
            string customerId = appUserService.GetActionUserId();

            IQueryable<Payment> query = _paymentRepository.Entities.Where(q => q.CustomerId == customerId);
            List<PaymentViewDto> paymentViewDtos = new();

            //If params negative = show all
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var allPayments = await query.ToListAsync();

                foreach (var payment in allPayments)
                {
                    paymentViewDtos.Add(PaymentViewMapper(payment));
                }
                return new BasePaginatedList<PaymentViewDto>(paymentViewDtos, paymentViewDtos.Count, 1, paymentViewDtos.Count);
            }

            // Show with pagination
            BasePaginatedList<Payment>? paginatedPayments = await _paymentRepository.GetPagging(query, pageNumber, pageSize);

            foreach (var payment in paginatedPayments.Items)
            {
                paymentViewDtos.Add(PaymentViewMapper(payment));
            }

            return new BasePaginatedList<PaymentViewDto>(paymentViewDtos, paginatedPayments.TotalItems, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<Payment>> GetPayments(int pageNumber, int pageSize)
        {
            IQueryable<Payment> query = _paymentRepository.Entities;

            // Negative params = show all 
            if (pageNumber <= 0 || pageSize <= 0)
            {
                List<Payment> allPayments = query.ToList();
                return new BasePaginatedList<Payment>(allPayments, allPayments.Count, 1, allPayments.Count);
            }

            // Show with pagination
            return await _paymentRepository.GetPagging(query, pageNumber, pageSize);

        }

        private PaymentViewDto PaymentViewMapper(Payment payment)
        {
            return new PaymentViewDto
            {
                PaymentId = payment.Id,
                CustomerName = payment.User.FullName,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status
            };

        }
    }
}
