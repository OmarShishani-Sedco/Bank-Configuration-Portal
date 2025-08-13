using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using System;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL
{
    public class BankManager : IBankManager
    {
        private readonly IBankDAL _bankDAL;

        public BankManager(IBankDAL bankDAL)
        {
            _bankDAL = bankDAL;
        }

        public async Task<BankModel?> GetByNameAsync(string name)
        {
            try
            {
                return await _bankDAL.GetByNameAsync(name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task<bool> IsUserMappedToBankAsync(string username, int bankId)
        {
            try
            {
                return await _bankDAL.BankUserMappingExistsAsync(username, bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
        public async Task<bool> BankExistsAsync(int bankId)
        {
            try
            { 
                return await _bankDAL.BankExistsAsync(bankId); 
            }
            catch (Exception ex) 
            { 
                Logger.LogError(ex); throw;
            }
        }
    }
}
