using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MusicFlow.Entities;

namespace MusicFlow.Services
{
    public class AuthManager
    {
        private readonly byte[] secretKey;
        private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        private readonly HashAlgorithm algorithm = new SHA256Managed();
        public AuthManager(byte[] secret)
        {
            this.secretKey = secret;
        }
        public string GenerateToken(User user)
        {
            var tokenGenerator = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenGenerator));
        }
        public byte[] HashPassword(string password)
        {
            //byte[] saltedPlainPassword = new byte[password.Length + secretKey.Length];
            //Buffer.BlockCopy(Encoding.ASCII.GetBytes(password), 0, saltedPlainPassword, password.Length);
            byte[] saltedPlainPassword = Encoding.ASCII.GetBytes(password).Concat(secretKey).ToArray();
            return algorithm.ComputeHash(saltedPlainPassword);
        }
    }
}
