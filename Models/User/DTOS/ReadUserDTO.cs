using System.ComponentModel.DataAnnotations;

namespace UserService.Models.User.DTOS
{
    public class ReadUserDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public bool Status { get; set; } = true;
        public string? AccessToken { get; set; }
    }
}
