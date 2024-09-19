using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class PaymentService : IPaymentService, IAppPaymentServices
    {
        private readonly IAppOrderServices _orderService;
        private readonly IAppSubjectServices _subjectService;
        private readonly IAppUserServices _userService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IAppOrderServices orderService, IAppSubjectServices subjectService, IAppUserServices userService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _subjectService = subjectService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentViewDto> Checkout(OrderCreateDto optionCreateDto)
        {
            var orderId = optionCreateDto.OrderId;

            // Fetch the order associated with the payment
            var order = await _unitOfWork.GetRepository<Order>()
                                         .Entities
                                         .Include(o => o.User)
                                         .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            Payment payment = new()
            {
                Id = Guid.NewGuid().ToString().ToUpper(),
                CustomerId = order.CustomerId,
                OrderId = orderId,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "VnPay",
                Amount = order.TotalPrice,
                Status = "Success"
            };

            await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
            await _unitOfWork.SaveAsync();

            PaymentViewDto paymentViewDto = new()
            {
                PaymentId = payment.Id,
                CustomerName = order.User.FullName,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status
            };

            foreach (var subjectStudent in optionCreateDto.SubjectStudents)
            {
                var subject = await _unitOfWork.GetRepository<Subject>()
                                               .GetByIdAsync(subjectStudent.SubjectId);
                var student = await _unitOfWork.GetRepository<User>()
                                               .GetByIdAsync(subjectStudent.StudentId);

                if (subject != null && student != null)
                {
                    PaymentSubjectStudent paymentSubjectStudent = new()
                    {
                        SubjectName = subject.SubjectName,
                        StudentName = student.FullName
                    };
                    paymentViewDto.SubjectStudents.Add(paymentSubjectStudent);
                }
            }

            return paymentViewDto;
        }

        public async Task<Payment> GetPaymentById(string paymentId)
        {
            var payment = await _unitOfWork.GetRepository<Payment>()
                                           .GetByIdAsync(paymentId)
                                           ?? throw new KeyNotFoundException("Invalid payment ID");
            return payment;
        }

        public async Task<BasePaginatedList<PaymentViewDto>> GetPaymentHistory(int pageNumber, int pageSize)
        {
            string customerId = _userService.GetActionUserId();

            IQueryable<Payment> query = _unitOfWork.GetRepository<Payment>()
                                                   .Entities
                                                   .Where(q => q.CustomerId == customerId);
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
            var paginatedPayments = await _unitOfWork.GetRepository<Payment>().GetPagging(query, pageNumber, pageSize);

            foreach (var payment in paginatedPayments.Items)
            {
                paymentViewDtos.Add(PaymentViewMapper(payment));
            }

            return new BasePaginatedList<PaymentViewDto>(paymentViewDtos, paginatedPayments.TotalItems, pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<Payment>> GetPayments(int pageNumber, int pageSize)
        {
            IQueryable<Payment> query = _unitOfWork.GetRepository<Payment>().Entities;

            // Negative params = show all 
            if (pageNumber <= 0 || pageSize <= 0)
            {
                List<Payment> allPayments = await query.ToListAsync();
                return new BasePaginatedList<Payment>(allPayments, allPayments.Count, 1, allPayments.Count);
            }

            // Show with pagination
            return await _unitOfWork.GetRepository<Payment>().GetPagging(query, pageNumber, pageSize);
        }

        private PaymentViewDto PaymentViewMapper(Payment payment)
        {
            return new PaymentViewDto
            {
                PaymentId = payment.Id,
                CustomerName = payment.User?.FullName ?? "Unknown",
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status
            };
        }
    }
}
