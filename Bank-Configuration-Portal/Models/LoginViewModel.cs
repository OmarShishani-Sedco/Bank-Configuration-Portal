using Bank_Configuration_Portal.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "Login_BankName_Required", ErrorMessageResourceType = typeof(Language))]
        [MaxLength(100)]
        [Display(ResourceType = typeof(Language), Name = "Login_BankName_Label")]
        public string BankName { get; set; }

        [Required(ErrorMessageResourceName = "Login_UserName_Required", ErrorMessageResourceType = typeof(Language))]
        [MaxLength(100)]
        [Display(ResourceType = typeof(Language), Name = "Login_UserName_Label")]
        public string UserName { get; set; }


    }
}