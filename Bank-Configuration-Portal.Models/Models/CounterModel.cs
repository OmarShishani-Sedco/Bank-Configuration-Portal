using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Models.Models
{
    public class CounterModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string NameEnglish { get; set; }
        public string NameArabic { get; set; }
        public bool IsActive { get; set; }
        public CounterType Type { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
