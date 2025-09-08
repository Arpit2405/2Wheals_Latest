namespace test2wheelers.Models
{
    public class UserModel
    {
        public Guid? Id { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string PasswordHash { get; set; } // plain for create/edit
        public string PasswordSalt { get; set; }
        public string DisplayName { get; set; }
        public int? RoleId { get; set; }
        public bool Approved { get; set; }
        public bool IsActive { get; set; }
    }
}
