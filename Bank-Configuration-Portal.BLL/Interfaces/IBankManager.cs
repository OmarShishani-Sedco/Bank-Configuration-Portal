using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Interfaces
{
    public interface IBankManager
    {
        BankModel GetByName(string name);
        bool IsUserMappedToBank(string username, int bankId);
    }
}
