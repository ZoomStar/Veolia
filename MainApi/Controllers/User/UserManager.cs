using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MainApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace MainApi.Controllers
{
    public class UserManager
    {
        public readonly DatabaseContext _context;

        public UserManager(DatabaseContext context)
        {
            _context = context;
        }
        public bool ValidateTokens(Users user, string Tokens)
        {
            Tokens TokensUser = _context.Tokens.Where(Rt => Rt.Token == Tokens).OrderByDescending(rt => rt.ExpiryDate).FirstOrDefault();

            if (TokensUser != null && TokensUser.IdUser == user.Id
                && TokensUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }


            return false;
        }

        public Users GetUserFromAccessToken(string accessToken, JWTSettings _jwtsettings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            SecurityToken securityToken;

            var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var userId = principle.FindFirst(ClaimTypes.Name)?.Value;
                return _context.Users.Where(usr => usr.Id == Convert.ToInt32(userId)).FirstOrDefault();
            }
            return null;
        }

        public Tokens GenerateTokens()
        {
            Tokens Tokens = new Tokens();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                Tokens.Token = Convert.ToBase64String(randomNumber);
            }
            Tokens.ExpiryDate = DateTime.UtcNow.AddMonths(1);
            return Tokens;
        }

        public string GenerateAccessToken(int userID, JWTSettings _jwtsettings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userID.ToString())
                }),
                Expires = DateTime.UtcNow.AddMonths(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task saveloginHistory(LoginDetails loginDetails, Users user)
        {
            LoginHistory loginHistory = new LoginHistory();

            loginHistory.IdUser = user.Id;
            loginHistory.LoginDateTime = DateTime.Now;
            await _context.AddAsync(loginHistory);
        }
        public async Task RecoverPassword(string email)
        {
            //do stuff
        }
    }
}
