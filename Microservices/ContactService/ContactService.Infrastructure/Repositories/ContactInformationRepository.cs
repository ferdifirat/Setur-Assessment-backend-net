using ContactService.Domain;
using ContactService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace ContactService.Infrastructure.Repositories
{
    public class ContactInformationRepository : BaseRepository<ContactInformation>, IContactInformationRepository
    {
        private readonly ContactDbContext _context;

        public ContactInformationRepository(ContactDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactInformation>> GetByContactIdAsync(Guid contactId)
        {
            return await _context.ContactInformations
                .Where(ci => ci.ContactId == contactId)
                .ToListAsync();
        }
    }
}
