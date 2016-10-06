using System;
using System.Globalization;
using System.IO;
using TextWriter = Core.Libs.FileSystem.TextWriter;

namespace Core.Libs
{
    public static class Log
    {
        public static void LogMessage(string message, int indentLever = 0)
        {
            var indent = new string(' ', indentLever * 2);
            // var currentDir = Directory.GetCurrentDirectory();

            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var currentDir = Path.GetDirectoryName(path);

            var now = DateTime.Now;
            var logFile = "log-" + now.Year + now.Month + ".txt";

            TextWriter.AppendToTextFile($"{now}:  {indent + message}", currentDir, logFile);
        }

        public static string LogException(Exception ex, int indentLever = 0)
        {
            try
            {
                var indent = new string(' ', indentLever * 2);

                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var currentDir = Path.GetDirectoryName(path);

                var now = DateTime.Now;
                var indent2 = new string(' ', now.ToString(CultureInfo.InvariantCulture).Length + indentLever * 2);
                var logFile = "log-" + now.Year + now.Month + ".txt";

                TextWriter.AppendToTextFile($"{now}:  {indent + "Error!"}", currentDir, logFile);
                TextWriter.AppendToTextFile(indent2 + $"  Message: {indent + ex.Message}", currentDir, logFile);
                if (ex.InnerException != null)
                {
                    TextWriter.AppendToTextFile(indent2 + $"  Inner Message: {indent + ex.InnerException.Message}", currentDir, logFile);
                }
                TextWriter.AppendToTextFile(indent2 + $"  Method: {indent + ex.TargetSite}", currentDir, logFile);
                return Path.Combine(currentDir, logFile);
            }
            catch (Exception)
            {
                // eat the exception
                return null;
            }
        }
    }
}
