using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Bank_Configuration_Portal.DAL.DAL
{
    public class BranchDAL : IBranchDAL
    {
        public List<BranchModel> GetAllByBankId(int bankId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT BranchId, BankId, NameEnglish, NameArabic, IsActive, RowVersion 
                                     FROM Branch WHERE BankId = @BankId";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", bankId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var branches = new List<BranchModel>();
                            while (reader.Read())
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
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex);
                throw;
            }


        }
        public BranchModel GetById(int id, int bankId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT BranchId, BankId, NameEnglish, NameArabic, IsActive, RowVersion 
                                     FROM Branch WHERE BranchId = @Id AND BankId = @BankId";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@BankId", bankId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
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
                        }
                    }
                }

                return null;
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public void Create(BranchModel branch)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"INSERT INTO Branch (BankId, NameEnglish, NameArabic, IsActive) 
                                     VALUES (@BankId, @NameEnglish, @NameArabic, @IsActive);
                                     SELECT SCOPE_IDENTITY()";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", branch.BankId);
                        cmd.Parameters.AddWithValue("@NameEnglish", branch.NameEnglish);
                        cmd.Parameters.AddWithValue("@NameArabic", branch.NameArabic);
                        cmd.Parameters.AddWithValue("@IsActive", branch.IsActive);
                        conn.Open();
                        branch.Id = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        public void Update(BranchModel branch, bool forceUpdate = false)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    if (!forceUpdate)
                    {
                        string selectQuery = "SELECT RowVersion FROM Branch WHERE BranchId = @Id AND BankId = @BankId";
                        using (var cmd = new SqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", branch.Id);
                            cmd.Parameters.AddWithValue("@BankId", branch.BankId);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new DBConcurrencyException("The branch was deleted.");

                                byte[] currentVersion = (byte[])reader["RowVersion"];
                                if (!currentVersion.SequenceEqual(branch.RowVersion))
                                    throw new DBConcurrencyException("The branch was modified by another user.");
                            }
                        }
                    }

                    string updateQuery = forceUpdate
                        ? @"UPDATE Branch 
                   SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive 
                   WHERE BranchId = @Id AND BankId = @BankId"
                        : @"UPDATE Branch 
                   SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive 
                   WHERE BranchId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                    using (var cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@NameEnglish", branch.NameEnglish);
                        cmd.Parameters.AddWithValue("@NameArabic", branch.NameArabic);
                        cmd.Parameters.AddWithValue("@IsActive", branch.IsActive);
                        cmd.Parameters.AddWithValue("@Id", branch.Id);
                        cmd.Parameters.AddWithValue("@BankId", branch.BankId);

                        if (!forceUpdate)
                        {
                            cmd.Parameters.Add(new SqlParameter("@RowVersion", SqlDbType.Timestamp) { Value = branch.RowVersion });
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0 && !forceUpdate)
                            throw new DBConcurrencyException("The branch was modified by another user.");
                    }

                    // Refresh RowVersion
                    string versionQuery = "SELECT RowVersion FROM Branch WHERE BranchId = @Id AND BankId = @BankId";
                    using (var cmd = new SqlCommand(versionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", branch.Id);
                        cmd.Parameters.AddWithValue("@BankId", branch.BankId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                branch.RowVersion = (byte[])reader["RowVersion"];
                        }
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                Logger.LogError(ex, "BranchDAL.Update");
                throw;
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "BranchDAL.Update");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "BranchDAL.Update");
                throw;
            }
        }


        public void Delete(int id, int bankId, byte[] rowVersion)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"DELETE FROM Branch 
                             WHERE BranchId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@BankId", bankId);
                        cmd.Parameters.Add(new SqlParameter("@RowVersion", SqlDbType.Timestamp) { Value = rowVersion });

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                            throw new DBConcurrencyException("Delete failed. The branch was modified or already deleted.");
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                Logger.LogError(ex, "BranchDAL.Delete");
                throw;
            }
            catch (SqlException ex)
            {
                Logger.LogError(ex, "BranchDAL.Delete");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "BranchDAL.Delete");
                throw;
            }
        }

    }
}
