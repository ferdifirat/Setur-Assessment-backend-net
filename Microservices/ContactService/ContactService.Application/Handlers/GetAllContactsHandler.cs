using ContactService.Domain;
using Shared.Kernel.Results;

namespace ContactService.Application;

public class GetAllContactsHandler : IRequestHandler<GetAllContactsQuery, Result<List<ContactDto>>>
{
    private readonly IContactRepository _contactRepository;

    public GetAllContactsHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<Result<List<ContactDto>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var contacts = await _contactRepository.GetActiveWithInformationsAsync();

            var contactDtos = contacts.Select(contact => new ContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Company = contact.Company,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt,
                ContactInformations = contact.ContactInformations.Select(ci => new ContactInformationDto
                {
                    Id = ci.Id,
                    Type = ci.Type,
                    Value = ci.Value,
                    CreatedAt = ci.CreatedAt
                }).ToList()
            }).ToList();

            return Result<List<ContactDto>>.Success(contactDtos);
        }
        catch (Exception ex)
        {
            return Result<List<ContactDto>>.Fail($"Kişiler getirilirken hata oluştu: {ex.Message}");
        }
    }
}