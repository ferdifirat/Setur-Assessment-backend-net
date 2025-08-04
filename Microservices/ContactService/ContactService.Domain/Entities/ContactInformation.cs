using Shared.Kernel.Entities;
using System.ComponentModel.DataAnnotations;

namespace ContactService.Domain
{

    public class ContactInformation : BaseEntity
    {
        public Guid ContactId { get; set; }
        public virtual Contact Contact { get; set; } = null!;

        [Required]
        public ContactInfoType Type { get; set; }

        [Required]
        [MaxLength(500)]
        public string Value { get; set; } = string.Empty;
    }

    public enum ContactInfoType
    {
        Phone,
        Email,
        Location
    }
}
