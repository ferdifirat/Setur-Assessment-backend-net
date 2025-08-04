using ContactService.Commands;
using ContactService.Domain;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Application;

public class UpdateContactHandler : IRequestHandler<UpdateContactCommand, Result>
{
    private readonly IContactRepository _contactRepository;

    public UpdateContactHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<Result> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id);
            if (contact == null)
                return Result.Fail("Kişi bulunamadı");

            contact.FirstName = request.Dto.FirstName;
            contact.LastName = request.Dto.LastName;
            contact.Company = request.Dto.Company;
            contact.UpdatedBy = request.UpdatedBy ?? "System";

            await _contactRepository.UpdateAsync(contact);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Kişi güncellenirken hata oluştu: {ex.Message}");
        }
    }
}