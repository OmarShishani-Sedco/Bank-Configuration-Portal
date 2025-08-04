using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

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
                                     FROM Branch WHERE Id = @Id AND BankId = @BankId";

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

        public void Update(BranchModel branch)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE Branch 
                                     SET NameEnglish = @NameEnglish, NameArabic = @NameArabic, IsActive = @IsActive
                                     WHERE BranchId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NameEnglish", branch.NameEnglish);
                        cmd.Parameters.AddWithValue("@NameArabic", branch.NameArabic);
                        cmd.Parameters.AddWithValue("@IsActive", branch.IsActive);
                        cmd.Parameters.AddWithValue("@Id", branch.Id);
                        cmd.Parameters.AddWithValue("@BankId", branch.BankId);
                        cmd.Parameters.Add(new SqlParameter("@RowVersion", System.Data.SqlDbType.Timestamp) { Value = branch.RowVersion });

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Update failed. The record may have been modified or deleted by another user.");
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

        public void Delete(int id, int bankId, byte[] rowVersion)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    string query = @"DELETE FROM Branch WHERE BranchId = @Id AND BankId = @BankId AND RowVersion = @RowVersion";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@BankId", bankId);
                        cmd.Parameters.Add(new SqlParameter("@RowVersion", System.Data.SqlDbType.Timestamp) { Value = rowVersion });

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Delete failed. The record may have been modified or deleted by another user.");
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
    }
}
