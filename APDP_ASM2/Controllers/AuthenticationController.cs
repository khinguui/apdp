using APDP_ASM2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using APDP_ASM2.Helpers;

namespace APDP_ASM2.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Pass))
            {
                ViewBag.error = "Username and Password cannot be empty!";
                return View("Login");
            }

            var users = FileHelper.LoadFromFile<List<User>>("users.json");
            var result = users?.FirstOrDefault(u => u.UserName == user.UserName);

            if (result != null && result.Pass == user.Pass)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.UserName),
                new Claim(ClaimTypes.Role, result.Role)
            };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToRoleDashboard(result.Role);
            }

            ViewBag.error = "Invalid username or password!";
            return View("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid || user.Pass != user.ConfirmPass)
            {
                ModelState.AddModelError("ConfirmPass", "Passwords do not match");
                return View("Register", user);
            }

            var users = LoadFromFile<List<User>>("users.json") ?? new List<User>();

            if (users.Any(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "Username already exists");
                return View("Register", user);
            }

            users.Add(user);
            SaveToFile("users.json", users);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register() => View();

        private IActionResult RedirectToRoleDashboard(string role)
        {
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Teacher" => RedirectToAction("Index", "Teacher"),
                "Student" => RedirectToAction("Index", "Student"),
                _ => RedirectToAction("Index", "Home"),
            };
        }

        private T? LoadFromFile<T>(string fileName)
        {
            if (!System.IO.File.Exists(fileName)) return default;
            var readText = System.IO.File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<T>(readText);
        }

        private void SaveToFile<T>(string fileName, T data)
        {
            var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(fileName, jsonString);
        }
    }
}