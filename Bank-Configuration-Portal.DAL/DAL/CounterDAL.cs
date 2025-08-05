using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.DAL
{
    public class CounterDAL : ICounterDAL
    {
        public async Task<List<CounterModel>> GetAllByBranchIdAsync(int branchId)
        {
            var counters = new List<CounterModel>();

            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("SELECT * FROM Counter WHERE BranchId = @BranchId", connection);
            command.Parameters.AddWithValue("@BranchId", branchId);

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    counters.Add(new CounterModel
                    {
                        Id = (int)reader["CounterId"],
                        BranchId = (int)reader["BranchId"],
                        NameEnglish = reader["NameEnglish"].ToString() ?? "",
                        NameArabic = reader["NameArabic"].ToString() ?? "",
                        IsActive = (bool)reader["IsActive"],
                        Type = (CounterType)reader["CounterType"],
                        RowVersion = (byte[])reader["RowVersion"]
                    });
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "CounterDAL.GetAllByBranchIdAsync");
                throw;
            }

            return counters;
        }

        public async Task<CounterModel?> GetByIdAsync(int id)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("SELECT * FROM Counter WHERE CounterId = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new CounterModel
                    {
                        Id = (int)reader["CounterId"],
                        BranchId = (int)reader["BranchId"],
                        NameEnglish = reader["NameEnglish"].ToString() ?? "",
                        NameArabic = reader["NameArabic"].ToString() ?? "",
                        IsActive = (bool)reader["IsActive"],
                        Type = (CounterType)reader["CounterType"],
                        RowVersion = (byte[])reader["RowVersion"]
                    };
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "CounterDAL.GetByIdAsync");
                throw;
            }

            return null;
        }

        public async Task<int> CreateAsync(CounterModel counter)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand(@"
            INSERT INTO Counter (BranchId, NameEnglish, NameArabic, IsActive, CounterType)
            VALUES (@BranchId, @NameEnglish, @NameArabic, @IsActive, @CounterType);
            SELECT SCOPE_IDENTITY();", connection);

            command.Parameters.AddWithValue("@BranchId", counter.BranchId);
            command.Parameters.AddWithValue("@NameEnglish", counter.NameEnglish);
            command.Parameters.AddWithValue("@NameArabic", counter.NameArabic);
            command.Parameters.AddWithValue("@IsActive", counter.IsActive);
            command.Parameters.AddWithValue("@CounterType", (int)counter.Type);

            try
            {
                await connection.OpenAsync();
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "CounterDAL.CreateAsync");
                throw;
            }
        }

        public async Task UpdateAsync(CounterModel counter, bool forceUpdate = false)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand(@"
            UPDATE Counter
            SET NameEnglish = @NameEnglish,
                NameArabic = @NameArabic,
                IsActive = @IsActive,
                CounterType = @CounterType
            WHERE CounterId = @Id AND (RowVersion = @RowVersion OR @ForceUpdate = 1);", connection);

            command.Parameters.AddWithValue("@Id", counter.Id);
            command.Parameters.AddWithValue("@NameEnglish", counter.NameEnglish);
            command.Parameters.AddWithValue("@NameArabic", counter.NameArabic);
            command.Parameters.AddWithValue("@IsActive", counter.IsActive);
            command.Parameters.AddWithValue("@CounterType", (int)counter.Type);
            command.Parameters.AddWithValue("@RowVersion", counter.RowVersion);
            command.Parameters.AddWithValue("@ForceUpdate", forceUpdate ? 1 : 0);

            try
            {
                await connection.OpenAsync();
                var affected = await command.ExecuteNonQueryAsync();
                if (!forceUpdate && affected == 0)
                    throw new DBConcurrencyException("The record was modified by another user.");
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "CounterDAL.UpdateAsync");
                throw;
            }
        }

        public async Task DeleteAsync(int id, byte[] rowVersion)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("DELETE FROM Counter WHERE CounterId = @Id AND RowVersion = @RowVersion", connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@RowVersion", rowVersion);

            try
            {
                await connection.OpenAsync();
                var affected = await command.ExecuteNonQueryAsync();
                if (affected == 0)
                    throw new DBConcurrencyException("The record was modified or deleted by another user.");
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "CounterDAL.DeleteAsync");
                throw;
            }
        }
    }

}
