using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Models.Models
{
    public class ServiceModel
    {
        public int ServiceId { get; set; }
        public int BankId { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public bool IsActive { get; set; }
        public int MaxTicketsPerDay { get; set; }
    }

}
