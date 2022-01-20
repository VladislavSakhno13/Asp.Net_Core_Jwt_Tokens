using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AuthenticateResponse
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string JwtToken { get; set; }
        [JsonIgnore] 
        public string RefreshToken { get; set; }
        public AuthenticateResponse(Users user, string jwtToken, string refreshToken)
        {
            
            Name = user.Name;
            Login = user.Login;
            Password = user.Password;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
