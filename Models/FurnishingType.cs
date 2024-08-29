using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class FurnishingType:BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}