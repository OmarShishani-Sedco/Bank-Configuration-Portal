using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models.Api
{
    public  class TicketingDesignModel
    {
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public IList<ButtonModel> Buttons { get; set; }
    }
}