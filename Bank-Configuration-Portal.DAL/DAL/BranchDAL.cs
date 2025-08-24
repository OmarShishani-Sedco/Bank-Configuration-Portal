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
    public class BranchDAL : IBranchDAL
    {
        public async Task<List<BranchModel>> GetAllByBankIdAsync(int bankId)
        {
                using var conn = DatabaseHelper.GetConnection();
                await conn.OpenAsync();

                var branches = new List<BranchModel>();
                string query = @"SELECT BranchId, BankId, NameEnglish, NameArabic, IsActive, RowVersion 
                                 FROM Branch WHERE BankId = @BankId";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BankId", bankId);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    branches.Add(new BranchModel
                    {
                        Id = (int)reader["BranchId"],
                        BankId = (int)reader["BankId"],
                        NameEnglish = reader["NameEnglish"] as string,
                        NameArabic = reader["NameArabic"] as string,
                        IsActive = (bool)reader["IsActive"],
                        RowVersion = (byte[])reader["RowVersion"]
                    });
                }

                return branches;
        }

        public async Task<PagedResult<BranchModel>> GetPagedByBankIdAsync(
              int bankId, string searchTerm, bool? isActive, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 10;

            using var conn = DatabaseHelper.GetConnection();
            await conn.OpenAsync();

            var branches = new List<BranchModel>();
            int totalCount = 0;

            string query = @"
                -- 1) total rows
                SELECT COUNT(1)
                FROM Branch
                WHERE BankId = @BankId
                  AND (@Search   IS NULL OR NameEnglish LIKE @Search OR NameArabic LIKE @Search)
                  AND (@IsActive IS NULL OR IsActive = @IsActive);

                -- 2) current page
                SELECT BranchId, BankId, NameEnglish, NameArabic, IsActive, RowVersion
                FROM Branch
                WHERE BankId = @BankId
                  AND (@Search   IS NULL OR NameEnglish LIKE @Search OR NameArabic LIKE @Search)
                  AND (@IsActive IS NULL OR IsActive = @IsActive)
                ORDER BY NameEnglish, BranchId
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

            // result set 1: total count
            if (await reader.ReadAsync())
                totalCount = Convert.ToInt32(reader[0]);

            // result set 2: page rows
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    branches.Add(new BranchModel
                    {
                        Id = (int)reader["BranchId"],
                        BankId = (int)reader["BankId"],
                        NameEnglish = reader["NameEnglish"] as string,
                        NameArabic = reader["NameArabic"] as string,
                        IsActive = (bool)reader["IsActive"],
                        RowVersion = (byte[])reader["RowVersion"]
                    });
                }
            }

            return new PagedResult<BranchModel>(branches, totalCount, page, pageSize);
        }


        public async Task<BranchModel?> GetByIdAsync(int id, int bankId)
        {
                using var conn = DatabaseHelper.GetConnection();
                await conn.OpenAsync();

                string query = @"SELECT BranchId, BankId, NameEnglish, NameArabic, IsActive, RowVersion 
                                 FROM Branch WHERE BranchId = @Id AND BankId = @BankId";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@BankId", bankId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new BranchModel
                    {
                        Id = (int)reader["BranchId"],
                        BankId = (int)reader["BankId"],
                        NameEnglish = reader["NameEnglish"] as string,
                        NameArabic = reader["NameArabic"] as string,
                        IsActive = (bool)reader["IsActive"],
                        RowVersion = (byte[])reader["RowVersion"]
                    };
                }

                return null;
        }

        public async Task CreateAsync(BranchModel branch)
        {
                using var conn = DatabaseHelper.GetConnection();
                await conn.OpenAsync();

                string query = @"INSERT INTO Branch (BankId, NameEnglish, NameArabic, IsActive) 
                                 VALUES (@BankId, @NameEnglish, @NameArabic, @IsActive);
                                 SELECT SCOPE_IDENTITY()";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BankId", branch.BankId);
                cmd.Parameters.AddWithValue("@NameEnglish", branch.NameEnglish);
                cmd.Parameters.AddWithValue("@NameArabic", branch.NameArabic);
                cmd.Parameters.AddWithValue("@IsActive", branch.IsActive);

                branch.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task UpdateAsync(BranchModel branch, bool forceUpdate = false)
        {
                using var conn = DatabaseHelper.GetConnection();
                await conn.OpenAsync();

                if (!forceUpdate)
                {
                    string selectQuery = "SELECT RowVersion FROM Branch WHERE BranchId = @Id AND BankId = @BankId";
                    using var versionCmd = new SqlCommand(selectQuery, conn);
                    versionCmd.Parameters.AddWithValue("@Id", branch.Id);
                    versionCmd.Parameters.AddWithValue("@BankId", branch.BankId);

                    using var reader = await versionCmd.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                        throw new DBConcurrencyException("The branch was deleted.");

                    byte[] currentVersion = (byte[])reader["RowVersion"];
                    if (!currentVersion.SequenceEqual(branch.RowVersion))
                        throw new DBConcurrencyException("The branch was modified by another user.");
                }
                
                string updateQuery = forceUpdate
                    ? @"UPDATE Branch 
                       SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive 
                       WHERE BranchId = @Id AND BankId = @BankId"
                    : @"UPDATE Branch 
                       SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive 
                       WHERE BranchId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@NameEnglish", branch.NameEnglish);
                cmd.Parameters.AddWithValue("@NameArabic", branch.NameArabic);
                cmd.Parameters.AddWithValue("@IsActive", branch.IsActive);
                cmd.Parameters.AddWithValue("@Id", branch.Id);
                cmd.Parameters.AddWithValue("@BankId", branch.BankId);

                if (!forceUpdate)
                    cmd.Parameters.Add(new SqlParameter("@RowVersion", SqlDbType.Timestamp) { Value = branch.RowVersion });

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected == 0 && !forceUpdate)
                    throw new DBConcurrencyException("The branch was modified by another user.");

                // Refresh RowVersion
                string versionQuery = "SELECT RowVersion FROM Branch WHERE BranchId = @Id AND BankId = @BankId";
                using var refreshCmd = new SqlCommand(versionQuery, conn);
                refreshCmd.Parameters.AddWithValue("@Id", branch.Id);
                refreshCmd.Parameters.AddWithValue("@BankId", branch.BankId);

                using var versionReader = await refreshCmd.ExecuteReaderAsync();
                if (await versionReader.ReadAsync())
                    branch.RowVersion = (byte[])versionReader["RowVersion"];
        }

        public async Task DeleteAsync(int id, int bankId, byte[] rowVersion, bool forceDelete = false)
        {
                using var conn = DatabaseHelper.GetConnection();
                await conn.OpenAsync();

                string deleteQuery = forceDelete ?
                    @"DELETE FROM Branch
              WHERE BranchId = @Id AND BankId = @BankId" :
                    @"DELETE FROM Branch
              WHERE BranchId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                using var cmd = new SqlCommand(deleteQuery, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@BankId", bankId);

                if (!forceDelete)
                {
                    cmd.Parameters.Add(new SqlParameter("@RowVersion", SqlDbType.Timestamp) { Value = rowVersion });
                }

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    string checkQuery = "SELECT COUNT(1) FROM Branch WHERE BranchId = @Id AND BankId = @BankId";
                    using var checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    checkCmd.Parameters.AddWithValue("@BankId", bankId);

                    bool exists = (int)await checkCmd.ExecuteScalarAsync() > 0;

                    if (exists)
                    {
                        // The record exists, but the RowVersion didn't match. It was modified.
                        throw new CustomConcurrencyModifiedException("The branch was modified by another user.");
                    }
                    else
                    {
                        // The record no longer exists. It was deleted.
                        throw new CustomConcurrencyDeletedException("The branch was deleted by another user.");
                    }
                }
        }
    }
}
