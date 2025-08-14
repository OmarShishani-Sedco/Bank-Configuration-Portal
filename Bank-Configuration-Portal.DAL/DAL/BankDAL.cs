using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.DAL
{
    public class BankDAL : IBankDAL
    {
        public async Task<BankModel?> GetByNameAsync(string name)
        {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand("SELECT BankId, BankName FROM Bank WHERE BankName = @Name", conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new BankModel
                                {
                                    Id = (int)reader["BankId"],
                                    Name = (string)reader["BankName"]
                                };
                            }
                        }
                    }
                }

                return null;
        }

        public async Task<bool> BankUserMappingExistsAsync(string username, int bankId)
        {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT COUNT(1) FROM BankUserMapping 
                                 WHERE UserName = @UserName AND BankId = @BankId";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@BankId", bankId);

                        object result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result) > 0;
                    }
                }
        }
        public async Task<bool> BankExistsAsync(int bankId)
        {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT 1 FROM Bank WHERE BankId = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", bankId);
                        var result = await cmd.ExecuteScalarAsync();
                        return result != null;
                    }
                }
        }
    }

}
