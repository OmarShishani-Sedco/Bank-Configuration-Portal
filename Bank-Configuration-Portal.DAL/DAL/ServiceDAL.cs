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
    public class ServiceDAL : IServiceDAL
    {
        public async Task<List<ServiceModel>> GetAllByBankIdAsync(int bankId)
        {
            var services = new List<ServiceModel>();

            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("SELECT * FROM Service WHERE BankId = @BankId", connection))
            {
                command.Parameters.AddWithValue("@BankId", bankId);

                try
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(MapReaderToService(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    throw;
                }
            }

            return services;
        }

        public async Task<ServiceModel?> GetByIdAsync(int serviceId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("SELECT * FROM Service WHERE ServiceId = @ServiceId", connection))
            {
                command.Parameters.AddWithValue("@ServiceId", serviceId);

                try
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return MapReaderToService(reader);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    throw;
                }
            }

            return null;
        }

        public async Task<int> CreateAsync(ServiceModel service)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand(@"
                INSERT INTO Service (BankId, NameEnglish, NameArabic, IsActive, MaxTicketsPerDay)
                VALUES (@BankId, @NameEnglish, @NameArabic, @IsActive, @MaxTicketsPerDay);
                SELECT SCOPE_IDENTITY();", connection))
            {
                command.Parameters.AddWithValue("@BankId", service.BankId);
                command.Parameters.AddWithValue("@NameEnglish", service.NameEnglish);
                command.Parameters.AddWithValue("@NameArabic", service.NameArabic);
                command.Parameters.AddWithValue("@IsActive", service.IsActive);
                command.Parameters.AddWithValue("@MaxTicketsPerDay", service.MaxTicketsPerDay);

                try
                {
                    await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    throw;
                }
            }
        }

        public async Task UpdateAsync(ServiceModel service, bool forceUpdate = false)
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                await conn.OpenAsync();

                if (!forceUpdate)
                {
                    string selectQuery = "SELECT RowVersion FROM Service WHERE ServiceId = @Id AND BankId = @BankId";
                    using var versionCmd = new SqlCommand(selectQuery, conn);
                    versionCmd.Parameters.AddWithValue("@Id", service.Id);
                    versionCmd.Parameters.AddWithValue("@BankId", service.BankId);

                    using var reader = await versionCmd.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                        throw new DBConcurrencyException("The service was deleted.");

                    byte[] currentVersion = (byte[])reader["RowVersion"];
                    if (!currentVersion.SequenceEqual(service.RowVersion))
                        throw new DBConcurrencyException("The service was modified by another user.");
                }

                string updateQuery = forceUpdate
                    ? @"UPDATE Service 
               SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive, MaxTicketsPerDay = @MaxTicketsPerDay 
               WHERE ServiceId = @Id AND BankId = @BankId"
                    : @"UPDATE Service 
               SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive, MaxTicketsPerDay = @MaxTicketsPerDay 
               WHERE ServiceId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@NameEnglish", service.NameEnglish);
                cmd.Parameters.AddWithValue("@NameArabic", service.NameArabic);
                cmd.Parameters.AddWithValue("@IsActive", service.IsActive);
                cmd.Parameters.AddWithValue("@MaxTicketsPerDay", service.MaxTicketsPerDay);
                cmd.Parameters.AddWithValue("@Id", service.Id);
                cmd.Parameters.AddWithValue("@BankId", service.BankId);

                if (!forceUpdate)
                    cmd.Parameters.Add(new SqlParameter("@RowVersion", SqlDbType.Timestamp) { Value = service.RowVersion });

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected == 0 && !forceUpdate)
                    throw new DBConcurrencyException("The service was modified by another user.");

                // Refresh RowVersion
                string versionQuery = "SELECT RowVersion FROM Service WHERE ServiceId = @Id AND BankId = @BankId";
                using var refreshCmd = new SqlCommand(versionQuery, conn);
                refreshCmd.Parameters.AddWithValue("@Id", service.Id);
                refreshCmd.Parameters.AddWithValue("@BankId", service.BankId);

                using var versionReader = await refreshCmd.ExecuteReaderAsync();
                if (await versionReader.ReadAsync())
                    service.RowVersion = (byte[])versionReader["RowVersion"];
            }
            catch (DBConcurrencyException ex)
            {
                Logger.LogError(ex, "ServiceDAL.UpdateAsync");
                throw;
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "ServiceDAL.UpdateAsync");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceDAL.UpdateAsync");
                throw;
            }
        }


        public async Task<bool> DeleteAsync(int serviceId, byte[] rowVersion)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("DELETE FROM Service WHERE ServiceId = @ServiceId AND RowVersion = @RowVersion", connection))
            {
                command.Parameters.AddWithValue("@ServiceId", serviceId);
                command.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = rowVersion;

                try
                {
                    await connection.OpenAsync();
                    int affected = await command.ExecuteNonQueryAsync();
                    return affected > 0;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    throw;
                }
            }
        }

        private ServiceModel MapReaderToService(SqlDataReader reader)
        {
            return new ServiceModel
            {
                Id = Convert.ToInt32(reader["ServiceId"]),
                BankId = Convert.ToInt32(reader["BankId"]),
                NameEnglish = reader["NameEnglish"] as string,
                NameArabic = reader["NameArabic"] as string,
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                MaxTicketsPerDay = Convert.ToInt32(reader["MaxTicketsPerDay"]),
                RowVersion = (byte[])reader["RowVersion"]
            };
        }
    }
}
