using Bank_Configuration_Portal.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Interfaces
{
    public interface IUserDAL
    {
        Task<AppUserModel> GetByUserNameAsync(string userName);
        Task CreateAsync(AppUserModel user);
        Task UpdatePasswordAsync(string userName, byte[] hash, byte[] salt, int iterations, bool mustChangePassword);
        Task EnsureUserMappedToBankAsync(string userName, int bankId);
    }
}
