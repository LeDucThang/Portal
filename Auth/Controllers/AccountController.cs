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
using Nancy.Json;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;

namespace Auth.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;

        private string UserId = "998310595831-dceeoaikv8ce1qls0v35h1fbd3uskiel.apps.googleusercontent.com";
        private string client_secret = "7Aur4sJBSIOXWv4gJdlDsu_B";
        private string redirect_uri = "https://localhost:5001/account/GoogleCallback";
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
            ViewBag.GgCallBack = redirect_uri;
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
                string gurl = "code=" + code + "&client_id=" + UserId +
                         "&client_secret=" + client_secret + "&redirect_uri=" + redirect_uri + "&grant_type=" + grant_type;
                var u = await POSTResultAsync(gurl);
                if (u != null)
                {
                    //Check có tồn tại trong danh sách user hay không? 
                    ApplicationUser applicationUser = _context.ApplicationUser
            .Where(au => au.Username.ToLower() == u.Username)
            .FirstOrDefault();
                    if (applicationUser != null)
                    {
                        string token = CreateToken(u.Id, u.Username);
                        Response.Cookies.Append("Token", token);
                        return Redirect("/");
                    }
                    return Redirect("/error");
                }
            }

            return Redirect("/");
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
                JavaScriptSerializer js = new JavaScriptSerializer();
                gLoginInfo gli = js.Deserialize<gLoginInfo>(googleAuth);

                // lấy thông tin của gmail
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(gli.id_token);
                ApplicationUser u = new ApplicationUser();
                u.Username = validPayload.Email;
                u.DisplayName = validPayload.Name;

                //string[] tokenArray = gli.id_token.Split(new Char[] { '.' }); 
                //JavaScriptSerializer js2 = new JavaScriptSerializer();
                //gLoginClaims glc2 = js2.Deserialize<gLoginClaims>(base64Decode(tokenArray[1]));

                return u;
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public string base64Decode(string data)
        {
            //add padding with '=' to string to accommodate C# Base64 requirements
            int strlen = data.Length + (4 - (data.Length % 4));
            char pad = '=';
            string datapad;

            if (strlen == (data.Length + 4))
            {
                datapad = data;
            }
            else
            {
                datapad = data.PadRight(strlen, pad);
            }

            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                // create byte array to store Base64 string
                byte[] todecode_byte = Convert.FromBase64String(datapad);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode: " + e.Message);
            }
        }
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