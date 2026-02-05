using System;
using System.Collections.Generic;
using System.Text;

namespace SystemMonitor.Logging
{
    public class FileLogger
    {
        private readonly string logFilePath;

        public FileLogger(string logDirectory)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            logFilePath = Path.Combine(logDirectory, "monitor.log");

        }

        public void Log(LogLevel level, string component, string message)
        {
            string timestamp = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");

            string logEntry = $"[{timestamp}] [{level}] [{component}]: {message}";

            try
            {
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch
            {
                Console.Error.WriteLine("Failed to write to log file");
                Console.Error.WriteLine(logEntry);
            }

        }
    }
}
