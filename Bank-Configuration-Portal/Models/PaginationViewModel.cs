using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int? BranchId { get; set; }

    }
}