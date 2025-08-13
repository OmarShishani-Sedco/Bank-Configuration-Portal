// Bank_Configuration_Portal.Models.Models/AppUserModel.cs
namespace Bank_Configuration_Portal.Models.Models
{
    public class AppUserModel
    {
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int Iterations { get; set; }
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
