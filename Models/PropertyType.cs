using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class PropertyType:BaseEntity
    {
       [Required]
        public string Name { get; set; }
    }
}