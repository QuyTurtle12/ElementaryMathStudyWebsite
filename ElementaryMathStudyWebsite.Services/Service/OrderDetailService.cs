﻿using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
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
            // Get all order details from database
            // If null then return empty collection
            IQueryable<OrderDetail> query = _unitOfWork.GetRepository<OrderDetail>()
                .Entities
                .Where(od => od.OrderId.Equals(orderId))
                .Include(od => od.Subject)
                .Include(od => od.User);

            IList<OrderDetailViewDto> detailDtos = new List<OrderDetailViewDto>();

            var allDetails = await query.ToListAsync();

            // Map orders to OrderViewDto
            foreach (var detail in allDetails)
            {
                var dto = _mapper.Map<OrderDetailViewDto>(detail);

                detailDtos.Add(dto);
            }

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
            // Check for duplicates in the SubjectStudents list
            var uniqueSubjectStudents = orderCreateDto.SubjectStudents
                .GroupBy(s => new { s.SubjectId, s.StudentId })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (uniqueSubjectStudents.Any())
            {
                // Return false or throw an exception if duplicates are found
                return false;
            }

            // Validation process
            foreach (var newSubject in orderCreateDto.SubjectStudents)
            {
                // Get student assigned subject
                IQueryable<OrderDetail> query = _unitOfWork.GetRepository<OrderDetail>().Entities.Where(d => d.StudentId.Equals(newSubject.StudentId));
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
