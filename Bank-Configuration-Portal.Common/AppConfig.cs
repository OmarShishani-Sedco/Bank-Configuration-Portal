using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration; // Still used for ConfigurationErrorsException
using System.IO;

public static class AppConfig
{
    private static IConfigurationRoot? _configuration;
    private static bool _initialized = false;

    public static void Initialize()
    {
        if (_initialized) return;

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var configDirectory = Path.Combine(baseDirectory, "Config"); 

       
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();

        // Add appsettings.json from the Config directory first
        if (Directory.Exists(configDirectory))
        {
            var configFilePath = Path.Combine(configDirectory, "appsettings.json");
            if (File.Exists(configFilePath))
            {
                builder.SetBasePath(configDirectory)
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            }
            else
            {
                // If Config directory exists but appsettings.json is not there, it's an error
                throw new FileNotFoundException($"Configuration file 'appsettings.json' not found in 'Config' directory: {configFilePath}");
            }
        }
        else
        {
            throw new DirectoryNotFoundException($"Config directory not found at: {configDirectory}");
        }

        _configuration = builder.Build();
        _initialized = true;
    }

    public static string GetConnectionString()
    {
        if (!_initialized)
            throw new InvalidOperationException("AppConfig is not initialized. Call AppConfig.Initialize() first.");

        // Access the 'DbConnection' section directly
        var databaseSection = _configuration!.GetSection("DbConnection");

        if (!databaseSection.Exists())
            throw new ConfigurationErrorsException("The 'DbConnection' section is empty or missing from appsettings.json.");

        // Retrieve individual connection string components
        var server = databaseSection["Server"];
        var database = databaseSection["Database"];
        var userId = databaseSection["UserId"];
        var password = databaseSection["Password"];

        // Use TryGetValue to safely parse boolean values
        bool trustServerCertificate = false;
        bool integratedSecurity = false;

        if (!bool.TryParse(databaseSection["TrustServerCertificate"], out trustServerCertificate))
        {
            throw new ConfigurationErrorsException("The 'TrustServerCertificate' is invalid or empty in the 'Database' section.");
        }

        if (!bool.TryParse(databaseSection["IntegratedSecurity"], out integratedSecurity))
        {
            throw new ConfigurationErrorsException("The 'IntegratedSecurity' is invalid or empty in the 'Database' section.");
        }

        // Validate critical fields
        if (string.IsNullOrWhiteSpace(server))
            throw new ConfigurationErrorsException("The 'Server' is missing or empty in the 'Database' section.");

        if (string.IsNullOrWhiteSpace(database))
            throw new ConfigurationErrorsException("The 'Database' is missing or empty in the 'Database' section.");

        // If not using Integrated Security, User ID and Password are required
        if (!integratedSecurity)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
                throw new ConfigurationErrorsException("Database credentials (UserId or Password) are missing or empty in the 'Database' section when IntegratedSecurity is false.");
        }

        // Build the connection string using SqlConnectionStringBuilder
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            IntegratedSecurity = integratedSecurity,
            TrustServerCertificate = trustServerCertificate // Set the parsed boolean value
        };

        // Only add User ID and Password if not using Integrated Security
        if (!integratedSecurity)
        {
            builder.UserID = userId;
            builder.Password = password;
        }

        return builder.ConnectionString;
    }
}
