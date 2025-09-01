using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.DAL.Interfaces;
using Bank_Configuration_Portal.Models.Api;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.DAL.Api
{
    public class TicketingDesginDAL : ITicketingDesignDAL
    {
        public async Task<TicketingDesignModel> GetActiveScreenButtonsForBranchAsync(int bankId, int branchId, bool onlyAllocated)
        {
            using var conn = DatabaseHelper.GetConnection();
            await conn.OpenAsync();

            TicketingDesignModel screen = null;
            var buttons = new List<ButtonModel>();

            string query = @"EXEC GetActiveScreenButtonsForBranch @BankId, @BranchId, @OnlyAllocated";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@BankId", bankId);
            cmd.Parameters.AddWithValue("@BranchId", branchId);
            cmd.Parameters.AddWithValue("@OnlyAllocated", onlyAllocated);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (screen == null)
                {
                    screen = new TicketingDesignModel
                    {
                        ScreenId = reader["ScreenId"] != DBNull.Value ? (int)reader["ScreenId"] : 0,
                        ScreenName = reader["ScreenName"] as string ?? string.Empty,
                        Buttons = new List<ButtonModel>()
                    };
                }

                buttons.Add(new ButtonModel
                {
                    ButtonId = reader["ButtonId"] != DBNull.Value ? (int)reader["ButtonId"] : 0,
                    ScreenId = reader["ScreenId"] != DBNull.Value ? (int)reader["ScreenId"] : 0,
                    NameEnglish = reader["NameEnglish"] as string ?? string.Empty,
                    NameArabic = reader["NameArabic"] as string ?? string.Empty,
                    ButtonType = reader["ButtonType"] != DBNull.Value ? (ButtonType)(int)reader["ButtonType"] : 0,
                    MessageEnglish = reader["MessageEnglish"] as string,
                    MessageArabic = reader["MessageArabic"] as string,
                    ServiceId = reader["ServiceId"] == DBNull.Value ? (int?)null : (int)reader["ServiceId"]
                });
            }

            if (screen != null)
            {
                screen.Buttons = buttons;
            }

            return screen; 
        }

    }
}
