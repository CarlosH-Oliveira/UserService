using System.ComponentModel.DataAnnotations;

namespace UserService.Models.User.DTOS
{
    public class RecoverPasswordDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
