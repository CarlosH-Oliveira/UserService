using System.ComponentModel.DataAnnotations;

namespace UserService.Models.User
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? AuxPassword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set;}
        public bool Status { get; set; } = true;
    }
}
