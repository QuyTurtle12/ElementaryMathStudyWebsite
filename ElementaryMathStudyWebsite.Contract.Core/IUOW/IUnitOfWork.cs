using ElementaryMathStudyWebsite.Core.Base;

namespace ElementaryMathStudyWebsite.Contract.Core.IUOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        void Save();
        Task SaveAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollBack();
        bool IsValid<T>(string id) where T : BaseEntity;
    }
}
