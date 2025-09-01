using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Common.Auth
{
    public sealed class TokenPrincipal
    {
        public string UserName { get; set; }
        public int BankId { get; set; }
        public IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
