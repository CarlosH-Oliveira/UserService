using UserService.Models.User.DTOS;

namespace UserService.Models.UserMainService
{
    public class GetUsersResult
    {
        public bool Error { get; set; } = false;
        public int StatusCode { get; set; } = 0;
        public string Message { get; set; } = "";
        public List<ReadUserDTO>? ReadUsers { get; set; }
    }
}
