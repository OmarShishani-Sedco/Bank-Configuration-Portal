using System;
using System.Configuration;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace Bank_Configuration_Portal.Common
{
    public static class WindowsEventLogger
    {
        private static readonly bool Enabled =
            bool.TryParse(ConfigurationManager.AppSettings["WinEventLog.Enabled"], out var on) && on;

        private static readonly string LogName =
            ConfigurationManager.AppSettings["WinEventLog.LogName"] ?? "Application";

        private static readonly string Source =
            ConfigurationManager.AppSettings["WinEventLog.Source"] ?? "BankConfigurationPortal";

        private static readonly int BaseEventId =
            int.TryParse(ConfigurationManager.AppSettings["WinEventLog.BaseEventId"], out var id) ? id : 9000;

        
        public static void TryEnsureSource()
        {
            if (!Enabled) return;

            try
            {
                if (!EventLog.SourceExists(Source))
                {
                    var scd = new EventSourceCreationData(Source, LogName);
                    EventLog.CreateEventSource(scd);
                    Logger.LogInfo($"Created Windows Event Log ({LogName}:{Source}) source successfully!", "WindowsEventLog_Source_Creation");
                }
                else
                {
                    Logger.LogInfo($"Windows Event Log ({LogName}:{Source}) source already exists.", "WindowsEventLog_Source_Creation");
                }

            }
            catch (SecurityException ex)
            {
                Logger.LogError(ex,"WindowsEventLog_Source_Creation");
            }
            catch
            {}
        }

        private static string FormatExceptionForEventLog(Exception ex, string context)
        {
            var prefix = string.IsNullOrWhiteSpace(context) ? "" :
                        $"The exception originated from the following (method/operation) : [{context}] ";

            var sb = new StringBuilder();
            sb.AppendLine($"{prefix}{ex.Message}");

            int depth = 0;
            for (var e = ex; e != null; e = e.InnerException, depth++)
            {
                sb.AppendLine(depth == 0 ? "Exception:" : $"Inner Exception {depth}:");
                sb.AppendLine($"{e.GetType().FullName}");
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(e.StackTrace ?? "No stack trace available");
                if (e.InnerException != null) sb.AppendLine();
            }
            return sb.ToString();
        }

        public static void WriteError(Exception ex, string context = null, int eventIdOffset = 1)
        {
            if (!Enabled || ex == null) return;
            try
            {
                var payload = FormatExceptionForEventLog(ex, context);
                Write(payload, EventLogEntryType.Error, BaseEventId + eventIdOffset);
            }
            catch { }
        }

        public static void WriteError(string message, int eventIdOffset = 1)
            => Write(message, EventLogEntryType.Error, BaseEventId + eventIdOffset);

        public static void WriteWarning(string message, int eventIdOffset = 2)
            => Write(message, EventLogEntryType.Warning, BaseEventId + eventIdOffset);

        public static void WriteInfo(string message, int eventIdOffset = 3)
            => Write(message, EventLogEntryType.Information, BaseEventId + eventIdOffset);

        private static void Write(string message, EventLogEntryType type, int eventId)
        {
            if (!Enabled) return;

            try
            {
                const int Max = 30000; // Application log supports up to ~31k chars
                var payload = (message?.Length > Max) ? message.Substring(0, Max) : message;

                EventLog.WriteEntry(Source, payload ?? string.Empty, type, eventId);
            }
            catch
            {}
        }
    }
}
