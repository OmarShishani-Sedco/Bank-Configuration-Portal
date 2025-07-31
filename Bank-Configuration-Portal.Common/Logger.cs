using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Common
{
    public class ErrorLog
    {
        public string ErrorTime { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    public static class Logger
    {
        private static readonly string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string logFilePath = Path.Combine(logDirectory, "error_log.json");

        static Logger()
        {
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
        }

        public static void LogError(Exception ex, string context = "")
        {
            var log = new ErrorLog
            {
                ErrorTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Message = $"[{context}] {ex.Message}",
                StackTrace = ex.StackTrace ?? "No stack trace available"
            };

            string json = JsonConvert.SerializeObject(log, Formatting.Indented);
            File.AppendAllText(logFilePath, json + "," + Environment.NewLine);
        }
    }
}
