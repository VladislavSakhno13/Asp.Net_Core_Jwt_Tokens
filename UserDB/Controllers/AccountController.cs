using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDB.Models;
using UserDB.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
namespace UserDB.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        public IUserService _userService;
        AccountContext db;
        public AccountController(AccountContext context, IUserService userService)
        {
            db = context;
            _userService = userService;
        }
        
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            return Ok(_userService.GetById(id));
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _userService.GetAll();
            return Ok(data);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult PostUser(User user)
        {
            //User _user = db.Users.FirstOrDefault(x => x.Login == user.Login);
            //if (user == null || _user!=null) return BadRequest();
            var data = _userService.PostUser(user, ipAddress());
            return Ok(data);
        }

        [HttpPost("auth_user")]     
        [AllowAnonymous]
        public IActionResult AuthUser(AuthUser user)
        {
            User users = db.Users.FirstOrDefault(x => x.Login == user.Login && x.Password == user.Password);
            var authData = _userService.AuthUsers(user, ipAddress());
            if(authData==null) return BadRequest(new { message = "Username or password is incorrect" });
            setTokenCookie(authData.RefreshToken);
            db.Users.Update(users);
            db.SaveChanges();
            return Ok(authData);
        }
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }
        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(string id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = _userService.RevokeToken(token, ipAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}
