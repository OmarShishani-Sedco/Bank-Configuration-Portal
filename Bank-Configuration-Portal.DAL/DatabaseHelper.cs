using Microsoft.Data.SqlClient;

public static class DatabaseHelper
{
    public static SqlConnection GetConnection()
    {
        return new SqlConnection(AppConfig.GetConnectionString());
    }
}
