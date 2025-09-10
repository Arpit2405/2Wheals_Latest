namespace test2wheelers.Models
{
    public class RoleMenuPermissionModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int? ParentMenuId { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }

        public bool CanView { get; set; } = false;
        public bool CanCreate { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;

        public List<RoleMenuPermissionModel> Children { get; set; } = new List<RoleMenuPermissionModel>();
    }
}
