using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserServices
    {
        /// <summary>
        /// Creates a new user based on the provided data.
        /// </summary>
        Task<User> CreateUserAsync(CreateUserDto dto);

        /// <summary>
        /// Updates an existing user's details using the provided data.
        /// </summary>
        Task<User> UpdateUserAsync(string userId, UpdateUserDto dto);

        /// <summary>
        /// Updates an existing user's details using the provided data.
        /// </summary>
        Task<User> UpdateProfileAsync(string userId, RequestUpdateProfileDto dto);

        /// <summary>
        /// Retrieves the username for the given user ID.
        /// </summary>
        Task<string> GetUserNameAsync(string userId);

        /// <summary>
        /// Checks if the specified student is a child of the specified parent.
        /// </summary>
        Task<bool> IsCustomerChildren(string parentId, string studentId);

        /// <summary>
        /// Sets audit fields for an entity when creating or disabling.
        /// </summary>
        void AuditFields(BaseEntity entity, bool isCreating = false, bool isDisable = false);

		void AuditFields(string userId, BaseEntity entity, bool isCreating = false, bool isDisable = false);

		/// <summary>
		/// Retrieves the ID of the user performing the action.
		/// </summary>
		string GetActionUserId();

        /// <summary>
        /// Retrieves a paginated list of all users.
        /// </summary>
        Task<BasePaginatedList<User>> GetAllUsersAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        Task<User?> GetUserByIdAsync(string userId);

        /// <summary>
        /// Retrieves the current logged-in user.
        /// </summary>
        Task<User> GetCurrentUserAsync();

        /// <summary>
        /// Disables a user by their ID.
        /// </summary>
        Task<bool> DisableUserAsync(string userId);

        /// <summary>
        /// Enable a user by their ID.
        /// </summary>
        Task<bool> EnableUserAsync(string userId);

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        Task<bool> DeleteUserAsync(string userId);

        /// <summary>
        /// Retrieves a list of all users along with their roles.
        /// </summary>
        Task<List<User>> GetAllUsersWithRolesAsync();

        /// <summary>
        /// Searches users based on the given criteria with pagination support.
        /// </summary>
        Task<BasePaginatedList<User>> SearchUsersAsync(string? name, bool? status, string? phone, string? email, int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves a paginated list of children associated with a specific parent.
        /// </summary>
        Task<BasePaginatedList<User>> GetChildrenOfParentAsync(string parentId, int pageNumber, int pageSize);

        /// <summary>
        /// Checks if the username is already used.
        /// </summary>
        Task<bool> CheckExistingUserName(string username);
    }
}
