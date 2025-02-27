using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestCaseBookShop.Models;
using TestCaseBookShop.Models.Data.Auth;
using TestCaseBookShop.Models.Data.Dto;

namespace TestCaseBookShop.Services
{
    public class TokenGenerateService
    {

        public interface ITokenService
        {
            TokenResponse CreateToken(User user); // Sadece token üretir
        }

        public class TokenService : ITokenService
        {
            private readonly IConfiguration _configuration;

            public TokenService(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public TokenResponse CreateToken(User user)
            {
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Token:Issuer"],
                    audience: _configuration["Token:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Token:Expiration"])),
                    signingCredentials: creds
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                return new TokenResponse(accessToken);
            }
        }

        public record TokenResponse(string AccessToken);
    }
}
