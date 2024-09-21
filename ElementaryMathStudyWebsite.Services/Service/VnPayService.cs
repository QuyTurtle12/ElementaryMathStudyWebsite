﻿using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Store;
using ElementaryMathStudyWebsite.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace ElementaryMathStudyWebsite.Services.Service
{
    public class VnPayService : IVnPayService
    {
        private readonly IAppUserServices _userService;
        private readonly IUnitOfWork _unitOfWork;


        public VnPayService(IAppUserServices appUserService, IUnitOfWork unitOfWork)
        {
            _userService = appUserService;
            _unitOfWork = unitOfWork;
        }
        public async Task<string> CreatePaymentUrl(HttpContext context)
        {
            // Get logged in User
            User currentUser = await _userService.GetCurrentUserAsync();

            IQueryable<Order> query = _unitOfWork.GetRepository<Order>().Entities.Where(o => o.CustomerId == currentUser.Id && o.Status == PaymentStatusHelper.CART.ToString());

            if (query.Count() <= 0) throw new BaseException.BadRequestException(
                "invalid argument",
                "You have no items in your cart"
            );

            var cart = query.First();

            var tick = DateTime.Now.Ticks.ToString();

            VnPayHelper vnpay = new();
            vnpay.AddRequestData("vnp_Version", VnPayConfig.Version);
            vnpay.AddRequestData("vnp_Command", VnPayConfig.Command);
            vnpay.AddRequestData("vnp_TmnCode", VnPayConfig.TmnCode);
            vnpay.AddRequestData("vnp_Amount", (cart.TotalPrice * 100).ToString());
            //Số tiền thanh toán. Số tiền không 
            //mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
            //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
            //là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", CoreHelper.SystemTimeNow.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", VnPayConfig.CurrCode);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", VnPayConfig.Locale);
            vnpay.AddRequestData("vnp_OrderInfo", cart.Id);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", VnPayConfig.ReturnUrl);
            vnpay.AddRequestData("vnp_ExpireDate", CoreHelper.SystemTimeNow.AddMinutes(5).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_TxnRef", $"{tick}");

            cart.Status = PaymentStatusHelper.PENDING.ToString();
            cart.PaymentMethod = "VnPay";
            await _unitOfWork.SaveAsync();

            var paymentUrl = vnpay.CreateRequestUrl(VnPayConfig.BaseUrl, VnPayConfig.HashSecret);

            return paymentUrl;
        }

        public VnPayResponseDto PaymentExecute(IQueryCollection collections)
        {
            VnPayHelper vnpay = new();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value.ToString();
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, VnPayConfig.HashSecret);
            if (!checkSignature)
            {
                return new VnPayResponseDto
                {
                    Success = false
                };
            }
            return new VnPayResponseDto
            {
                Success = true,
                OrderId = vnp_orderId.ToString(),
                VnPayResponseCode = vnp_ResponseCode
            };

        }

    }
}
