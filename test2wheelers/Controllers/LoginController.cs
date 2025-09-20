using _2whealers.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Claims;
using test2wheelers.Helpers;
using test2wheelers.Models;

namespace test2wheelers.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly SqlHelper _db;
        private readonly IWebHostEnvironment _env;

        public LoginController(ILogger<LoginController> logger, SqlHelper db, IWebHostEnvironment env)
        {
            _logger = logger;
            _db = db;
            _env = env;
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
                    new SqlParameter("@UserName", model.UserName)
                };

                DataSet ds = _db.ExecuteStoredProcedureWithDataSet("sp_LoginUser", parameters);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Invalid Username or Password" });
                }


                bool isValid = PasswordHelper.VerifyPassword(model.Password, ds.Tables[0].Rows[0]["PasswordHash"].ToString());
                //if (isValid == false)
                //{
                //    return Json(new { success = false, message = "Invalid Username or Password" });
                //}

                var row = ds.Tables[0].Rows[0];
                var menuTable = ds.Tables[1];

                // Create claims
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, row["Id"].ToString()),
                        new Claim(ClaimTypes.Name, row["Name"].ToString()),
                        new Claim(ClaimTypes.Thumbprint, row["ProfileImage"].ToString()),
                        new Claim("RegionLogo", row["RegionLogo"].ToString()),
                        new Claim("RegionName", row["RegionName"].ToString()),
                        new Claim(ClaimTypes.Role, row["RoleId"].ToString())
                    };

                foreach (DataRow m in   menuTable.Rows)
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
            UserModel model = new UserModel();
            var dt = _db.ExecuteStoredProcedure("sp_AspNetUsers", new[] {
                    new SqlParameter("@Id", User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    new SqlParameter("@CallType", "GetById")
                });

            if (dt.Rows.Count > 0)
            {
                model.Id = Guid.Parse(dt.Rows[0]["Id"].ToString());
                model.UserName = dt.Rows[0]["UserName"].ToString();
                model.Email = dt.Rows[0]["Email"].ToString();
                model.PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                model.PasswordHash = dt.Rows[0]["PasswordHash"].ToString();
                model.FirstName = dt.Rows[0]["FirstName"].ToString();
                model.LastName = dt.Rows[0]["LastName"].ToString();
                model.ProfileImage = dt.Rows[0]["ProfileImage"].ToString();
                model.RoleId = dt.Rows[0]["RoleId"] != DBNull.Value ? (int?)dt.Rows[0]["RoleId"] : null;
                model.RoleName = dt.Rows[0]["RoleName"].ToString();
                model.IsActive = (bool)dt.Rows[0]["IsActive"];
                model.Approved = (bool)dt.Rows[0]["Approved"];
            }
            return View(model);


        }

        [HttpPost]
        public IActionResult Profile(UserModel model)
        {
            //bool isValid = PasswordHelper.VerifyPassword(loginModel.Password, userFromDb.HashedPassword);
            string hashedPassword = PasswordHelper.HashPassword(model.Password);
            // Only check username for now
            var dt = _db.ExecuteStoredProcedureWithDataSet("sp_AspNetUsers", new[]
            {
                new SqlParameter("@Id", User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                new SqlParameter("@UserName", model.UserName),
                new SqlParameter("@FirstName", model.FirstName),
                new SqlParameter("@LastName", model.LastName),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@PhoneNumber", model.PhoneNumber),
                new SqlParameter("@Password", hashedPassword),
                new SqlParameter("@IsActive", model.IsActive),
                new SqlParameter("@Approved", model.Approved),
                new SqlParameter("@CallType", "UpdateProfile")
            });

            TempData["Success"] = "User created successfully!";

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(IFormFile photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                // Ensure "Uploads/Photos" folder exists inside wwwroot
                var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Photos");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var fileName = Path.GetFileName(photoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }


                var dt = _db.ExecuteStoredProcedureWithDataSet("sp_AspNetUsers", new[]
                {
                    new SqlParameter("@Id", User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    new SqlParameter("@ProfileImage", fileName),
                    new SqlParameter("@CallType", "UpdatePhoto")
                });

                // TODO: Save relative path to DB (e.g., "/Uploads/Photos/filename.jpg")
                TempData["Success"] = "Profile Pic uploaded successfully!";
            }
            else
            {
                TempData["Danger"] = "Please select a valid file.";
            }

            return RedirectToAction("Profile");
        }
    }
}
