using Shared.Kernel.Repositories;

namespace ContactService.Domain
{
    public interface IContactInformationRepository : IBaseRepository<ContactInformation>
    {
        Task<IEnumerable<ContactInformation>> GetByContactIdAsync(Guid contactId);
    }
}
