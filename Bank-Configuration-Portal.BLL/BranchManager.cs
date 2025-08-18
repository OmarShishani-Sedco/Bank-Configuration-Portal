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
                await _branchDAL.CreateAsync(branch);
        }

        public async Task UpdateAsync(BranchModel branch, bool forceUpdate = false)
        {
                await _branchDAL.UpdateAsync(branch, forceUpdate);
        }

        public async Task DeleteAsync(int id, int bankId, byte[] rowVersion, bool forceDelete = false)
        {
                await _branchDAL.DeleteAsync(id, bankId, rowVersion, forceDelete);
        }

        public async Task<List<BranchModel>> GetAllByBankIdAsync(int bankId)
        {
                return await _branchDAL.GetAllByBankIdAsync(bankId);
        }

        public async Task<BranchModel> GetByIdAsync(int id, int bankId)
        {
                return await _branchDAL.GetByIdAsync(id, bankId);
        }
    }

}
