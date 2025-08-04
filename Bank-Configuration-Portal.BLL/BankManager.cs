using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public BankModel GetByName(string name)
        {
            try
            {
                return _bankDAL.GetByName(name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
            
        }
        public bool IsUserMappedToBank(string username, int bankId)
        {
            try
            {
                return _bankDAL.BankUserMappingExists(username, bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

    }
}
