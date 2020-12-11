using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace APIContagem.Security
{
    public class SigningConfigurations
    {
        public Guid Id { get; } = Guid.NewGuid();
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(IConfiguration configuration)
        {
            Key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Secret-JWTKey"]));

            SigningCredentials = new (
                Key, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}