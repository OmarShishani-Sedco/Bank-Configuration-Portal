using Bank_Configuration_Portal.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class BranchViewModel
    {
        public int Id { get; set; }
        public int BankId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Branch_NameEnglish_Required")]
        [Display(Name = "Branch_NameEnglish_Label", ResourceType = typeof(Language))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Branch_NameEnglish_MaxLimit") ]
        [RegularExpression("^[a-zA-Z0-9\\s]*$", ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Branch_NameEnglish_Invalid")]
        public string NameEnglish { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Branch_NameArabic_Required")]
        [Display(Name = "Branch_NameArabic_Label", ResourceType = typeof(Language))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Branch_NameArabic_MaxLimit")]
        [RegularExpression("^[\\u0600-\\u06FF\\u0750-\\u077F\\u08A0-\\u08FF\\s]*$", ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "Branch_NameArabic_Invalid")]
        public string NameArabic { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(Language))]
        public bool IsActive { get; set; }
        public byte[] RowVersion { get; set; }
    }
}