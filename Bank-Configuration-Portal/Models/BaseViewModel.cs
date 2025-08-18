using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class BaseViewModel
    {
        public bool IsLoggedIn { get; set; }
        public bool MustChangePassword { get; set; }
        public string UserName { get; set; }
        public string bankName { get; set; }
        public string LocalizedBankName { get; set; }
        public bool ShowBankInfo { get; set; }
        public string TargetUrl { get; set; }
    }
}