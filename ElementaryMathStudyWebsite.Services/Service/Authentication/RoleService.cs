using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices.Authentication;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Services.Service.Authentication
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BasePaginatedList<Role>> GetAllRolesAsync(int pageNumber, int pageSize)
        {
            // Validate pageNumber and pageSize
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10; // Set a default page size if invalid


            // Use GetEntitiesWithCondition with includes to get the queryable set of users
            IQueryable<Role> query = _unitOfWork.GetRepository<Role>().Entities;

            // Retrieve paginated users from the repository
            var paginatedRoles = await _unitOfWork.GetRepository<Role>().GetPagging(query, pageNumber, pageSize);

            // Return the paginated result with users
            return new BasePaginatedList<Role>(paginatedRoles.Items, paginatedRoles.TotalItems, paginatedRoles.CurrentPage, paginatedRoles.PageSize);
        }
    }
}
