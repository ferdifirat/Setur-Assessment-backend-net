using Shared.Kernel.Entities;
using System.ComponentModel.DataAnnotations;

namespace ContactService.Domain
{
    public class Contact : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Company { get; set; } = string.Empty;

        public virtual ICollection<ContactInformation> ContactInformations { get; set; } = new List<ContactInformation>();
    }
}
