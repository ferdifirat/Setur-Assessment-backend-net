using Shared.Kernel.Repositories;

namespace ContactService.Domain
{
    public interface IContactRepository : IBaseRepository<Contact>
    {
        Task<Contact?> GetByIdWithInformationsAsync(Guid id);
        Task<IEnumerable<Contact>> GetAllWithInformationsAsync();
        Task<IEnumerable<Contact>> GetActiveWithInformationsAsync();
    }
}
