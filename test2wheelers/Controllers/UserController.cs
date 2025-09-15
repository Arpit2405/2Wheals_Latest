using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using test2wheelers.Helpers;
using test2wheelers.Models;

namespace test2wheelers.Controllers
{
    public class UserController : Controller
    {
        private readonly SqlHelper _db;

        public UserController(SqlHelper db)
        {
            _db = db;
        }

        // GET: Users List
        public IActionResult List()
        {
            var dt = _db.ExecuteStoredProcedure("sp_AspNetUsers", new[] {
                new SqlParameter("@CallType", "GetAll")
            });
            var users = dt.AsEnumerable().Select(r => new UserModel
            {
                Id = r.Field<Guid>("Id"),
                UserName = r.Field<string>("UserName"),
                RoleName = r.Field<string>("RoleName"),
                IsActive = r.Field<bool>("IsActive"),
                RoleId = null // Only if needed for listing
            }).ToList();

            return View(users);
        }

        // GET: Create/Edit
        public IActionResult Manage(Guid? id)
        {
            UserModel model = new UserModel();

            if (id.HasValue)
            {
                var dt = _db.ExecuteStoredProcedure("sp_AspNetUsers", new[] {
                    new SqlParameter("@Id", id.Value),
                    new SqlParameter("@CallType", "GetById")
                });

                if (dt.Rows.Count > 0)
                {
                    model.Id = Guid.Parse(dt.Rows[0]["Id"].ToString());
                    model.UserName = dt.Rows[0]["UserName"].ToString();
                    model.Email = dt.Rows[0]["Email"].ToString();
                    model.PhoneNumber = dt.Rows[0]["PhoneNumber"].ToString();
                    model.PasswordHash = dt.Rows[0]["PasswordHash"].ToString();
                    model.RoleId = dt.Rows[0]["RoleId"] != DBNull.Value ? (int?)dt.Rows[0]["RoleId"] : null;
                    model.IsActive = (bool)dt.Rows[0]["IsActive"];
                    model.Approved = (bool)dt.Rows[0]["Approved"];
                }
            }

            var roles = _db.ExecuteStoredProcedure("sp_Role", new[] {
                    new SqlParameter("@RoleId", model.RoleId),
                    new SqlParameter("@CallType", "GetAll")
                });

            ViewBag.Roles = roles.AsEnumerable()
                .Select(r => new SelectListItem
                {
                    Value = r["RoleId"].ToString(),
                    Text = r["RoleName"].ToString()
                })
                .ToList();
            return View(model);
        }

        // POST: Save User
        [HttpPost]
        public IActionResult Manage(UserModel model)
        {
            //if (!ModelState.IsValid) return View(model);

            if (model.Id == null) // Create
            {
                Guid Id = Guid.NewGuid();
                var sql = _db.ExecuteStoredProcedure("sp_AspNetUsers", new[] {
                    new SqlParameter("@Id", Id),
                    new SqlParameter("@UserName",  model.UserName),
                    new SqlParameter("@Email",  model.Email),
                    new SqlParameter("@PhoneNumber",  model.PhoneNumber),
                    new SqlParameter("@Password",  model.PasswordHash),
                    new SqlParameter("@IsActive",  model.IsActive),
                    new SqlParameter("@Approved",  model.Approved),
                    new SqlParameter("@RoleId",  model.RoleId),
                    new SqlParameter("@CallType",  "Insert")
                });
                TempData["Success"] = "User created successfully!";
            }
            else // Update
            {

                var sql = _db.ExecuteStoredProcedure("sp_AspNetUsers", new[] {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@UserName",  model.UserName),
                    new SqlParameter("@Email",  model.Email),
                    new SqlParameter("@PhoneNumber",  model.PhoneNumber),
                    new SqlParameter("@Password",  model.PasswordHash),
                    new SqlParameter("@IsActive",  model.IsActive),
                    new SqlParameter("@Approved",  model.Approved),
                    new SqlParameter("@RoleId",  model.RoleId),
                    new SqlParameter("@CallType",  "Update")
                });
                TempData["Success"] = "User updated successfully!";
            }

            return RedirectToAction("List");
        }

     
    }
}