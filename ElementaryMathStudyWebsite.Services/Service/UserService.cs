using ElementaryMathStudyWebsite.Contract.Core.IUOW;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Services.IDomainService;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IGenericRepository<User> userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<User?> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        // Get user name from database
        public async Task<string?> GetUserNameAsync(string userId)
        {
            try
            {
                User user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return string.Empty;
                }

                return user.FullName;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        // Check if the relationship between two users is parents and child
        public async Task<bool> IsCustomerChildren(string parentId, string studentId)
        {
            User? student = await _userRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                return false;
            }

            string? creatorId = student.CreatedBy;

            if (creatorId.Equals(parentId))
            {
                return true;
            } 
                
            return false;
            
        }
    }
}
