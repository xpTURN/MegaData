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
            var location = Assembly.GetExecutingAssembly().Location;
            return !string.IsNullOrEmpty(location) ? location : AppContext.BaseDirectory ?? ".";
        }

        public static string GetExecutableDirectory()
        {
            var path = GetExecutablePath();
            return !string.IsNullOrEmpty(path) ? Path.GetDirectoryName(path) ?? path : (AppContext.BaseDirectory ?? ".");
        }

        public void Open(string fileName)
        {
            string dirName = Path.GetDirectoryName(fileName);
            if (dirName != null && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // Open the file for writing (StreamWriter owns and disposes the stream when leaveOpen: false)
            var fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _streamWriter = new StreamWriter(fs, leaveOpen: false);
        }

        public void Close()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Flush();
                _streamWriter.Dispose();
                _streamWriter = null;
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Debug(string message)
        {
            if (_streamWriter == null) return;
            _streamWriter.WriteLine($"DEBUG: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"DEBUG: {DateTime.Now:HH:mm:ss} - {message}");
        }

        public void Info(string message)
        {
            if (_streamWriter == null) return;
            _streamWriter.WriteLine($"INFO: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"INFO: {DateTime.Now:HH:mm:ss} - {message}");
        }

        public void Warn(string message)
        {
            if (_streamWriter == null) return;
            _streamWriter.WriteLine($"WARN: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"WARN: {DateTime.Now:HH:mm:ss} - {message}");
        }

        public void Error(string message)
        {
            if (_streamWriter == null) return;
            _streamWriter.WriteLine($"ERROR: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"ERROR: {DateTime.Now:HH:mm:ss} - {message}");
        }

        public void Fatal(string message)
        {
            if (_streamWriter == null) return;
            _streamWriter.WriteLine($"FATAL: {DateTime.Now:HH:mm:ss} - {message}");
            _streamWriter.Flush();
            Console.WriteLine($"FATAL: {DateTime.Now:HH:mm:ss} - {message}");
        }
    }
}
