using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Models.Api
{
    /// <summary>Branch info exposed by the API.</summary>
    public class BranchApiModel
    {
        /// <summary>Branch identifier.</summary>
        public int Id { get; set; }
        /// <summary>English display name.</summary>
        public string NameEnglish { get; set; }
        /// <summary>Arabic display name.</summary>
        public string NameArabic { get; set; }
        /// <summary>Whether the branch is active.</summary>
        public bool IsActive { get; set; }
    }
}
