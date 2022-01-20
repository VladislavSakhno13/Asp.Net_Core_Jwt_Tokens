using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Web.Entities;
using Web.Models;

namespace Web.Services
{
    public interface IUserServices
    {
        IEnumerable<Users> GetAll();
        Task<ActionResult<Users>> PostUser(Users user);
        AuthenticateResponse Authenticate(AuthUser model, string ipAddress);


    }
    public class UserServices : IUserServices
    {
        UsersContext db;
        private readonly AppSettings _appSettings;
        public UserServices(UsersContext context, IOptions<AppSettings> appSettings)
        {
            db = context;
            _appSettings = appSettings.Value;
        }
        public IEnumerable<Users> GetAll()
        {
            return db.Users;
        }
        public async Task<ActionResult<Users>> PostUser(Users user)
        {
            db.Add(user);
            await db.SaveChangesAsync();
            return user;
        }
        public AuthenticateResponse Authenticate(AuthUser model, string ipAddress)
        {
            var user = db.Users.SingleOrDefault(x => x.Login == model.Login && x.Password == model.Password);

            if (user == null) return null;

            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token
            user.RefreshTokens
                .Add(refreshToken);
            db.Update(user);
            db.SaveChanges();

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }
        private string generateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

    }
}
