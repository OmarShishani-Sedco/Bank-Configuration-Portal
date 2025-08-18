using Bank_Configuration_Portal.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.IO;

public static class DatabaseUtility
{
    public static bool TestConnection(out string errorMessage)
    {
        try
        {
            AppConfig.Initialize();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                errorMessage = string.Empty;
                return true;
            }
        }
        catch (DirectoryNotFoundException ex)
        {
            errorMessage = "Config folder is missing. Please ensure all files are correctly deployed.";
            Logger.LogError(ex, $"DatabaseUtility.TestConnection | {errorMessage}");
        }
        catch (ConfigurationErrorsException ex)
        {
            errorMessage = "Configuration file error: check appsettings.json. " + ex.Message;
            Logger.LogError(ex, $"DatabaseUtility.TestConnection | {errorMessage}");
        }
        catch (FileNotFoundException ex)
        {
            errorMessage = "Config file is missing. Please ensure all files are correctly deployed. " + ex.Message;
            Logger.LogError(ex, $"DatabaseUtility.TestConnection | {errorMessage}");
        }
        catch (SqlException ex)
        {
            errorMessage = "Cannot connect to database. Check SQL Server instance and credentials.";
            Logger.LogError(ex, $"DatabaseUtility.TestConnection | {errorMessage}");
        }
        catch (Exception ex)
        {
            errorMessage = "Unexpected error while testing DB connection.";
            Logger.LogError(ex, $"DatabaseUtility.TestConnection | {errorMessage}");
        }

        return false;
    }
}

   

