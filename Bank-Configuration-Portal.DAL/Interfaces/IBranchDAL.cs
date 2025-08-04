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
        List<BranchModel> GetAllByBankId(int bankId);
        BranchModel GetById(int id, int bankId);
        void Delete(int id, int bankId, byte[] rowVersion);
        void Update(BranchModel branch);
        void Create(BranchModel branch);
    }
}
