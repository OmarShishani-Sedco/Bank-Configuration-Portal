using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models.Api
{
    /// <summary>Active screen and its buttons.</summary>
    public class TicketingDesignModel
    {
        /// <summary>Screen identifier.</summary>
        public int ScreenId { get; set; }
        /// <summary>Screen display name.</summary>
        public string ScreenName { get; set; }
        /// <summary>Buttons belonging to this screen.</summary>
        public IList<ButtonModel> Buttons { get; set; }
    }
}