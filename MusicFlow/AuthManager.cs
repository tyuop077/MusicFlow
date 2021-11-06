using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MusicFlow.Context;

namespace MusicFlow
{
    public class AuthManager
    {
        private readonly string tokenKey;

        public AuthManager(string tokenKey) => this.tokenKey = tokenKey;

        public readonly IDictionary<string, string> tokens = new Dictionary<string, string>();

        public string Login(string username, string password, UsersContext db)
        {
            if (!db.Users.Any(
                User => User.Username == username && User.Password == password)
            ) return null;

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(tokenKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // We're using ClaimsIdentity and dictionary

            string tokenString = tokenHandler.WriteToken(token);
            tokens.Add(tokenString, username);

            return tokenString;
        }
    }
}
