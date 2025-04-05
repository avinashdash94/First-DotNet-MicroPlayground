using Mango.Service.AuthAPI.Models;
using Mango.Service.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Service.AuthAPI.Service
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions) { //We need IOptions<JwtOptions> as we are cofigutring the JwtOptions in Program.cs file, not injecting it directly
            _jwtOptions = jwtOptions.Value;
        }
        public string GenerateToken(ApplicationUser applicationUser)
        {

            var tokenHandler = new JwtSecurityTokenHandler(); // inbuild class for token generation
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret); // secret key for token generation

            var claimList = new List<Claim> // claims are used to store user information in the token like Email, user name etc
            {
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor // token descriptor is used to describe the token like expiration time, issuer, audience etc
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.UtcNow.AddDays(7), // token will expire in 7 days    
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // signing credentials are used to sign the token
            };

            var token = tokenHandler.CreateToken(tokenDescriptor); // create the token
            return tokenHandler.WriteToken(token); // write the token to string
        }
    }
}
