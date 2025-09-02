using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Models.Api
{
    public class TokenRequestModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BankName { get; set; }
    }
}
