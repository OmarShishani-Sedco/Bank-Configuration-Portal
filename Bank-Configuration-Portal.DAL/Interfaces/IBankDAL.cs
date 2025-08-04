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
        BankModel GetByName(string name);
        bool BankUserMappingExists(string username, int bankId);
    }
}
