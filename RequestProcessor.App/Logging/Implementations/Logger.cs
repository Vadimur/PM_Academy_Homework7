using System;

namespace RequestProcessor.App.Logging.Implementations
{
    internal class Logger : ILogger
    {
        public void Log(string message)
        {
            if (IsValidMessage(message))
            {
                System.Diagnostics.Debug.WriteLine($"Logged message: {message}");
            }
        }

        public void Log(Exception exception, string message)
        {
            if (exception != null)
            {
                System.Diagnostics.Debug.WriteLine($"Logged error: {exception.Message}\n");
            }

            if (IsValidMessage(message))
            {
                System.Diagnostics.Debug.WriteLine($"Logged message: {message}");
            }
        }

        private bool IsValidMessage(string message)
        {
            return !string.IsNullOrEmpty(message);
        }
    }
}