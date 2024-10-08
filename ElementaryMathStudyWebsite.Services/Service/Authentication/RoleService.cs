﻿using AutoMapper;
using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using System.Linq.Expressions;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppUserServices _appUserServices;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper, IAppUserServices appUserServices)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appUserServices = appUserServices;
        }

        public async Task<BasePaginatedList<Role>> GetAllRolesAsync(int pageNumber, int pageSize)
        {
            // Validate pageNumber and pageSize
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10; // Set a default page size if invalid

            Expression<Func<Role, bool>> condition = role => true && role.DeletedBy == null;


            // Use GetEntitiesWithCondition with includes to get the queryable set of users
            IQueryable<Role> query = _unitOfWork.GetRepository<Role>().GetEntitiesWithCondition(condition);

            // Retrieve paginated users from the repository
            var paginatedRoles = await _unitOfWork.GetRepository<Role>().GetPagging(query, pageNumber, pageSize);

            // Return the paginated result with users
            return new BasePaginatedList<Role>(paginatedRoles.Items, paginatedRoles.TotalItems, paginatedRoles.CurrentPage, paginatedRoles.PageSize);
        }

        public async Task<Role> CreateRoleAsync(RequestRole dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            // Use AutoMapper to map 
            var role = _mapper.Map<Role>(dto);


            _appUserServices.AuditFields(role, isCreating: true);

            // Add role to the repository
            await _unitOfWork.GetRepository<Role>().InsertAsync(role);
            await _unitOfWork.SaveAsync();

            return role;
        }

        public async Task<Role> UpdateRoleAsync(string roleId, RequestRole dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            Expression<Func<Role, bool>> condition = role => true && role.DeletedBy == null && role.Id.Equals(roleId);

            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(condition);

            if (role == null)
            {
                throw new BaseException.NotFoundException("not_found", "Role not found");
            }

            if (dto.RoleName != null)
            {
                role.RoleName = dto.RoleName;
            }


            _appUserServices.AuditFields(role);

            // Add role to the repository
            await _unitOfWork.GetRepository<Role>().UpdateAsync(role);
            await _unitOfWork.SaveAsync();

            return role;
        }

        public async Task<Role> GetRoleByIdAsync(string roleId)
        {

            Expression<Func<Role, bool>> condition = role => true && role.DeletedBy == null && role.Id.Equals(roleId);

            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(condition);

            if (role == null)
            {
                throw new BaseException.NotFoundException("not_found", "Role not found");
            }

            return role;
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            // Fetch the role by ID
            Expression<Func<Role, bool>> condition = role => true && role.DeletedBy == null && role.Id.Equals(roleId);

            var role = await _unitOfWork.GetRepository<Role>().FindByConditionAsync(condition);

            if (role == null)
            {
                throw new BaseException.NotFoundException("not_found", "Role not found");
            }

            // Set audit fields
            _appUserServices.AuditFields(role, false, true);

            // Update the role in the repository
            await _unitOfWork.GetRepository<Role>().UpdateAsync(role);
            await _unitOfWork.SaveAsync();

            return true; // Return true if the role was successfully disabled
        }
    }
}
