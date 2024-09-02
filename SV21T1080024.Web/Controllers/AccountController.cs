using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080024.BusinessLayers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SV21T1080024.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.UserName = username;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                return View();
            }

            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại!");
                return View();
            }

            //Đăng nhập thành công, tạo dữ liệu để lưu thông tin đăng nhập
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList()
            };
            //Thiết lập phiên đăng nhập cho tài khoản
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            //Redirec về trang chủ sau khi đăng nhập
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string userName = "", string oldPassword = "", string newPassword = "", string confirmPassword = "")
        {
            if (string.IsNullOrWhiteSpace(oldPassword))
                ModelState.AddModelError(nameof(oldPassword), "Mật khẩu cũ không được để trống");
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError(nameof(newPassword), "Vui lòng nhập mật khẩu mới");
            if (string.IsNullOrWhiteSpace(confirmPassword))
                ModelState.AddModelError(nameof(confirmPassword), "Vui lòng nhập xác nhận lại mật khẩu");
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(nameof(confirmPassword), "Mật khẩu mới và mật khẩu xác nhận không giống nhau! Vui lòng nhập lại");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool changePassword = UserAccountService.ChangePassword(userName, oldPassword, newPassword);
            if (!changePassword)
            {
                ModelState.AddModelError(nameof(oldPassword), "Mật khẩu cũ không chính xác");
            }
            else
            {
                return RedirectToAction("Logout");
            }    
            return View();
        }
        public async Task<IActionResult> AccessDenined()
        {
            return View();
        }
    }
}
