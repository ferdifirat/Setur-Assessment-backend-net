using ContactService.Domain;
using System.ComponentModel.DataAnnotations;

namespace ContactService.Application;

public class CreateContactInformationDto
{
    [Required]
    public Guid ContactId { get; set; }

    [Required]
    public ContactInfoType Type { get; set; }

    [Required]
    [MaxLength(500)]
    public string Value { get; set; } = string.Empty;
}