using HLSMP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Net;
using HLSMP.ViewModel;
using HLSMP.Models;
using System.Text.Json;

namespace HLSMP.Controllers
{

    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly Dictionary<string, int> _roles = new()
{
            { "GIS Lab", 1 },
            { "Service of India", 2 },
            { "Revenue Department", 3 }
};
        public LoginController(ApplicationDbContext context)
        {

            _context = context;
        }

        public IActionResult LoginView()
        {
            var vm = new LoginViewModel
            {
                GeneratedCaptcha = GenerateCaptcha()
            };
            ViewBag.RoleList = _roles.Keys.ToList();
            HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
            return View(vm);
        }
        //===================== Login Authentication ======================//

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            try
            {
                
                var storedCaptcha = HttpContext.Session.GetString("Captcha");
                if (vm.CaptchaCode != storedCaptcha)
                {
                    ModelState.AddModelError("", "Invalid Captcha");
                    vm.GeneratedCaptcha = GenerateCaptcha();
                    HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
                    ViewBag.RoleList = _roles.Keys.ToList();
                    return View("LoginView", vm);
                }

                if (string.IsNullOrWhiteSpace(vm.Role) ||
                    string.IsNullOrWhiteSpace(vm.Email) ||
                    string.IsNullOrWhiteSpace(vm.Password))
                {
                    ModelState.AddModelError("", "All fields are required.");
                    vm.GeneratedCaptcha = GenerateCaptcha();
                    HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
                    ViewBag.RoleList = _roles.Keys.ToList();
                    return View("LoginView", vm);
                }

                // Get RoleId from the dictionary
                if (!_roles.TryGetValue(vm.Role, out int roleId))
                {
                    ModelState.AddModelError("", "Invalid Role selected.");
                    vm.GeneratedCaptcha = GenerateCaptcha();
                    HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
                    ViewBag.RoleList = _roles.Keys.ToList();
                    return View("LoginView", vm);
                }

                var user = _context.LoginDetails.FirstOrDefault(u =>
                    u.RoleId == roleId &&
                    u.Email.ToLower() == vm.Email.ToLower() &&
                    u.Password == vm.Password &&
                    u.IsActive);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid login credentials.");
                    vm.GeneratedCaptcha = GenerateCaptcha();
                    HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
                    ViewBag.RoleList = _roles.Keys.ToList();
                    return View("LoginView", vm);
                }

                TempData["Message"] = "Login successful!";
                await SaveLogs(user);
             

                // ✅ Role-based redirection
                return vm.Role switch
                {
                    "GIS Lab" => RedirectToAction("Index", "GISLab"),
                    "Service of India" => RedirectToAction("Index", "SOILogin"),
                    "Revenue Department" => RedirectToAction("RevDepartmentView", "RevDepartment"),
                    _ => RedirectToAction("LoginView")
                };

            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Exception : " + ex.ToString();
                return RedirectToAction("LoginView", vm);
            }
        }

        //===================== Save Login Logs Details  ======================//
        [HttpGet]
        public async Task SaveLogs(LoginDetail user)
        {
            var log = new LoginLog
            {
                UserName = user.Email,
                RoleId = user.RoleId,
                DistrictId = user.DistrictId,
                LoginTime = DateTime.Now,
                IPAddress = GetSystemIpAddress()
            };

            _context.LoginLogs.Add(log);
            await _context.SaveChangesAsync();

            SaveSession(log);

        }
        private void SaveSession(LoginLog log)
        {
            var userJson = JsonSerializer.Serialize(log);
            HttpContext.Session.SetString("LoginUser", userJson);
        }

        //===================== Refresh and generate Captcha ======================//
        [HttpGet]
        public IActionResult RefreshCaptcha()
        {
            var newCaptcha = GenerateCaptcha();
            HttpContext.Session.SetString("Captcha", newCaptcha);
            return Content(newCaptcha);

        }
        private string GenerateCaptcha()
        {
            var random = new Random();
            const string chars = "ABCDEFGHJKMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //===================== Get IP Address ======================//
        public string GetSystemIpAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = host.AddressList
                                     .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                return ipAddress?.ToString() ?? throw new Exception("No IPv4 address found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving system IP: {ex.Message}");
            }
        }

        //===================== Access Denied  ======================//
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
