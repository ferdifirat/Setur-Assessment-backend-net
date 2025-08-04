using Shared.Kernel.Entities;

namespace Shared.Kernel.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<T>> GetActiveAsync();
        Task SoftDeleteAsync(Guid id, string deletedBy);
    }
}
