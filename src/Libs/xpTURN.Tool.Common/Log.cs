using System;
using System.IO;
using System.Reflection;

namespace xpTURN.Tool.Common
{
    public class Log : xpTURN.Common.ILog, IDisposable
    {
        private StreamWriter _streamWriter;

        public Log(Type type)
        {
            Open($"{GetExecutableDirectory()}/Logs/{type.FullName}.{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log");
        }

        public static string GetExecutablePath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        public static string GetExecutableDirectory()
        {
            return Path.GetDirectoryName(GetExecutablePath());
        }

        public void Open(string fileName)
        {
            string dirName = Path.GetDirectoryName(fileName);
            if (dirName != null && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // Open the file for writing
            FileStream fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _streamWriter = new StreamWriter(fs);
        }

        public void Close()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Flush();
                _streamWriter.Close();
                _streamWriter = null;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Debug(string message)
        {
            _streamWriter.WriteLine($"DEBUG: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"DEBUG: - {message}");
        }

        public void Info(string message)
        {
            _streamWriter.WriteLine($"INFO: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"INFO: - {message}");
        }

        public void Warn(string message)
        {
            _streamWriter.WriteLine($"WARN: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"WARN: - {message}");
        }

        public void Error(string message)
        {
            _streamWriter.WriteLine($"ERROR: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"ERROR: - {message}");
        }

        public void Fatal(string message)
        {
            _streamWriter.WriteLine($"FATAL: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"FATAL: - {message}");
        }
    }
}
