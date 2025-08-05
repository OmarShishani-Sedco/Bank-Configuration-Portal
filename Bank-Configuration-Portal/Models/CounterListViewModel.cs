using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.Models
{
    public class CounterListViewModel
    {
        public List<CounterViewModel> Counters { get; set; } = new List<CounterViewModel>();
        public int BranchId { get; set; }
        public string BranchNameArabic { get; set; }
        public string BranchNameEnglish { get; set; }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
    }

}