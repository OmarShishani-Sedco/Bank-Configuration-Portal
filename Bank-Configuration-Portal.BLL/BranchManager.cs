using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL
{
    public class BranchManager : IBranchManager
    {
        private readonly IBranchDAL _branchDAL;

        public BranchManager(IBranchDAL branchDAL)
        {
            _branchDAL = branchDAL;
        }

        public async Task CreateAsync(BranchModel branch)
        {
            try
            {
                await _branchDAL.CreateAsync(branch);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task UpdateAsync(BranchModel branch, bool forceUpdate = false)
        {
            try
            {
                await _branchDAL.UpdateAsync(branch);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task DeleteAsync(int id, int bankId, byte[] rowVersion)
        {
            try
            {
                await _branchDAL.DeleteAsync(id, bankId, rowVersion);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task<List<BranchModel>> GetAllByBankIdAsync(int bankId)
        {
            try
            {
                return await _branchDAL.GetAllByBankIdAsync(bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task<BranchModel> GetByIdAsync(int id, int bankId)
        {
            try
            {
                return await _branchDAL.GetByIdAsync(id, bankId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
    }

}
