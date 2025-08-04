using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Interfaces
{
    public interface IBranchManager
    {
        List<BranchModel> GetAllByBankId(int bankId);
        BranchModel GetById(int id, int bankId);
        void Create(BranchModel branch);
        void Update(BranchModel branch);
        void Delete(int id, int bankId, byte[] rowVersion);
    }
}
