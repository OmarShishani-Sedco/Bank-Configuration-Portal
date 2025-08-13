using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Interfaces
{
    public interface IBankDAL
    {
        Task<BankModel?> GetByNameAsync(string name);
        Task<bool> BankUserMappingExistsAsync(string username, int bankId);
        Task<bool> BankExistsAsync(int bankId);
    }

}
