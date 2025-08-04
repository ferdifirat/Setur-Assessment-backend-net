using ContactService.Commands;
using ContactService.Domain;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Application;


public class CreateContactInformationHandler : IRequestHandler<CreateContactInformationCommand, Result<Guid>>
{
    private readonly IContactInformationRepository _contactInformationRepository;
    private readonly IContactRepository _contactRepository;

    public CreateContactInformationHandler(
        IContactInformationRepository contactInformationRepository,
        IContactRepository contactRepository)
    {
        _contactInformationRepository = contactInformationRepository;
        _contactRepository = contactRepository;
    }

    public async Task<Result<Guid>> Handle(CreateContactInformationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(request.Dto.ContactId);
            if (contact == null)
                return Result<Guid>.Fail("Kişi bulunamadı");

            var contactInformation = new ContactInformation
            {
                ContactId = request.Dto.ContactId,
                Type = request.Dto.Type,
                Value = request.Dto.Value
            };

            await _contactInformationRepository.AddAsync(contactInformation);
            return Result<Guid>.Success(contactInformation.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail($"İletişim bilgisi oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}