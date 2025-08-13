using Bank_Configuration_Portal.Resources;
using System.ComponentModel.DataAnnotations;

namespace Bank_Configuration_Portal.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ChangePassword_Old_Required")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Language), Name = "ChangePassword_Old_Label")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ChangePassword_New_Required")]
        [MinLength(8, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Password_Min_Length")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Language), Name = "ChangePassword_New_Label")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ChangePassword_Confirm_Required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Passwords_Must_Match")]
        [Display(ResourceType = typeof(Language), Name = "ChangePassword_Confirm_Label")]
        public string ConfirmPassword { get; set; }
    }
}
