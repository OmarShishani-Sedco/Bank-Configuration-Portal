using Bank_Configuration_Portal.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Api
{
    public interface ITicketingDesignDAL
    {
        Task<TicketingDesignModel> GetActiveScreenButtonsForBranchAsync(int bankId, int branchId, bool onlyAllocated);
    }
}
