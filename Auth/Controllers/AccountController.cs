using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Auth.Models;
using Auth.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;

        public AccountController(ILogger<AccountController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous]
        public IActionResult Login([FromBody]AccountViewModel AccountViewModel)
        {
            ApplicationUser applicationUser = _context.ApplicationUser
                .Where(au => au.Username.ToLower() == AccountViewModel.Username.ToLower())
                .FirstOrDefault();
            if (applicationUser == null)
            {
                ViewBag.ReturnUrl = AccountViewModel.ReturnUrl;
                ViewBag.Username = AccountViewModel.Username;
                ViewBag.Error = "Tài khoản không tồn tại.";
                return View();
            }
            bool verified = VerifyPassword(applicationUser.Password, AccountViewModel.Password);
            if (!verified)
            {
                ViewBag.ReturnUrl = AccountViewModel.ReturnUrl;
                ViewBag.Username = AccountViewModel.Username;
                ViewBag.Error = "Bạn đã nhập sai mật khẩu.";
                return View();
            }

            string token = CreateToken(applicationUser.Id, applicationUser.Username);
            Response.Cookies.Append("Token", token);
            if (string.IsNullOrEmpty(AccountViewModel.ReturnUrl))
                return Redirect("/portal");
            return Redirect(AccountViewModel.ReturnUrl);
        }

        private bool VerifyPassword(string oldPassword, string newPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(oldPassword);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(newPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        private string CreateToken(long id, string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(StaticParams.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, userName)
                }),
                Expires = DateTime.UtcNow.AddSeconds(StaticParams.ExpiredTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken SecurityToken = tokenHandler.CreateToken(tokenDescriptor);
            string Token = tokenHandler.WriteToken(SecurityToken);
            return Token;
        }
    }
}
