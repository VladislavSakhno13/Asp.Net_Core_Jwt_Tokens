using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDB.Models;

namespace UserDB.Service
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        IEnumerable<User> PostUser(User user, string ipAddress);
        IEnumerable<User> GetById(string id);
        AuthResponse AuthUsers(AuthUser user, string ipAddress);
        AuthResponse RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
    }
}
