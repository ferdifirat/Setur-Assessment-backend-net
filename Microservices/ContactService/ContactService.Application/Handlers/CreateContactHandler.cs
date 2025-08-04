using ContactService.Domain;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Application;


public class CreateContactHandler : IRequestHandler<CreateContactCommand, Result<Guid>>
{
    private readonly IContactRepository _contactRepository;

    public CreateContactHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<Result<Guid>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = new Contact
            {
                FirstName = request.Dto.FirstName,
                LastName = request.Dto.LastName,
                Company = request.Dto.Company,
                CreatedBy = request.CreatedBy ?? "System"
            };

            await _contactRepository.AddAsync(contact);
            return Result<Guid>.Success(contact.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail($"Kişi oluşturulurken hata oluştu: {ex.Message}");
        }
    }
}