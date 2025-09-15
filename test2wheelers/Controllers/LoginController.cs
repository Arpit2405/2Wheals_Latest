using Microsoft.AspNetCore.Mvc; 
using System.Data;
using test2wheelers.Models;
using _2whealers.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data.SqlClient;
using test2wheelers.Helpers;

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

                DataSet ds = _db.ExecuteStoredProcedureWithDataSet("sp_LoginUser", parameters);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Invalid Username or Password" });
                }

                var row = ds.Tables[0].Rows[0];
                var menuTable = ds.Tables[1];

                // Create claims
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, row["Id"].ToString()),
                        new Claim(ClaimTypes.Name, row["UserName"].ToString()),
                        new Claim(ClaimTypes.Role, row["RoleId"].ToString())
                    };

                foreach (DataRow m in menuTable.Rows)
                {
                    var menuId = m["MenuId"].ToString();
                    var menuName = m["MenuName"].ToString();
                    var url = m["Url"] == DBNull.Value ? "" : m["Url"].ToString();
                    var parent = m["ParentId"] == DBNull.Value ? "" : m["ParentId"].ToString();
                    var icon = m["Icon"] == DBNull.Value ? "" : m["Icon"].ToString();
                    var canCreate = Convert.ToInt32(m["CanCreate"]) == 1 ? "1" : "0";
                    var canEdit = Convert.ToInt32(m["CanEdit"]) == 1 ? "1" : "0";
                    var canDelete = Convert.ToInt32(m["CanDelete"]) == 1 ? "1" : "0";
                    var canView = Convert.ToInt32(m["CanView"]) == 1 ? "1" : "0";
                    var sort = Convert.ToInt32(m["SortOrder"]) == 1 ? "1" : "0";

                    string claimValue = string.Join("|", new[] { menuId, menuName, url, parent, icon, canCreate, canEdit, canDelete, canView, sort });
                    claims.Add(new Claim("Menu", claimValue));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
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

            return Json(new { success = false, message = "Invalid Request" });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

           public IActionResult Profile()
        {
            return View();
        }
    }
}
