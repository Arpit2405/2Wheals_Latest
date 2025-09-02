using Microsoft.AspNetCore.Mvc; 
using System.Data;
using test2wheelers.Models;
using _2whealers.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data.SqlClient;

namespace test2wheelers.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly SqlHelper _db;

        public LoginController(ILogger<LoginController> logger, SqlHelper db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Index(Login model)
        {
            if (ModelState.IsValid)
            {
                if (model.CaptchaCode == null || model.CaptchaCode.ToUpper() != model.HiddenCaptcha.ToUpper())
                {
                    return Json(new { success = false, message = "Invalid Captcha" });
                }

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserName", model.UserName),
                    new SqlParameter("@Password", model.Password)
                };

                DataTable dt = _db.ExecuteStoredProcedure("sp_LoginUser", parameters);
                if (dt.Rows.Count > 0)
                {
                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, dt.Rows[0]["Id"].ToString()),
                        new Claim(ClaimTypes.Name, dt.Rows[0]["UserName"].ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                        });

                    return Json(new
                    {
                        success = true,
                        message = "Login Successful!",
                        redirectUrl = Url.Action("Dashboard", "Admin")
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid Username or Password" });
                }
            }

            return Json(new { success = false, message = "Invalid Request" });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
