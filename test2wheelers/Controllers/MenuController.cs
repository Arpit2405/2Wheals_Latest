using _2whealers.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using _2whealers.Helpers;
using _2whealers.Models;

namespace _2whealers.Controllers
{
    public class MenuController : Controller
    {
        private readonly SqlHelper _db;
        public MenuController(SqlHelper db)
        {
            _db = db;
        }

        public IActionResult List()
        {
            List<RoleMenuPermissionModel> flatList = new List<RoleMenuPermissionModel>();

            var menus = _db.ExecuteStoredProcedure("sp_Menu", new[] {
                new SqlParameter("@CallType", "GetAll")
            });

            foreach (DataRow row in menus.Rows)
            {
                flatList.Add(new RoleMenuPermissionModel
                {
                    MenuId = (int)row["MenuId"],
                    MenuName = row["MenuName"].ToString(),
                    Url = row["Url"].ToString(),
                    Icon = row["Icon"].ToString(),
                    SortOrder = (int)row["SortOrder"],
                    ParentMenuId = row["ParentId"] == DBNull.Value ? null : (int?)row["ParentId"]
                });
            }

            var tree = BuildMenuTree(flatList, null);
            return View(tree);
        }

        [HttpPost]
        public IActionResult SaveMenu(MenuItem model)
        {
            if (model.Action == "InsertParent")
            {
                // Insert new
                var dt = _db.ExecuteStoredProcedure("sp_Menu", new[] {
                    new SqlParameter("@MenuName", model.MenuName),
                    new SqlParameter("@Url", model.Url),
                    new SqlParameter("@Icon", model.Icon),
                    new SqlParameter("@CallType", "InsertParent")
                });

                return Json(new { menuId = 0, menuName = model.MenuName });
            }
            else
            {
                // Insert new
                var dt = _db.ExecuteStoredProcedure("sp_Menu", new[] {
                    new SqlParameter("@MenuName", model.MenuName),
                    new SqlParameter("@MenuId", model.MenuId),
                    new SqlParameter("@ParentId", model.ParentMenuId),
                    new SqlParameter("@Url", model.Url),
                    new SqlParameter("@Icon", model.Icon),
                    new SqlParameter("@CallType", "InsertChild")
                });

                return Json(new { menuId = 0, menuName = model.MenuName });
            }    
        }


        private List<RoleMenuPermissionModel> BuildMenuTree(List<RoleMenuPermissionModel> flatList, int? parentId)
        {
            return flatList
                .Where(x => x.ParentMenuId == parentId)
                .Select(x => new RoleMenuPermissionModel
                {
                    MenuId = x.MenuId,
                    MenuName = x.MenuName,
                    ParentMenuId = x.ParentMenuId,
                    Url = x.Url,
                    Icon = x.Icon,
                    // initialize default permissions (unchecked)
                    CanView = false,
                    CanCreate = false,
                    CanEdit = false,
                    CanDelete = false,
                    Children = BuildMenuTree(flatList, x.MenuId)
                }).OrderBy(x => x.SortOrder)
                .ToList();
        }

        [HttpPost]
        public IActionResult Edit([FromBody] MenuUpdateModel model)
        {
            if (model == null) return BadRequest();

            _db.ExecuteStoredProcedure("sp_Menu", new[] {
                    new SqlParameter("@MenuName", model.MenuName),
                    new SqlParameter("@MenuId", model.MenuId),
                    new SqlParameter("@Url", (object)model.Url ?? DBNull.Value),
                    new SqlParameter("@Icon", model.Icon),
                    new SqlParameter("@CallType", "Update")
                });

            return Ok(new { success = true });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _db.ExecuteStoredProcedure("sp_Menu", new[] {
                    new SqlParameter("@MenuId", id),
                    new SqlParameter("@CallType", "Delete")
                });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class MenuNodeDto
    {
        public int MenuId { get; set; }
        public int Order { get; set; }
        public List<MenuNodeDto> Children { get; set; }
    }
}
