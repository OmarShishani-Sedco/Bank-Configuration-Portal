using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class ServiceListViewModel
    {
        public List<ServiceViewModel> Services { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Filter properties
        public string SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}