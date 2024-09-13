namespace ElementaryMathStudyWebsite.Contract.UseCases.IAppServices
{
    public interface IAppUserServices
    {
        // Get user name by id
        Task<string?> GetUserNameAsync(string userId);

        // Check if the relationship between two users is parents and child
        Task<bool> IsCustomerChildren(string parentId, string studentId);
    }
}
