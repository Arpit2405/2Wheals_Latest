using _2whealers.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using test2wheelers.Helpers;
using test2wheelers.Models;

namespace test2wheelers.Controllers
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
                    ParentMenuId = row["ParentId"] == DBNull.Value ? null : (int?)row["ParentId"]
                });
            }

            var tree = BuildMenuTree(flatList, null);
            return View(tree);
        }

        [HttpPost]
        public IActionResult SaveMenu(MenuItem model)
        {
            if (model.MenuId == 0)
            {
                // Insert new
                var dt = _db.ExecuteStoredProcedure("sp_Menu", new[] {
                    new SqlParameter("@MenuName", model.MenuName),
                    new SqlParameter("@Url", model.Url),
                    new SqlParameter("@Icon", model.Icon),
                    new SqlParameter("@CallType", "Insert")
                });

                //if (dt.Rows.Count > 0)
                //{
                //    var row = dt.Rows[0];
                //    int result = Convert.ToInt32(row["Result"]);
                //    string message = row["Message"].ToString();

                //    if (result == -1)
                //        TempData["Error"] = message;
                //    else
                //        TempData["Success"] = message;
                //}


                return Json(new { menuId = 0, menuName = model.MenuName });
            }
            else
            {
                // Update existing
                var dt = _db.ExecuteStoredProcedure("sp_Menu", new[] {
                    new SqlParameter("@MenuId", model.MenuId),
                    new SqlParameter("@MenuName", model.MenuName),
                    new SqlParameter("@Url", model.Url),
                    new SqlParameter("@Icon", model.Icon),
                    new SqlParameter("@CallType", "Update")
                });

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    int result = Convert.ToInt32(row["Result"]);
                    string message = row["Message"].ToString();

                    if (result == -1)
                        TempData["Error"] = message;
                    else
                        TempData["Success"] = message;
                }

                return Json(new { menuId = model.MenuId, menuName = model.MenuName });
            }
        }

        [HttpPost]
        public IActionResult SaveTree([FromBody] List<MenuNodeDto> nodes)
        {
            SaveHierarchy(nodes, null);
            return Ok();
        }

        private void SaveHierarchy(List<MenuNodeDto> nodes, int? parentId)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                _db.ExecuteNonQuery(
                    "UPDATE Menus SET ParentId=@ParentId, SortOrder=@SortOrder WHERE MenuId=@MenuId",
                    new[]
                    {
                    new SqlParameter("@ParentId", (object?)parentId ?? DBNull.Value),
                    new SqlParameter("@SortOrder", i),
                    new SqlParameter("@MenuId", node.MenuId)
                    }
                );

                if (node.Children != null && node.Children.Any())
                    SaveHierarchy(node.Children, node.MenuId);
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
                    // initialize default permissions (unchecked)
                    CanView = false,
                    CanCreate = false,
                    CanEdit = false,
                    CanDelete = false,
                    Children = BuildMenuTree(flatList, x.MenuId)
                })
                .ToList();
        }

        [HttpPost]
        public IActionResult Edit([FromBody] MenuUpdateModel model)
        {
            if (model == null) return BadRequest();

            _db.ExecuteNonQuery(@"
        UPDATE Menus 
        SET MenuName = @MenuName, Url = @Url 
        WHERE MenuId = @MenuId", new[]
                    {
                new SqlParameter("@MenuId", model.MenuId),
                new SqlParameter("@MenuName", model.MenuName),
                new SqlParameter("@Url", (object)model.Url ?? DBNull.Value)
            }
                );

            return Ok(new { success = true });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                string sql = "DELETE FROM Menus WHERE MenuId = @p0";
                _db.ExecuteNonQuery(sql, new[] { new SqlParameter("@p0", id) });
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
