using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserDB.Models
{
    public class AuthResponse
    {
        public string id { get; set; }
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }
        public string JwtToken { get; set; }

        //[JsonIgnore] 
        public string RefreshToken { get; set; }

        public AuthResponse(User user, string token, string refreshToken)
        {
            id = user.id;
            Login = user.Login;
            UserName = user.UserName;
            Roles = user.Roles;
            JwtToken = token;
            RefreshToken = refreshToken;
        }
    }
}
