using UserService.Models.User.DTOS;

namespace UserService.Models.UserMainService
{
    public class LoginResult
    {
        public bool Error { get; set; } = false;
        public int StatusCode { get; set; } = 0;
        public string Message { get; set; } = "";
        public ReadUserDTO? ReadUser { get; set; }
    }
}
