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
            return await _bankDAL.GetByNameAsync(name);
        }

        public async Task<bool> IsUserMappedToBankAsync(string username, int bankId)
        {

            return await _bankDAL.BankUserMappingExistsAsync(username, bankId);

        }
        public async Task<bool> BankExistsAsync(int bankId)
        {
            return await _bankDAL.BankExistsAsync(bankId);
        }
    }
}
