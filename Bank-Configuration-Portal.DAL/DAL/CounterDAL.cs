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
            var countersDictionary = new Dictionary<int, CounterModel>();

            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand(@"
              SELECT c.*, a.ServiceId 
              FROM Counter c
              LEFT JOIN CounterServiceAllocation a ON c.CounterId = a.CounterId
              WHERE c.BranchId = @BranchId", connection);
            command.Parameters.AddWithValue("@BranchId", branchId);

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var counterId = (int)reader["CounterId"];
                    CounterModel counter;

                    if (!countersDictionary.TryGetValue(counterId, out counter))
                    {
                        counter = new CounterModel
                        {
                            Id = counterId,
                            BranchId = (int)reader["BranchId"],
                            NameEnglish = reader["NameEnglish"].ToString() ?? "",
                            NameArabic = reader["NameArabic"].ToString() ?? "",
                            IsActive = (bool)reader["IsActive"],
                            Type = (CounterType)Convert.ToInt32(reader["CounterType"]),
                            RowVersion = (byte[])reader["RowVersion"],
                            AllocatedServiceIds = new List<int>()
                        };
                        countersDictionary.Add(counterId, counter);
                    }

                    if (reader["ServiceId"] != DBNull.Value)
                    {
                        var serviceId = Convert.ToInt32(reader["ServiceId"]);
                        counter.AllocatedServiceIds.Add(serviceId);
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "CounterDAL.GetAllByBranchIdAsync");
                throw;
            }

            return countersDictionary.Values.ToList();
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
                        Type = (CounterType)Convert.ToInt32(reader["CounterType"]),
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


        public async Task<List<int>> GetAllocatedServiceIdsAsync(int counterId)
        {
            var serviceIds = new List<int>();
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("SELECT ServiceId FROM CounterServiceAllocation WHERE CounterId = @CounterId", connection);
            command.Parameters.AddWithValue("@CounterId", counterId);

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    serviceIds.Add(Convert.ToInt32(reader["ServiceId"]));
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "CounterDAL.GetAllocatedServiceIdsAsync");
                throw;
            }

            return serviceIds;
        }

        public async Task SaveAllocationsAsync(int counterId, List<int> serviceIds)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var deleteCommand = new SqlCommand("DELETE FROM CounterServiceAllocation WHERE CounterId = @CounterId", connection, transaction);
                deleteCommand.Parameters.AddWithValue("@CounterId", counterId);
                await deleteCommand.ExecuteNonQueryAsync();

                foreach (var serviceId in serviceIds)
                {
                    var insertCommand = new SqlCommand(
                        "INSERT INTO CounterServiceAllocation (CounterId, ServiceId) VALUES (@CounterId, @ServiceId)", connection, transaction);
                    insertCommand.Parameters.AddWithValue("@CounterId", counterId);
                    insertCommand.Parameters.AddWithValue("@ServiceId", serviceId);
                    await insertCommand.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                Logger.LogError(ex, $"CounterDAL.SaveAllocationsAsync for CounterId: {counterId}");
                throw;
            }
        }

        public async Task DeleteAllocationsByCounterIdAsync(int counterId)
        {
            using var connection = DatabaseHelper.GetConnection();
            var command = new SqlCommand("DELETE FROM CounterServiceAllocation WHERE CounterId = @CounterId", connection);
            command.Parameters.AddWithValue("@CounterId", counterId);

            try
            {
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, $"CounterDAL.DeleteAllocationsByCounterIdAsync for CounterId: {counterId}");
                throw;
            }
        }
    }

}
