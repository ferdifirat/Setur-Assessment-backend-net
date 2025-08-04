using ContactService.Domain;

namespace ContactService.Application;

public class ContactInformationDto
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ContactInfoType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}
