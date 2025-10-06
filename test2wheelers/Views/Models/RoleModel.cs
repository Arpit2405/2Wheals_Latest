using _2whealers.Models;

namespace _2whealers.Models
{
    public class RoleModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public bool IsActive { get; set; }
        public List<RoleMenuPermissionModel> Permissions { get; set; } = new List<RoleMenuPermissionModel>();
    }
}
