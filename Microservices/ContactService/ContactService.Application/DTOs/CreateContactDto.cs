using System.ComponentModel.DataAnnotations;

namespace ContactService.Application;

public class CreateContactDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Company { get; set; } = string.Empty;
}
