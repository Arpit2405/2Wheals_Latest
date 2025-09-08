using test2wheelers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace test2wheelers.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        // Return flat list of MenuItem parsed from claims
        public static List<MenuItem> GetFlatMenus(this ClaimsPrincipal user)
        {
            var list = new List<MenuItem>();

            var menuClaims = user.Claims.Where(c => c.Type == "Menu");
            foreach (var c in menuClaims)
            {
                var parts = c.Value.Split('|');
                // expected: 8 parts
                if (parts.Length < 8) continue;

                if (!int.TryParse(parts[0], out int menuId)) continue;
                var menuName = parts[1];
                var url = string.IsNullOrEmpty(parts[2]) ? null : parts[2];
                var icon = string.IsNullOrEmpty(parts[4]) ? null : parts[4];
                int? parent = null;
                if (int.TryParse(parts[3], out int p)) parent = p;

                bool canCreate = parts[5] == "1";
                bool canEdit = parts[6] == "1";
                bool canDelete = parts[7] == "1";
                bool canView = parts[8] == "1";

                list.Add(new MenuItem
                {
                    MenuId = menuId,
                    MenuName = menuName,
                    Url = url,
                    ParentMenuId = parent,
                    Icon = icon,
                    CanCreate = canCreate,
                    CanEdit = canEdit,
                    CanDelete = canDelete,
                    CanView = canView
                });
            }

            return list;
        }

        // Build a tree and return root nodes (ParentMenuId == null)
        public static List<MenuItem> GetMenuTree(this ClaimsPrincipal user)
        {
            var flat = user.GetFlatMenus();
            var lookup = flat.ToLookup(m => m.ParentMenuId);
            foreach (var node in flat)
            {
                node.Children = lookup[node.MenuId].OrderBy(m => m.MenuName).ToList();
            }

            var roots = lookup[null].OrderBy(m => m.SortOrder).ToList();
            return roots;
        }

        // Permission checks by menuId (or by URL)
        public static bool HasPermission(this ClaimsPrincipal user, int menuId, string action)
        {
            var item = user.GetFlatMenus().FirstOrDefault(m => m.MenuId == menuId);
            return item switch
            {
                null => false,
                _ when action == "Create" => item.CanCreate,
                _ when action == "Edit" => item.CanEdit,
                _ when action == "Delete" => item.CanDelete,
                _ when action == "View" => item.CanView,
                _ => false
            };
        }

        public static bool HasPermissionForUrl(this ClaimsPrincipal user, string url, string action)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var item = user.GetFlatMenus().FirstOrDefault(m => string.Equals(m.Url, url, StringComparison.OrdinalIgnoreCase));
            if (item == null) return false;
            return user.HasPermission(item.MenuId, action);
        }
    }
}
