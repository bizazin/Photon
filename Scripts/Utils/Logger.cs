using System.Runtime.CompilerServices;
using UnityEngine;
using static Utils.Enumerators;

namespace Utils
{
    public static class Logger
    {
        private const bool EnableLogs = true;
        private const LogLevel MinimumLogLevel = LogLevel.Debug;

        public static void Log(object caller, string message, LogLevel logLevel = LogLevel.Info,
            [CallerMemberName] string method = "")
        {
            if (!EnableLogs || logLevel < MinimumLogLevel)
                return;

            var type = caller.GetType().Name;
            var logMessage = $"[{logLevel}] {type}.{method}: {message}";

            switch (logLevel)
            {
                case LogLevel.Debug:
                    Debug.Log($"<color=grey>{logMessage}</color>");
                    break;
                case LogLevel.Info:
                    Debug.Log(logMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(logMessage);
                    break;
                case LogLevel.Error:
                    Debug.LogError(logMessage);
                    break;
                case LogLevel.Critical:
                    Debug.LogError($"<color=red>{logMessage}</color>");
                    break;
            }
        }

        public static void Log(string message, LogLevel logLevel = LogLevel.Info,
            [CallerMemberName] string method = "")
        {
            if (!EnableLogs || logLevel < MinimumLogLevel)
                return;

            var logMessage = $"[{logLevel}] method:{method}: {message}";

            switch (logLevel)
            {
                case LogLevel.Debug:
                    Debug.Log($"<color=grey>{logMessage}</color>");
                    break;
                case LogLevel.Info:
                    Debug.Log(logMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(logMessage);
                    break;
                case LogLevel.Error:
                    Debug.LogError(logMessage);
                    break;
                case LogLevel.Critical:
                    Debug.LogError($"<color=red>{logMessage}</color>");
                    break;
            }
        }

        public static void LogDebug(object caller, string message, [CallerMemberName] string method = "")
            => Log(message, LogLevel.Debug, method);

        public static void LogInfo(object caller, string message, [CallerMemberName] string method = "")
            => Log(message, LogLevel.Info, method);

        public static void LogWarning(object caller, string message, [CallerMemberName] string method = "")
            => Log(caller, message, LogLevel.Warning, method);

        public static void LogError(object caller, string message, [CallerMemberName] string method = "")
            => Log(message, LogLevel.Error, method);

        public static void LogCritical(object caller, string message, [CallerMemberName] string method = "")
            => Log(message, LogLevel.Critical, method);
    }
}