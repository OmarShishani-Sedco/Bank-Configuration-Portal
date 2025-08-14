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
            Logger.LogError(ex, "DirectoryNotFoundException");
            errorMessage = "Config folder is missing. Please ensure all files are correctly deployed.";
        }
        catch (ConfigurationErrorsException ex)
        {
            Logger.LogError(ex, "ConfigurationErrorsException");
            errorMessage = "Configuration file error: check appsettings.json. " + ex.Message;
        }
        catch (FileNotFoundException ex)
        {
            Logger.LogError(ex, "FileNotFoundException");
            errorMessage = "Config file is missing. Please ensure all files are correctly deployed. " + ex.Message;
        }
        catch (SqlException ex)
        {
            Logger.LogError(ex, "SqlException");
            errorMessage = "Cannot connect to database. Check SQL Server instance and credentials.";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            errorMessage = "Unexpected error while testing DB connection.";
        }

        return false;
    }
}

   

