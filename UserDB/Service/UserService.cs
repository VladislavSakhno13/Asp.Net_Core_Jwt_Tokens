using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDB.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserDB.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace UserDB.Service
{
    public class UserService:IUserService
    {
        private AccountContext _context;
        private readonly AppSettings _setting;
        public UserService(AccountContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _setting = appSettings.Value;
        }
        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }
        public IEnumerable<User> GetById(string id)
        {
            yield return _context.Users.FirstOrDefault(x => x.id == id);
        }
        public IEnumerable<User> PostUser(User user, string ipAddress)
        { 
            //var refreshToken = generateRefreshToken(ipAddress);
            //user.RefreshTokens.Add(refreshToken);
            //user.RefreshTokens.Add(refreshToken);
            _context.Users.Add(user);
            _context.SaveChanges();
            
            yield return user;
        }
        public AuthResponse AuthUsers(AuthUser user, string ipAddress)
        {
            User users = _context.Users.FirstOrDefault(x=>x.Login==user.Login&&x.Password==user.Password);
            var jwtToken = generateJwtToken(users);
            var refreshToken = generateRefreshToken(ipAddress);

            users.RefreshTokens.Add(refreshToken);
            _context.Update(users);
            _context.SaveChanges();
            return new AuthResponse(users, jwtToken, refreshToken.Token);
        }
        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_setting.Secret);
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
        public AuthResponse RefreshToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null) return null;
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) return null;
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new AuthResponse(user, jwtToken, newRefreshToken.Token);
        }
        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }



    }
}
