using ContactService.Domain;
using Shared.Kernel.Results;

namespace ContactService.Application;

public class DeleteContactInformationHandler : IRequestHandler<DeleteContactInformationCommand, Result>
{
    private readonly IContactInformationRepository _contactInformationRepository;

    public DeleteContactInformationHandler(IContactInformationRepository contactInformationRepository)
    {
        _contactInformationRepository = contactInformationRepository;
    }

    public async Task<Result> Handle(DeleteContactInformationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contactInformation = await _contactInformationRepository.GetByIdAsync(request.Id);
            if (contactInformation == null)
                return Result.Fail("İletişim bilgisi bulunamadı");

            await _contactInformationRepository.DeleteAsync(request.Id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Fail($"İletişim bilgisi silinirken hata oluştu: {ex.Message}");
        }
    }
}