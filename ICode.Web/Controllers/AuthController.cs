using CodeStudy.Models;
using ICode.Web.Models.DTO;
using ICode.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ICode.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("access_token");
            HttpContext.Response.Cookies.Delete("refresh_token");
            return RedirectToAction("Index", "Home");
        }
    }
}
