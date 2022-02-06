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
using Newtonsoft.Json;
using AW.API.Services;

namespace AW.API
{
 
    [ApiController]
    public class AccountController :  Controller 
    { 
        private readonly IOptions<List<UserToLogin>> _users;
        private readonly IEncryptionService _encryptionService; 
        public AccountController(IOptions<List<UserToLogin>> users, IEncryptionService EncryptionService)
        { 
            _users = users;
            this._encryptionService = EncryptionService;
            
        } 
        [HttpGet("api/[controller]/Index")]
        public IActionResult Index()
        {
            return View("~/Account/Login.cshtml");
        }
        [Route("api/[controller]/GetCred/{serve}")]
        [HttpGet]
        public async Task<IActionResult> GetCred(string serve)
        {
            string salt = _encryptionService.CreateSalt(24);
            string hash = _encryptionService.Encrypt(serve, salt);
            return Json(new { cred = hash });
        }
        [Route("api/[controller]/Login")]
        [HttpPost] 
        public async Task<IActionResult> Login([FromForm] UserToLogin request)
        { 
            var user = _users.Value.Find(c => c.User == request.User && c.Attempt == request.Attempt);

            if (!(user is null))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,request.User),
                    new Claim("FullName", request.User),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

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
