using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Api;
using Bank_Configuration_Portal.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Api
{
    public class TicketingDesignManager : ITicketingDesignManager
    {
        private readonly ITicketingDesignDAL _screenDAL;

        public TicketingDesignManager(ITicketingDesignDAL screenDAL)
        {
            _screenDAL = screenDAL;
        }

        public async Task<(TicketingDesignModel screen, int status)> GetActiveScreenButtonsForBranchAsync(int bankId, int? branchId, bool onlyAllocated = false)
        {
            try
            {
                return await _screenDAL.GetActiveScreenButtonsForBranchAsync(bankId, branchId, onlyAllocated);
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteError(ex, "[ScreenManager.GetActiveScreenButtonsForBranchAsync]");
                throw;
            }
        }
    }
}
