using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserService.Services
{
    public class TokenHandler
    {
        private JwtSecurityTokenHandler _jwtHandler;

        public TokenHandler()
        {
            _jwtHandler = new JwtSecurityTokenHandler();
        }
        public string GenerateLoginToken(string email)
        {
            try
            {
                var expires = DateTime.UtcNow.AddMinutes(30);

                Claim[] claims = [
                    new Claim ("email", email),
                    new Claim ("exp", expires.ToString())
                    ];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9ASHDA98H9ah9ha9H9A89n0f9ASHDA98H9ah9ha9H9A89n0f"));

                var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                    (
                        expires: expires,
                        claims: claims,
                        signingCredentials: signInCredentials
                    );

                return _jwtHandler.WriteToken(token);
            }
            catch
            {
                throw;
            }
            
        }

        public string GeneratePasswordChangeToken(string email)
        {
            try
            {
                var expires = DateTime.UtcNow.AddMinutes(15);

                Claim[] claims = [
                    new Claim ("email", email),
                    new Claim ("exp", expires.ToString())
                    ];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hasbdjbaidwasmndjkwiuhdkasdknsldfjsdhbfbsebfjesk"));
                                                                            

                var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                    (
                        expires: expires,
                        claims: claims,
                        signingCredentials: signInCredentials
                    );

                return _jwtHandler.WriteToken(token);
            }
            catch
            {
                throw;
            }

        }

        public string GenerateEmailChangeToken(string email, string newEmail)
        {
            try
            {
                var expires = DateTime.UtcNow.AddMinutes(15);

                Claim[] claims = [
                    new Claim ("email", email),
                    new Claim ("newEmail", newEmail),
                    new Claim ("exp", expires.ToString())
                    ];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sadiwnqwidhasmkndwgjaksnbdjgwjvamasjdbwjbdkawndo"));
                                                                            
                var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken
                    (
                        expires: expires,
                        claims: claims,
                        signingCredentials: signInCredentials
                    );

                return _jwtHandler.WriteToken(token);
            }
            catch
            {
                throw;
            }

        }

        public List<Claim> ReadToken(string token)
        {
            try
            {
                var jwtToken = _jwtHandler.ReadJwtToken(token);

                var claims = jwtToken.Claims.ToList();

                return claims;
            }
            catch
            {
                throw;
            }
        }

        public bool VerifyExpirationTime(string token)
        {
            try
            {
                var jwtToken = _jwtHandler.ReadJwtToken(token);
                var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "exp");
                if (expClaim != null)
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                    if (expDate > DateTimeOffset.UtcNow)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
            
        }
    }
}
