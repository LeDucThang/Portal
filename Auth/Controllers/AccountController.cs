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
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;

namespace Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;

        private string clientId = StaticParams.GoogleClientId;
        private string clientSecret = StaticParams.GoogleClientSecret;
        private string redirectUri = StaticParams.GoogleRedirectUri;
        private string grant_type = "authorization_code";
        //private string grant_type = "refresh_token";

        public AccountController(ILogger<AccountController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            var stateNew = Guid.NewGuid();
            ViewBag.returnlink = returnUrl;
            ViewBag.State = stateNew;
            ViewBag.GgCallBack = redirectUri;
            ViewBag.ReturnUrl = returnUrl;
            HttpContext.Session.SetString(stateNew.ToString(), "State");
            return View();
        }

        //[HttpPost, AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromForm]AccountViewModel data)
        {
            ApplicationUser applicationUser = _context.ApplicationUser
                .Where(au => au.Username.ToLower() == data.Username.ToLower())
                .FirstOrDefault();
            if (applicationUser == null)
            {
                return Json(new
                {
                    error = true,
                    returnUrl = data.ReturnUrl,
                    username = data.Username,
                    message = "Tài khoản không tồn tại.",
                });
            }
            bool verified = VerifyPassword(applicationUser.Password, data.Password);
            if (!verified)
            {
                return Json(new
                {
                    error = true,
                    returnUrl = data.ReturnUrl,
                    username = data.Username,
                    message = "Bạn đã nhập sai mật khẩu.",
                });
            }

            string token = CreateToken(applicationUser.Id, applicationUser.Username);
            Response.Cookies.Append("Token", token);
            if (string.IsNullOrEmpty(data.ReturnUrl))
            {
                return Json(new
                {
                    error = false,
                    returnUrl = "/home/index",
                    username = data.Username,
                    message = "Đăng nhập thành công",
                });
            }
            return Json(new
            {
                error = false,
                returnUrl = data.ReturnUrl,
                username = data.Username,
                message = "Đăng nhập thành công",
            });
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


        // Đăng nhập bằng google 
        public async Task<ActionResult> GoogleCallback(string code)
        {
            if (code != null)
            {
                string gurl = "code=" + code + "&client_id=" + clientId +
                         "&client_secret=" + clientSecret + "&redirect_uri=" + redirectUri + "&grant_type=" + grant_type;
                var user = await POSTResultAsync(gurl);
                if (user != null)
                {
                    //Check có tồn tại trong danh sách user hay không? 
                    ApplicationUser applicationUser = _context.ApplicationUser
                        .Where(au => au.Username.ToLower() == user.Username)
                        .FirstOrDefault();
                    if (applicationUser != null)
                    {
                        string token = CreateToken(user.Id, user.Username);
                        Response.Cookies.Append("Token", token);
                        return Redirect("/");
                    }
                    return RedirectToAction("Login");
                }
            }

            return RedirectToAction("Login");
        }


        public async Task<ApplicationUser> POSTResultAsync(string e)
        {
            try
            {
                // variables to store parameter values
                string url = "https://accounts.google.com/o/oauth2/token";

                // creates the post data for the POST request
                string postData = (e);

                // create the POST request
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = postData.Length;

                // POST the data
                using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter2.Write(postData);
                }

                //This actually does the request and gets the response back
                HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();

                string googleAuth;

                using (StreamReader responseReader = new StreamReader(resp.GetResponseStream()))
                {
                    //dumps the HTML from the response into a string variable
                    googleAuth = responseReader.ReadToEnd();


                }

                gLoginInfo gli = JsonConvert.DeserializeObject<gLoginInfo>(googleAuth);

                // lấy thông tin của gmail
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(gli.id_token);
                ApplicationUser user = new ApplicationUser();
                user.Username = validPayload.Email;
                user.DisplayName = validPayload.Name;
                return user;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
    public class gLoginClaims
    {
        public string aud, iss, email_verified, at_hash, azp, email, sub;
        public int exp, iat;
    }
    public class gLoginInfo
    {
        public string access_token, token_type, id_token;
        public int expires_in;
    }
}
