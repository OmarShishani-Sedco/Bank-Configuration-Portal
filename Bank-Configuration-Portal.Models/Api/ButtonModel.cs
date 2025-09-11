using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models.Api
{
    /// <summary>A button on the ticketing screen.</summary>
    public class ButtonModel
    {
        /// <summary>Button identifier.</summary>
        public int ButtonId { get; set; }
        /// <summary>Owning screen identifier.</summary>
        public int ScreenId { get; set; }
        /// <summary>Owning screen name.</summary>
        public string ScreenName { get; set; }
        /// <summary>Button type (ShowMessage or IssueTicket).</summary>
         [JsonConverter(typeof(StringEnumConverter))]
        public ButtonType ButtonType { get; set; }
        /// <summary>Service id (for IssueTicket); null for ShowMessage.</summary>
        public int? ServiceId { get; set; }
        /// <summary>English name.</summary>
        public string NameEnglish { get; set; }
        /// <summary>Arabic name.</summary>
        public string NameArabic { get; set; }
        /// <summary>Message in English (ShowMessage only).</summary>
        public string MessageEnglish { get; set; }
        /// <summary>Message in Arabic (ShowMessage only).</summary>
        public string MessageArabic { get; set; }
    }
    /// <summary>Kinds of ticketing buttons.</summary>
    public enum ButtonType
    {
        ShowMessage = 0,
        IssueTicket = 1
    }
}

   
