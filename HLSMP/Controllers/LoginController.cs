using HLSMP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Net;
using HLSMP.ViewModel;
using HLSMP.Models;

namespace HLSMP.Controllers
{

    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly List<string> _districts = new()
    {
        "Ambala", "Bhiwani", "Charkhi Dadri", "Faridabad", "Fatehabad", "Gurugram",
        "Hisar", "Jhajjar", "Jind", "Kaithal", "Karnal", "Kurukshetra", "Mahendragarh",
        "Nuh", "Palwal", "Panchkula", "Panipat", "Rewari", "Rohtak", "Sirsa",
        "Sonipat", "Yamunanagar"
    };

        public LoginController(ApplicationDbContext context)
        {

            _context = context;
        }

        //public IActionResult Index()
        //{
        //    ViewBag.Districts = _districts;
        //    return View();
        //}

        public IActionResult LoginView()
        {
            var vm = new LoginViewModel
            {
                GeneratedCaptcha = GenerateCaptcha()
            };
            ViewBag.Districts = _districts;
            HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
        {
            try
            {
                var storedCaptcha = HttpContext.Session.GetString("Captcha");
                if (vm.CaptchaCode != storedCaptcha)
                {
                    ModelState.AddModelError("", "Invalid Captcha");
                    vm.GeneratedCaptcha = GenerateCaptcha();
                    HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
                    ViewBag.Districts = _districts;
                    return View("LoginView", vm);
                }

                if (string.IsNullOrWhiteSpace(vm.District) ||
                    string.IsNullOrWhiteSpace(vm.Email) ||
                    string.IsNullOrWhiteSpace(vm.Password))
                {
                    ModelState.AddModelError("", "All fields are required.");
                    vm.GeneratedCaptcha = GenerateCaptcha();
                    HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
                    ViewBag.Districts = _districts;
                    return View("LoginView", vm);
                }

                var user = _context.LoginDetails.FirstOrDefault(u =>
                             u.District.ToLower() == vm.District.ToLower() &&
                             u.Email.ToLower() == vm.Email.ToLower() &&
                             u.Password == vm.Password &&
                             u.IsActive);


                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid login credentials.");
                    vm.GeneratedCaptcha = GenerateCaptcha();
                    HttpContext.Session.SetString("Captcha", vm.GeneratedCaptcha);
                    ViewBag.Districts = _districts;
                    return View("LoginView", vm);
                }

                TempData["Message"] = "Login successful!";
                SaveLogs(vm);

                return RedirectToAction("Checklist", "DistrictLogin");
            }
            catch (Exception ex)
            {
                return View("LoginView", vm);
            }
        }

        public async Task SaveLogs(LoginViewModel vm)
        {
            var log = new LoginLog
            {
                UserName = vm.Email,
                District = vm.District,
                LoginTime = DateTime.Now,
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _context.LoginLogs.Add(log);
            await _context.SaveChangesAsync();
        }
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



        public string GetSystemIpAddress()
        {
            try
            {
                // Get all ip address of the machine
                var host = Dns.GetHostEntry(Dns.GetHostName());

                // Filter out only IPv4 addresses (excluding loopback addresses like 127.0.0.1)
                var ipAddress = host.AddressList
                                     .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                // Return the first valid IPv4 address found
                return ipAddress?.ToString() ?? throw new Exception("No IPv4 address found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving system IP: {ex.Message}");
            }
        }
    }
}
