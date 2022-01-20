using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserDB.Models
{
    public class User
    {
        [JsonIgnore]
        public string id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Login { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
        
       
    }
}
