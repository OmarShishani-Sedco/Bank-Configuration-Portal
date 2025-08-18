using System;
using System.Threading.Tasks;
using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Common.Security;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;

namespace Bank_Configuration_Portal.BLL
{
    public class UserManager : IUserManager
    {
        private readonly IUserDAL _userDAL;

        public UserManager(IUserDAL userDAL)
        {
            _userDAL = userDAL;
        }

        public async Task<(bool valid, bool mustChange)> ValidateCredentialsAsync(string userName, string password)
        {
                var user = await _userDAL.GetByUserNameAsync(userName);
                if (user == null || !user.IsActive)
                    return (false, false);
            if (!user.IsActive && user.MustChangePassword)
                return (false, true);

            var valid = PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt, user.Iterations);
                return (valid, valid && user.MustChangePassword);
        }

        public async Task<(string plainPassword, bool created)> CreateOrResetUserAsync(string userName, int? mapToBankId = null)
        {
                var existing = await _userDAL.GetByUserNameAsync(userName);
                var pwd = PasswordGenerator.Generate();
                var (hash, salt, iters) = PasswordHasher.Hash(pwd);

                if (existing == null)
                {
                    await _userDAL.CreateAsync(new AppUserModel
                    {
                        UserName = userName,
                        PasswordHash = hash,
                        PasswordSalt = salt,
                        Iterations = iters,
                        IsActive = true,
                        MustChangePassword = true
                    });

                    if (mapToBankId.HasValue)
                        await _userDAL.EnsureUserMappedToBankAsync(userName, mapToBankId.Value);

                    return (pwd, true);
                }
                else
                {
                    await _userDAL.UpdatePasswordAsync(userName, hash, salt, iters, true);

                    if (mapToBankId.HasValue)
                        await _userDAL.EnsureUserMappedToBankAsync(userName, mapToBankId.Value);

                    return (pwd, false);
                }
        }

        public async Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
                var user = await _userDAL.GetByUserNameAsync(userName);
                if (user == null)
                    return false;

                if (!PasswordHasher.Verify(oldPassword, user.PasswordHash, user.PasswordSalt, user.Iterations))
                    return false;

                var (hash, salt, iters) = PasswordHasher.Hash(newPassword, user.Iterations);
                await _userDAL.UpdatePasswordAsync(userName, hash, salt, iters, false);
                return true;
        }
    }
}
