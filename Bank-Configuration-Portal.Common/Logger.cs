using Newtonsoft.Json;
using System;
using System.IO;

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
            try
            {
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);
            }
            catch {}
        }

        public static void LogError(Exception ex, string context = "")
        {
            string contextPrefix = string.IsNullOrWhiteSpace(context) ? "" : $"[{context}] ";

            var log = new ErrorLog
            {
                ErrorTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Message = $"{contextPrefix}{ex.Message}",
                StackTrace = ex.StackTrace ?? "No stack trace available"
            };

            // Write JSON file
            try
            {
                string json = JsonConvert.SerializeObject(log, Formatting.Indented);
                File.AppendAllText(logFilePath, json + "," + Environment.NewLine);
            }
            catch {}

            // Mirror to Windows Event Log (Error)
            try
            {
                WindowsEventLogger.WriteError(ex, context);
            }
            catch {}
        }

        public static void LogWarning(string message, string context = "")
        {
            string contextPrefix = string.IsNullOrWhiteSpace(context) ? "" : $"[{context}] ";

            try
            {
                var log = new
                {
                    TimeUtc = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Level = "Warning",
                    Context = context,
                    Message = message
                };
                var json = JsonConvert.SerializeObject(log, Formatting.Indented);
                File.AppendAllText(logFilePath, json + "," + Environment.NewLine);
            }
            catch { }

            try
            {
                WindowsEventLogger.WriteWarning($"{contextPrefix}{message}");
            }
            catch { }
        }

        public static void LogInfo(string message, string context = "")
        {
            string contextPrefix = string.IsNullOrWhiteSpace(context) ? "" : $"[{context}] ";

            try
            {
                var log = new
                {
                    TimeUtc = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Level = "Info",
                    Context = context,
                    Message = message
                };
                var json = JsonConvert.SerializeObject(log, Formatting.Indented);
                File.AppendAllText(logFilePath, json + "," + Environment.NewLine);
            }
            catch { }

            try
            {
                WindowsEventLogger.WriteInfo($"{contextPrefix}{message}");
            }
            catch { }
        }
    }
}
