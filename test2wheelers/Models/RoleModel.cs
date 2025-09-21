using test2wheelers.Models;

namespace test2wheelers.Models
{
    public class RoleModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public bool IsActive { get; set; }
        public List<RoleMenuPermissionModel> Permissions { get; set; } = new List<RoleMenuPermissionModel>();
    }
}
