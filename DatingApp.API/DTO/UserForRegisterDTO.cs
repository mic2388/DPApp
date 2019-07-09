using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
    public class UserForRegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="Password should be between 4 and 8 chars")]
        public string Password { get; set; }
    }
}