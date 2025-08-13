using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL.Interfaces
{
    public interface IUserManager
    {
        Task<(bool valid, bool mustChange)> ValidateCredentialsAsync(string userName, string password);
        Task<(string plainPassword, bool created)> CreateOrResetUserAsync(string userName, int? mapToBankId = null);
        Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
    }
}
