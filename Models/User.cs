using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User:BaseEntity
    {
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public byte[] Password { get; set; }

        public byte[] PasswordKey { get; set; }
    }

}
