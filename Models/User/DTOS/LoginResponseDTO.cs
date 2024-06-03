using System.IdentityModel.Tokens.Jwt;

namespace UserService.Models.User.DTOS
{
    public class LoginResponseDTO
    {
        public JwtSecurityToken token;
        public LoginResponseDTO(JwtSecurityToken token)
        {
            this.token = token;
        }
    }
}
