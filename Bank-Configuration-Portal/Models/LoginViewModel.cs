using Bank_Configuration_Portal.Resources;
using System.ComponentModel.DataAnnotations;

namespace Bank_Configuration_Portal.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Login_BankName_Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "MaxLength_100")]
        [Display(ResourceType = typeof(Language), Name = "Login_BankName_Label")]
        public string BankName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "UserName_Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "MaxLength_100")]
        [Display(ResourceType = typeof(Language), Name = "Login_UserName_Label")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Login_Password_Required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Password_Min_Length")]
        [Display(ResourceType = typeof(Language), Name = "Login_Password_Label")]
        public string Password { get; set; }
    }
}
