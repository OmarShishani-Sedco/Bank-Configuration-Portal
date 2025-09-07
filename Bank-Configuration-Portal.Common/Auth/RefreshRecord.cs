using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Common.Auth
{
    public class RefreshRecord
    {
        public string UserName { get; set; }
        public int BankId { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? UsedAt { get; set; } 
    }
}
