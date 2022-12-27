using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServerWeb.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServerWeb.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Account> _userManager;

        public TokenService(UserManager<Account> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public string GenerateToken(Account account)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, account.Email!),
                new Claim(ClaimTypes.Name, account.UserName!),
            };

            // specify key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:TokenKey"]!));

            // HmacSha512 is the strongest algorithm to sign a key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenOptions = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}