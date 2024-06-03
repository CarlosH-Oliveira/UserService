using System.ComponentModel.DataAnnotations;

namespace UserService.Models.User.DTOS
{
    public class ChangeEmailDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }
    }
}
