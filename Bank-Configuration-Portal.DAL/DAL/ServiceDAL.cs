using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Common.Paging;
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

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(MapReaderToService(reader));
                        }
                    }
            }
            return services;
        }

        public async Task<PagedResult<ServiceModel>> GetPagedByBankIdAsync(
           int bankId, string searchTerm, bool? isActive, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 6;

            using var conn = DatabaseHelper.GetConnection();
            await conn.OpenAsync();

            var items = new List<ServiceModel>();
            int totalCount = 0;

            string query = @"
                -- 1) total rows
                SELECT COUNT(1)
                FROM Service
                WHERE BankId = @BankId
                  AND (@Search   IS NULL OR NameEnglish LIKE @Search OR NameArabic LIKE @Search)
                  AND (@IsActive IS NULL OR IsActive = @IsActive);

                -- 2) current page
                SELECT ServiceId, BankId, NameEnglish, NameArabic,
                       MaxTicketsPerDay, MinServiceTimeSeconds, MaxServiceTimeSeconds,
                       IsActive, RowVersion
                FROM Service
                WHERE BankId = @BankId
                  AND (@Search   IS NULL OR NameEnglish LIKE @Search OR NameArabic LIKE @Search)
                  AND (@IsActive IS NULL OR IsActive = @IsActive)
                ORDER BY NameEnglish, ServiceId
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@BankId", bankId);

            var searchValue = string.IsNullOrWhiteSpace(searchTerm) ? (object)DBNull.Value : $"%{searchTerm}%";
            cmd.Parameters.AddWithValue("@Search", searchValue);

            var isActiveValue = (object?)isActive ?? DBNull.Value;
            cmd.Parameters.AddWithValue("@IsActive", isActiveValue);

            int offset = (page - 1) * pageSize;
            cmd.Parameters.AddWithValue("@Offset", offset);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            using var reader = await cmd.ExecuteReaderAsync();

            // total count
            if (await reader.ReadAsync())
                totalCount = Convert.ToInt32(reader[0]);

            // page rows
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    items.Add(MapReaderToService(reader));
                }
            }

            return new PagedResult<ServiceModel>(items, totalCount, page, pageSize);
        }

        public async Task<List<ServiceModel>> GetByIdsAsync(IEnumerable<int> serviceIds)
        {
            var services = new List<ServiceModel>();

            var idsSqlParameters = new List<SqlParameter>();
            var parameterNames = new List<string>();

            int i = 0;
            foreach (var id in serviceIds)
            {
                var paramName = $"@p{i}";
                parameterNames.Add(paramName);
                idsSqlParameters.Add(new SqlParameter(paramName, id));
                i++;
            }

            string sqlQuery = $"SELECT * FROM Service WHERE ServiceId IN ({string.Join(",", parameterNames)})";

            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddRange(idsSqlParameters.ToArray());

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(MapReaderToService(reader));
                        }
                    }
            }

            return services;
        }
        public async Task<List<ServiceModel>> GetAllActiveByBankIdAsync(int bankId)
        {
            var services = new List<ServiceModel>();
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand("SELECT * FROM Service WHERE BankId = @BankId AND IsActive = 1", connection))
            {
                command.Parameters.AddWithValue("@BankId", bankId);
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(MapReaderToService(reader));
                        }
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

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return MapReaderToService(reader);
                    }
            }

            return null;
        }

        public async Task<int> CreateAsync(ServiceModel service)
        {
            using (var connection = DatabaseHelper.GetConnection())
            using (var command = new SqlCommand(@"
                INSERT INTO Service (BankId, NameEnglish, NameArabic, IsActive, MaxTicketsPerDay, MinServiceTimeSeconds, MaxServiceTimeSeconds)
                VALUES (@BankId, @NameEnglish, @NameArabic, @IsActive, @MaxTicketsPerDay, @MinServiceTimeSeconds, @MaxServiceTimeSeconds);
                SELECT SCOPE_IDENTITY();", connection))
            {
                command.Parameters.AddWithValue("@BankId", service.BankId);
                command.Parameters.AddWithValue("@NameEnglish", service.NameEnglish);
                command.Parameters.AddWithValue("@NameArabic", service.NameArabic);
                command.Parameters.AddWithValue("@IsActive", service.IsActive);
                command.Parameters.AddWithValue("@MaxTicketsPerDay", service.MaxTicketsPerDay);
                command.Parameters.AddWithValue("@MinServiceTimeSeconds", service.MinServiceTimeSeconds);
                command.Parameters.AddWithValue("@MaxServiceTimeSeconds", service.MaxServiceTimeSeconds);

                await connection.OpenAsync();
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
            }
        }

        public async Task UpdateAsync(ServiceModel service, bool forceUpdate = false)
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
               SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive, MaxTicketsPerDay = @MaxTicketsPerDay, MinServiceTimeSeconds = @MinServiceTimeSeconds, MaxServiceTimeSeconds = @MaxServiceTimeSeconds
               WHERE ServiceId = @Id AND BankId = @BankId"
                    : @"UPDATE Service 
               SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive, MaxTicketsPerDay = @MaxTicketsPerDay, MinServiceTimeSeconds = @MinServiceTimeSeconds, MaxServiceTimeSeconds = @MaxServiceTimeSeconds
               WHERE ServiceId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@NameEnglish", service.NameEnglish);
                cmd.Parameters.AddWithValue("@NameArabic", service.NameArabic);
                cmd.Parameters.AddWithValue("@IsActive", service.IsActive);
                cmd.Parameters.AddWithValue("@MaxTicketsPerDay", service.MaxTicketsPerDay);
                cmd.Parameters.AddWithValue("@MinServiceTimeSeconds", service.MinServiceTimeSeconds);
                cmd.Parameters.AddWithValue("@MaxServiceTimeSeconds", service.MaxServiceTimeSeconds);
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


        public async Task DeleteAsync(int serviceId, byte[] rowVersion, bool forceDelete = false)
        {
                using var connection = DatabaseHelper.GetConnection();
                await connection.OpenAsync();

                string deleteQuery = forceDelete ?
                    @"DELETE FROM Service WHERE ServiceId = @ServiceId" :
                    @"DELETE FROM Service WHERE ServiceId = @ServiceId AND RowVersion = @RowVersion";

                using var command = new SqlCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@ServiceId", serviceId);

                if (!forceDelete)
                {
                    command.Parameters.Add("@RowVersion", SqlDbType.Timestamp).Value = rowVersion;
                }

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    string checkQuery = "SELECT COUNT(1) FROM Service WHERE ServiceId = @ServiceId";
                    using var checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@ServiceId", serviceId);

                    bool exists = (int)await checkCommand.ExecuteScalarAsync() > 0;

                    if (exists)
                    {
                        // The record exists, but the RowVersion didn't match. It was modified.
                        throw new CustomConcurrencyModifiedException("The service was modified by another user.");
                    }
                    else
                    {
                        // The record no longer exists. It was deleted.
                        throw new CustomConcurrencyDeletedException("The service was deleted by another user.");
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
                RowVersion = (byte[])reader["RowVersion"],
                MinServiceTimeSeconds = Convert.ToInt32(reader["MinServiceTimeSeconds"]),
                MaxServiceTimeSeconds = Convert.ToInt32(reader["MaxServiceTimeSeconds"])
            };
        }
    }
}
