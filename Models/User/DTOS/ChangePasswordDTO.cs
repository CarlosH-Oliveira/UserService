using System.ComponentModel.DataAnnotations;

namespace UserService.Models.User.DTOS
{
    public class ChangePasswordDTO
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        public string AccessToken { get; set; }
    }
}
