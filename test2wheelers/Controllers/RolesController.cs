using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using _2whealers.Helpers;
using _2whealers.Models;

namespace _2whealers.Controllers
{
    public class RolesController : Controller
    {
        private readonly SqlHelper _db;

        public RolesController(SqlHelper db)
        {
            _db = db;
        }

        public IActionResult List()
        {
            var dt = _db.ExecuteStoredProcedure("sp_Role", new[] {
                new SqlParameter("@CallType", "GetAll")
            });
            var roles = dt.AsEnumerable().Select(r => new RoleModel
            {
                RoleId = (int)r["RoleId"],
                RoleName = r["RoleName"].ToString(),
                IsActive = (bool)r["IsActive"]
            }).ToList();

            return View(roles);
        }


        public IActionResult Manage(int? id)
        {
            // Get all menus from DB
            var menus = _db.ExecuteStoredProcedure("sp_Menu", new[] {
                new SqlParameter("@CallType", "GetAll")
            });

            var flatList = menus.AsEnumerable().Select(m => new RoleMenuPermissionModel
            {
                MenuId = (int)m["MenuId"],
                MenuName = m["MenuName"].ToString(),
                ParentMenuId = m["ParentId"] == DBNull.Value ? null : (int?)m["ParentId"]
            }).ToList();

            var model = new RoleModel
            {
                RoleId = id ?? 0,
                Permissions = BuildMenuTree(flatList, null)
            };

            if (id.HasValue && id.Value > 0)
            {
                // Editing existing role
                var dtRole = _db.ExecuteStoredProcedure("sp_Role", new[] {
                    new SqlParameter("@RoleId", id.Value),
                    new SqlParameter("@CallType", "GetByRoleId")
                });

                if (dtRole.Rows.Count == 0)
                    return NotFound();

                model.RoleName = dtRole.Rows[0]["RoleName"].ToString();

                // Get existing permissions
                var dtPerms = _db.ExecuteStoredProcedure("sp_RolePermissions", new[] {
                    new SqlParameter("@RoleId", id.Value),
                    new SqlParameter("@CallType", "GetByRoleId")
                });

                ApplyPermissions(model.Permissions, dtPerms);  // <-- fill tree with flags
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Manage(RoleModel model)
        {
            if (model.RoleId == 0)
            {

                var roleIdParam = new SqlParameter("@OutputRoleId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var dt = _db.ExecuteStoredProcedure("sp_Role", new[] {
                    roleIdParam,
                    new SqlParameter("@RoleName", model.RoleName),
                    new SqlParameter("@CallType", "Insert")
                });

                int roleId = (int)roleIdParam.Value;

                SavePermissions(roleId, model.Permissions);

                TempData["Success"] = "Role saved successfully!";
            }
            else
            {
                // UPDATE
                _db.ExecuteStoredProcedure("sp_Role", new[] {
                    new SqlParameter("@RoleName", model.RoleName),
                    new SqlParameter("@RoleId", model.RoleId),
                     new SqlParameter("@CallType", "Update")
                });

                _db.ExecuteStoredProcedure("sp_RolePermissions", new[] {
                    new SqlParameter("@RoleId", model.RoleId),
                    new SqlParameter("@CallType", "Delete")
                });

                SavePermissions(model.RoleId, model.Permissions);

                TempData["Success"] = "Role updated successfully!";
            }
            
            //return RedirectToAction("Manage");
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _db.ExecuteStoredProcedure("sp_Role", new[] {
                    new SqlParameter("@RoleId", id),
                    new SqlParameter("@CallType", "Delete")
                });

            TempData["Success"] = "Role deleted successfully!";

            return RedirectToAction("List");
        }


        [HttpPost]
        public IActionResult Create(RoleModel model)
        {
            var roleId = (int)_db.ExecuteScalar(@"
        INSERT INTO Roles (RoleName)
        OUTPUT INSERTED.RoleId
        VALUES (@RoleName)",
                new[]
                {
            new SqlParameter("@RoleName", model.RoleName)
                });

            SavePermissions(roleId, model.Permissions);

            return RedirectToAction("Index");
        }

        private void SavePermissions(int roleId, List<RoleMenuPermissionModel> permissions)
        {
            foreach (var perm in permissions)
            {
                if (perm.CanView || perm.CanCreate || perm.CanEdit || perm.CanDelete)
                {
                    var exists = (int)_db.ExecuteScalar(@"
                SELECT COUNT(*) FROM RolePermissions
                WHERE RoleId = @RoleId AND MenuId = @MenuId",
                        new[]
                        {
                    new SqlParameter("@RoleId", roleId),
                    new SqlParameter("@MenuId", perm.MenuId)
                        });

                    if (exists == 0)
                    {
                        _db.ExecuteNonQuery(@"
                    INSERT INTO RolePermissions
                    (RoleId, MenuId, CanView, CanCreate, CanEdit, CanDelete,CreatedOn)
                    VALUES (@RoleId, @MenuId, @CanView, @CanCreate, @CanEdit, @CanDelete, getdate())",
                            new[]
                            {
                        new SqlParameter("@RoleId", roleId),
                        new SqlParameter("@MenuId", perm.MenuId),
                        new SqlParameter("@CanView", perm.CanView),
                        new SqlParameter("@CanCreate", perm.CanCreate),
                        new SqlParameter("@CanEdit", perm.CanEdit),
                        new SqlParameter("@CanDelete", perm.CanDelete)
                            });
                    }
                }

                // recurse into children
                if (perm.Children != null && perm.Children.Count > 0)
                {
                    SavePermissions(roleId, perm.Children);
                }
            }
        }

        private void ApplyPermissions(List<RoleMenuPermissionModel> menus, DataTable dtPerms)
        {
            var lookup = dtPerms.AsEnumerable()
    .GroupBy(r => (int)r["MenuId"])
    .ToDictionary(g => g.Key, g => new
    {
        CanView = g.Any(r => (bool)r["CanView"]),
        CanCreate = g.Any(r => (bool)r["CanCreate"]),
        CanEdit = g.Any(r => (bool)r["CanEdit"]),
        CanDelete = g.Any(r => (bool)r["CanDelete"])
    });

            foreach (var menu in menus)
            {
                if (lookup.TryGetValue(menu.MenuId, out var perm))
                {
                    menu.CanView = perm.CanView;
                    menu.CanCreate = perm.CanCreate;
                    menu.CanEdit = perm.CanEdit;
                    menu.CanDelete = perm.CanDelete;
                }

                if (menu.Children?.Any() == true)
                {
                    ApplyPermissions(menu.Children, dtPerms);
                }
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
                    // initialize default permissions (unchecked)
                    CanView = false,
                    CanCreate = false,
                    CanEdit = false,
                    CanDelete = false,
                    Children = BuildMenuTree(flatList, x.MenuId)
                })
                .ToList();
        }
    }
}
