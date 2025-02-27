using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Linq;
using TestCaseBookShop.Models;
using TestCaseBookShop.Models.Data;
using TestCaseBookShop.Models.Data.Dto;
using TestCaseBookShop.Services;
using TestCaseBookShop.Util;
using static TestCaseBookShop.Services.TokenGenerateService;

namespace TestCaseBookShop.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly ITokenService _tokenService;

        public LoginController(ApplicationDbContext context, ITokenService tokenService, ILogger<LoginController> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromBody] UserDto user)
        {
            var validUser = _context.Users.FirstOrDefault(a => a.UserName == user.UserName);

            if (validUser == null)
            {
                return Json(new { success = false, message = "Kullanıcı adı veya şifre hatalı!" });
            }

            var decryptionPass = EncryptionDecryptionUtil.Decrypt(validUser.Password);

            if (decryptionPass != user.Password)
            {
                return Json(new { success = false, message = "Kullanıcı adı veya şifre hatalı!" });
            }

            var tokenResponse = _tokenService.CreateToken(validUser);

            _logger.LogInformation($"Kullanıcı giriş yaptı: {validUser.UserName}");

            return Json(new { success = true, message = "Giriş başarılı!", tokenResponse });
        }
    }
}