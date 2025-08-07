using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Interfaces
{
    public interface IBranchDAL
    {
        Task<List<BranchModel>> GetAllByBankIdAsync(int bankId);
        Task<BranchModel?> GetByIdAsync(int id, int bankId);
        Task CreateAsync(BranchModel branch);
        Task UpdateAsync(BranchModel branch, bool forceUpdate = false);
        Task DeleteAsync(int id, int bankId, byte[] rowVersion, bool forceDelete = false);
    }
}
