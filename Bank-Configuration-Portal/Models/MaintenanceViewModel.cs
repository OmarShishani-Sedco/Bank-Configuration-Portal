using Bank_Configuration_Portal.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class MaintenanceViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "UserName_Required")]
        [MaxLength(100)]
        [Display(ResourceType = typeof(Language), Name = "Maint_UserName_Label")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Maint_BankId_Required")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Maint_BankId_Range")]
        [Display(ResourceType = typeof(Language), Name = "Maint_BankId_Label")]
        public int? BankId { get; set; }

        public string Secret { get; set; }
    }
}