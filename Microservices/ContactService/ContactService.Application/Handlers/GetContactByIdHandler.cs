using ContactService.Domain;
using Shared.Kernel.Results;

namespace ContactService.Application;

public class GetContactByIdHandler : IRequestHandler<GetContactByIdQuery, Result<ContactDto>>
{
    private readonly IContactRepository _contactRepository;

    public GetContactByIdHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<Result<ContactDto>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdWithInformationsAsync(request.Id);
            if (contact == null)
                return Result<ContactDto>.Fail("Kişi bulunamadı");

            var contactDto = new ContactDto
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
            };

            return Result<ContactDto>.Success(contactDto);
        }
        catch (Exception ex)
        {
            return Result<ContactDto>.Fail($"Kişi getirilirken hata oluştu: {ex.Message}");
        }
    }
}