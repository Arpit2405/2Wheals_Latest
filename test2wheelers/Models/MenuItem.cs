using System.Data.SqlClient;

namespace _2whealers.Models
{
    public class MenuItem
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public int? ParentMenuId { get; set; }

        public string Icon { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }

        public int SortOrder { get; set; }

        public string Action { get; set; }

        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }
    public class MenuUpdateModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
