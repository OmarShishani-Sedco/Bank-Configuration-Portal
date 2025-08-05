using Bank_Configuration_Portal.Models.Models;
using Bank_Configuration_Portal.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class CounterViewModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "NameEnglish_Required")]
        [Display(Name = "NameEnglish_Label", ResourceType = typeof(Language))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "NameEnglish_MaxLimit")]
        [RegularExpression("^[a-zA-Z0-9\\s]*$", ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "NameEnglish_Invalid")]
        public string NameEnglish { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "NameArabic_Required")]
        [Display(Name = "NameArabic_Label", ResourceType = typeof(Language))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "NameEnglish_MaxLimit")]
        [RegularExpression("^[\\u0600-\\u06FF\\u0750-\\u077F\\u08A0-\\u08FF\\s\\d\\p{P}]*$", ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "NameArabic_Invalid")]
        public string NameArabic { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Type")]
        [Required]
        public CounterType Type { get; set; }

        public byte[] RowVersion { get; set; }
    }

}