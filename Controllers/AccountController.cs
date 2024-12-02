using Chat.Models.EF;
using Chat.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Chat.Controllers
{
    public class AccountController : Controller
    {
        private Chat_DbContext db = new Chat_DbContext();

        [HttpGet] // Action dành cho truy cập bằng URL trực tiếp
        public ActionResult Register()
        {
            return View();
        }
        // Đăng ký
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid || model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Invalid data or passwords do not match.");
                return View(model);
            }

            if (db.Users.Any(u => u.Username == model.Username || u.Email == model.Email))
            {
                ModelState.AddModelError("", "Username or Email already exists.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                CreatedDate = DateTime.Now,
                Status = "active"
            };

            db.Users.Add(user);
            db.SaveChanges(); 
            return RedirectToAction("Login");
        }
        [HttpGet] // Action dành cho truy cập bằng URL trực tiếp
        public ActionResult Login()
        {
            return View();
        }

        // Đăng nhập
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid data.");
                return View(model);
            }

            var user = db.Users
                .FirstOrDefault(u => u.Username == model.Username && u.Status == "active");

            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            // Tạo session
            Session["UserId"] = user.UserId.ToString();
            Session["Username"] = user.Username;

            return RedirectToAction("Index", "Chat");
        }

        // Mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashed = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashed);
            }
        }

        // Kiểm tra mật khẩu
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashed = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputPassword));
                return Convert.ToBase64String(hashed) == storedHash;
            }
        }
    }

}