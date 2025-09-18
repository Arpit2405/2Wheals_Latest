using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using test2wheelers.Helpers;
using test2wheelers.Models;

namespace test2wheelers.Controllers
{
    public class RegionController : Controller
    {
        private readonly SqlHelper _db;
        private readonly IWebHostEnvironment _env;

        public RegionController(SqlHelper db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: Users List
        public IActionResult List()
        {
            var dt = _db.ExecuteStoredProcedure("sp_Region", new[] {
                new SqlParameter("@CallType", "GetAll")
            });
            var users = dt.AsEnumerable().Select(r => new RegionModel
            {
                Id = r.Field<int>("Id"),
                RegionName = r.Field<string>("RegionName"),
                AddressLine1 = r.Field<string>("AddressLine1"),
                AddressLine2 = r.Field<string>("AddressLine2"),
                MobileNo = r.Field<string>("MobileNo"),
                RegionLogo = r.Field<string>("RegionLogo"),
                IsActive = r.Field<bool>("IsActive")
            }).ToList();

            return View(users);
        }

        // GET: Create/Edit
        public IActionResult Manage(int? id)
        {
            RegionModel model = new RegionModel();

            if (id.HasValue)
            {
                var dt = _db.ExecuteStoredProcedure("sp_Region", new[] {
                    new SqlParameter("@Id", id.Value),
                    new SqlParameter("@CallType", "GetById")
                });

                if (dt.Rows.Count > 0)
                {
                    model.Id = Convert.ToInt32(dt.Rows[0]["Id"].ToString());
                    model.RegionName = dt.Rows[0]["RegionName"].ToString();
                    model.AddressLine1 = dt.Rows[0]["AddressLine1"].ToString();
                    model.AddressLine2 = dt.Rows[0]["AddressLine2"].ToString();
                    model.MobileNo = dt.Rows[0]["MobileNo"].ToString();
                    model.RegionLogo = dt.Rows[0]["RegionLogo"].ToString();
                    model.IsActive = (bool)dt.Rows[0]["IsActive"];
                }
            }

            
            return View(model);
        }

        [HttpPost]
        public IActionResult Manage(RegionModel model, IFormFile? RegionLogo)
        {
            //if (!ModelState.IsValid) return View(model);

            string uniqueFileName = "";
            if (RegionLogo != null && RegionLogo.Length > 0)
            {
                string uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "regions");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(RegionLogo.FileName);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    RegionLogo.CopyTo(stream);
                }

                // Save relative path in DB (e.g. /uploads/regions/xyz.png)
                model.RegionLogo = "/uploads/regions/" + uniqueFileName;
            }

            if (model.Id == 0) // Create
            {
                var sql = _db.ExecuteStoredProcedure("sp_Region", new[] {
                    new SqlParameter("@Id", 0),
                    new SqlParameter("@RegionName",  model.RegionName),
                    new SqlParameter("@AddressLine1",  model.AddressLine1),
                    new SqlParameter("@AddressLine2",  model.AddressLine2),
                    new SqlParameter("@MobileNo",   model.MobileNo),
                    new SqlParameter("@RegionLogo",  uniqueFileName),
                    new SqlParameter("@IsActive",  model.IsActive),
                    new SqlParameter("@CallType",  "Insert")
                });
                TempData["Success"] = "User created successfully!";
            }
            else // Update
            {

                if (uniqueFileName == "")
                {
                    uniqueFileName = model.RegionLogo;
                }
                    

                var sql = _db.ExecuteStoredProcedure("sp_Region", new[] {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@RegionName",  model.RegionName),
                    new SqlParameter("@AddressLine1",  model.AddressLine1),
                    new SqlParameter("@AddressLine2",  model.AddressLine2),
                    new SqlParameter("@MobileNo",   model.MobileNo),
                    new SqlParameter("@RegionLogo",  uniqueFileName),
                    new SqlParameter("@IsActive",  model.IsActive),
                    new SqlParameter("@CallType",  "Update")
                });
                TempData["Success"] = "User updated successfully!";
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _db.ExecuteStoredProcedure("sp_Region", new[] {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@CallType", "Delete")
                });

            TempData["Success"] = "Role deleted successfully!";

            return RedirectToAction("List");
        }
    }
}
