using Serilog;

namespace Project.SharedLibrary.Logs
{
    public static class LogExceptions
    {
        public static void LogException(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }

        public static void LogToDebugger(string message) => Log.Debug(message);
        public static void LogToConsole(string message) => Log.Warning(message);
        public static void LogToFile(string message) => Log.Information(message);
    }
}
