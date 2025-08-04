using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;

namespace Bank_Configuration_Portal.BLL
{
    public class BranchManager : IBranchManager
    {
        private readonly IBranchDAL _branchDAL;

        public BranchManager(IBranchDAL branchDAL)
        {
            _branchDAL = branchDAL;
        }

        public void Create(BranchModel branch)
        {
            try
            {
                _branchDAL.Create(branch);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public void Update(BranchModel branch, bool forceUpdate = false)
        {
            try
            {
                _branchDAL.Update(branch);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public void Delete(int id, int bankId, byte[] rowVersion)
        {
            try
            {
                _branchDAL.Delete(id, bankId, rowVersion);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public List<BranchModel> GetAllByBankId(int bankId)
        {
            try
            {
                return _branchDAL.GetAllByBankId(bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public BranchModel GetById(int id, int bankId)
        {
            try
            {
                return _branchDAL.GetById(id, bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
    }
}
