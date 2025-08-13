using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.DAL
{
    public class UserDAL : IUserDAL
    {
        public async Task<AppUserModel> GetByUserNameAsync(string userName)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(@"
                        SELECT UserName, PasswordHash, PasswordSalt, Iterations, IsActive, MustChangePassword
                        FROM dbo.AppUser
                        WHERE UserName = @UserName", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                return null;

                            return new AppUserModel
                            {
                                UserName = reader["UserName"] as string,
                                PasswordHash = (byte[])reader["PasswordHash"],
                                PasswordSalt = (byte[])reader["PasswordSalt"],
                                Iterations = (int)reader["Iterations"],
                                IsActive = (bool)reader["IsActive"],
                                MustChangePassword = (bool)reader["MustChangePassword"]

                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task CreateAsync(AppUserModel user)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(@"
                        INSERT INTO dbo.AppUser
                            (UserName, PasswordHash, PasswordSalt, Iterations, IsActive, MustChangePassword)
                        VALUES
                            (@UserName, @PasswordHash, @PasswordSalt, @Iterations, @IsActive, @MustChangePassword)", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, user.PasswordHash.Length).Value = user.PasswordHash;
                        cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, user.PasswordSalt.Length).Value = user.PasswordSalt;
                        cmd.Parameters.AddWithValue("@Iterations", user.Iterations);
                        cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                        cmd.Parameters.AddWithValue("@MustChangePassword", user.MustChangePassword);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task UpdatePasswordAsync(string userName, byte[] hash, byte[] salt, int iterations, bool mustChangePassword)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(@"
                        UPDATE dbo.AppUser
                           SET PasswordHash = @PasswordHash,
                               PasswordSalt = @PasswordSalt,
                               Iterations = @Iterations,
                               MustChangePassword = @MustChangePassword
                         WHERE UserName = @UserName", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, hash.Length).Value = hash;
                        cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, salt.Length).Value = salt;
                        cmd.Parameters.AddWithValue("@Iterations", iterations);
                        cmd.Parameters.AddWithValue("@MustChangePassword", mustChangePassword);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public async Task EnsureUserMappedToBankAsync(string userName, int bankId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand(@"
                        IF NOT EXISTS (SELECT 1 FROM dbo.BankUserMapping WHERE UserName = @UserName AND BankId = @BankId)
                            INSERT INTO dbo.BankUserMapping(UserName, BankId) VALUES(@UserName, @BankId)", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@BankId", bankId);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
    }
}
