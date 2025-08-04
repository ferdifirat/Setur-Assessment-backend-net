using ContactService.Commands;
using ContactService.Domain;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Application;

public class DeleteContactHandler : IRequestHandler<DeleteContactCommand, Result>
{
    private readonly IContactRepository _contactRepository;

    public DeleteContactHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<Result> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(request.Id);
            if (contact == null)
                return Result.Fail("Kişi bulunamadı");

            // Soft delete instead of hard delete
            await _contactRepository.SoftDeleteAsync(request.Id, request.DeletedBy ?? "System");
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Kişi silinirken hata oluştu: {ex.Message}");
        }
    }
}