using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Services;
namespace Web.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        UsersContext db;
        public IUserServices _userServices;
        public HomeController(IUserServices userServices, UsersContext context)
        {
            _userServices = userServices;
            // _userServices = userServices;
            db = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            //return await db.Users.ToListAsync();
            var data = _userServices.GetAll();
            return Ok(data);
        }

        [HttpPost]
        public ActionResult PostUser(Users user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            Users users = db.Users.FirstOrDefault(x => x.Login == user.Login);
            if (users != null)
            {
                return BadRequest();             
            }
            var userData = _userServices.PostUser(user);
            return Ok(user);
            


        }
        [HttpPost("auth")]
        public IActionResult Authentication(AuthUser user)
        {
            var data = _userServices.Authenticate(user, ipAddress());
            return Ok(data);
            /*Users users = db.Users.FirstOrDefault(x=>x.Login==user.Login&&x.Password==user.Password);
            if (users != null)
            {
                return Ok(users);
            }
            return NotFound();*/
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
