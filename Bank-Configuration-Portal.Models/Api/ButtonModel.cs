using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models.Api
{
    public  class ButtonModel
    {
        public int ButtonId { get; set; }
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }  
        public ButtonType ButtonType { get; set; }     
        public int? ServiceId { get; set; }

        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public string MessageEnglish { get; set; }
        public string MessageArabic { get; set; }
    }
    public enum ButtonType
    {
        ShowMessage = 0,
        IssueTicket = 1
    }
}

   
