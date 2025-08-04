using ContactService.Domain;
using ContactService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace ContactService.Infrastructure.Repositories
{
    public class ContactRepository : BaseRepository<Contact>, IContactRepository
    {
        private readonly ContactDbContext _context;

        public ContactRepository(ContactDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Contact?> GetByIdWithInformationsAsync(Guid id)
        {
            return await _context.Contacts
                .Include(c => c.ContactInformations)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Contact>> GetAllWithInformationsAsync()
        {
            return await _context.Contacts
                .Include(c => c.ContactInformations)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contact>> GetActiveWithInformationsAsync()
        {
            return await _context.Contacts
                .Include(c => c.ContactInformations)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }
    }
}
