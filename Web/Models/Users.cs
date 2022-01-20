using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Web.Entities;

namespace Web.Models
{
    public class Users
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Login { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
