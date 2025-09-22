using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Api;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Api
{
    public class TicketingDesginDAL : ITicketingDesignDAL
    {
        private const int TimeoutException = -2;
        public async Task<(TicketingDesignModel screen, int status)> GetActiveScreenButtonsForBranchAsync(int bankId, int? branchId, bool onlyAllocated)
        {
            using var conn = DatabaseHelper.GetConnection();
            try
            {
                await conn.OpenAsync();

                using var cmd = new SqlCommand("dbo.GetActiveScreenButtonsForBranch", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 30
                };

                cmd.Parameters.AddWithValue("@BankId", bankId);
                cmd.Parameters.AddWithValue("@BranchId", (object)branchId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OnlyAllocated", onlyAllocated);

                var pStatus = new SqlParameter("@Status", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pStatus);

                TicketingDesignModel screen = null;
                var buttons = new List<ButtonModel>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    // --- First result set: Branch info ---
                    string branchNameEn = null;
                    string branchNameAr = null;
                    if (await reader.ReadAsync())
                    {
                        branchNameEn = reader["BranchNameEnglish"] as string;
                        branchNameAr = reader["BranchNameArabic"] as string;
                    }

                    // Move to second result set
                    await reader.NextResultAsync();

                    while (await reader.ReadAsync())
                    {
                        if (screen == null)
                        {
                            screen = new TicketingDesignModel
                            {
                                ScreenId = reader["ScreenId"] != DBNull.Value ? (int)reader["ScreenId"] : 0,
                                ScreenName = reader["ScreenName"] as string ?? string.Empty,
                                BranchNameEnglish = branchNameEn,
                                BranchNameArabic = branchNameAr,
                                Buttons = new List<ButtonModel>()
                            };
                        }

                        buttons.Add(new ButtonModel
                        {
                            ButtonId = reader["ButtonId"] != DBNull.Value ? (int)reader["ButtonId"] : 0,
                            ScreenId = reader["ScreenId"] != DBNull.Value ? (int)reader["ScreenId"] : 0,
                            ScreenName = reader["ScreenName"] as string,
                            NameEnglish = reader["NameEnglish"] as string ?? string.Empty,
                            NameArabic = reader["NameArabic"] as string ?? string.Empty,
                            ButtonType = reader["ButtonType"] != DBNull.Value ? (ButtonType)(int)reader["ButtonType"] : 0,
                            MessageEnglish = reader["MessageEnglish"] as string,
                            MessageArabic = reader["MessageArabic"] as string,
                            ServiceId = reader["ServiceId"] == DBNull.Value ? (int?)null : (int)reader["ServiceId"],
                            ServiceNameEnglish = reader["ServiceNameEnglish"] as string,
                            ServiceNameArabic = reader["ServiceNameArabic"] as string
                        });
                    }
                }

                if (screen != null)
                    screen.Buttons = buttons;

                var status = (ActiveScreenStatus)((pStatus.Value is int i) ? i : 0);
                return (screen, (int)status);
            }
            catch (SqlException ex) when (ex.Number == TimeoutException)
            {
                throw new DatabaseTimeoutException("Database timeout while reading active screen/buttons.", ex);
            }
        }
    }
}
