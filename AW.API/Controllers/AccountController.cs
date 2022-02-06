using AW.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks; 

namespace AW.API
{
 
    [ApiController]
    public class AccountController :  Controller 
    { 
        private readonly IOptions<List<UserToLogin>> _users;
        public AccountController(IOptions<List<UserToLogin>> users)
        { 
            _users = users;
        }

        [HttpGet("api/[controller]/Index")]
        public IActionResult Index()
        {
            return View("~/Account/Login.cshtml");
        }
   
        [Route("api/[controller]/Login")]
        [HttpPost] 
        public async Task<IActionResult> Login([FromForm] UserToLogin userToLogin)
        {
            var user = _users.Value.Find(c => c.UserName == userToLogin.UserName && c.Password == userToLogin.Password);

            if (!(user is null))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,userToLogin.UserName),
                    new Claim("FullName", userToLogin.UserName),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                { 
                    RedirectUri = "/Home/Index", 
                }; 
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            return Ok();
        }
    }
}
