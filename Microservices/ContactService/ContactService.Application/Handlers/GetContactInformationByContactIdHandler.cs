using ContactService.Domain;
using MediatR;
using Shared.Kernel.Results;

namespace ContactService.Application;

public class GetContactInformationByContactIdHandler : IRequestHandler<GetContactInformationByContactIdQuery, Result<List<ContactInformationDto>>>
{
    private readonly IContactInformationRepository _contactInformationRepository;

    public GetContactInformationByContactIdHandler(IContactInformationRepository contactInformationRepository)
    {
        _contactInformationRepository = contactInformationRepository;
    }

    public async Task<Result<List<ContactInformationDto>>> Handle(GetContactInformationByContactIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var contactInformations = await _contactInformationRepository.GetByContactIdAsync(request.ContactId);

            var dtos = contactInformations.Select(ci => new ContactInformationDto
            {
                Id = ci.Id,
                ContactId = ci.ContactId,
                Type = ci.Type,
                Value = ci.Value
            }).ToList();

            return Result<List<ContactInformationDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<ContactInformationDto>>.Fail($"İletişim bilgileri getirilirken hata oluştu: {ex.Message}");
        }
    }
}
