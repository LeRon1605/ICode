using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUser user)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            AuthCredential credential = await _authService.Login(user);
            if (credential == null)
            {
                TempData["error"] = "Tài khoản không tồn tại";
                return View();
            }
            else
            {
                HttpContext.Response.Cookies.Append("access_token", credential.access_token);
                HttpContext.Response.Cookies.Append("refresh_token", credential.refresh_token);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser user)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            bool result = await _authService.Register(user);
            if (result)
            {
                return RedirectToAction("Login");
            }
            else
            {
                TempData["error"] = "Tài khoản đã tồn tại";
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPassword data)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            bool result = await _authService.RequestChangePassword(data);
            if (result)
            {
                TempData["message"] = "Kiểm tra email để tiếp tục";
            }
            else
            {
                TempData["error"] = "Tài khoản không tồn tại";
            }
            return View();
        }

        [HttpGet]
        [Route("auth/forget-password/callback")]
        public IActionResult ForgetPasswordForm(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.userId = userId;
            ViewBag.token = UrlEncoder.Default.Encode(token);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPasswordSubmit(string userId, string token, ForgetPasswordSubmit data)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ServiceResponse<bool> result = await _authService.ChangePassword(userId, token, data);
            if (result.State)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["error"] = result.Description;
                return View("ForgetPasswordForm");
            }
        }

        [Route("auth/google/callback")]
        public async Task<IActionResult> GoogleCallback(string code, string scope, string error)
        {
            if (error != null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpClient client = new HttpClient();
            HttpContent body = new StringContent(JsonConvert.SerializeObject(new {
                client_id = _configuration["Oauth:Google:ClientID"],
                client_secret = _configuration["Oauth:Google:ClientSecret"],
                code = code,
                grant_type = "authorization_code",
                redirect_uri = "https://localhost:8001/auth/google/callback"
            }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://oauth2.googleapis.com/token", body);
            if (response.IsSuccessStatusCode)
            {
                GoogleTokenResponse token = JsonConvert.DeserializeObject<GoogleTokenResponse>(await response.Content.ReadAsStringAsync());
                AuthCredential credential = await _authService.LoginByGoogle(token.access_token);
                if (credential != null)
                {
                    HttpContext.Response.Cookies.Append("access_token", credential.access_token);
                    HttpContext.Response.Cookies.Append("refresh_token", credential.refresh_token);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "Invalid token";
                    return View("Login");
                }
            }
            TempData["error"] = "Invalid";
            return View("Login");
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("access_token");
            HttpContext.Response.Cookies.Delete("refresh_token");
            return RedirectToAction("Index", "Home");
        }
    }
}
