using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace xpTURN.Common
{
    public class DebugInfo
    {
        public static DebugInfo Empty { get; } = new DebugInfo();
    
        public string File { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
    }

    public interface ILog
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
    }

    public interface IAppender
    {
        public int GetEventsCount(Logger.Level level);

        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
    }

    public class LoggingEvent
    {
        public Logger.Level Level { get; }
        public string RenderedMessage { get; }
        public LoggingEvent(Logger.Level level, string message)
        {
            Level = level;
            RenderedMessage = message;
        }
    }

    public class MemoryAppender : IAppender
    {
        private readonly List<LoggingEvent> _events = new List<LoggingEvent>();
        private void AddEvent(Logger.Level level, string message) => _events.Add(new LoggingEvent(level, message));

        public int GetEventsCount(Logger.Level level) => _events.Count(e => e.Level == level);
        public List<LoggingEvent> GetEvents() => _events;

        public void Debug(string message) => AddEvent(Logger.Level.Debug, message);
        public void Info(string message) => AddEvent(Logger.Level.Info, message);
        public void Warn(string message) => AddEvent(Logger.Level.Warn, message);
        public void Error(string message) => AddEvent(Logger.Level.Error, message);
        public void Fatal(string message) => AddEvent(Logger.Level.Fatal, message);
    }

    public class Logger
    {
        public enum Level
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        private ILog _log;
        private bool _enableIndent = false;

        private List<IAppender> _appenders = new List<IAppender>();
        private MemoryAppender _memoryAppender = null;

        public string IndentToken { get; } = "    "; // 4 spaces for indentation
        public int IndentLevel { get; private set; }

        public ToolLogger Tool { get; } = new ToolLogger();

        public static Logger Log { get; } = new Logger();

        public void SetLogger(ILog log, bool enableIndent = false)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log), "Log cannot be null.");
            }

            _log = log;
            _enableIndent = enableIndent;
        }

        private Logger()
        {
            _log = new DummyLog();
        }

        public void Indent()
        {
            if (!_enableIndent) return;
            IndentLevel++;
        }

        public void Outdent()
        {
            if (!_enableIndent) return;
            IndentLevel--;
        }

        public void CaptureStart()
        {
            if (_memoryAppender == null)
            {
                _memoryAppender = new MemoryAppender();
                _appenders.Add(_memoryAppender);
            }
        }

        public List<LoggingEvent> CaptureEnd()
        {
            if (_memoryAppender == null)
            {
                throw new InvalidOperationException("CaptureStart must be called before CaptureEnd.");
            }

            var events = _memoryAppender.GetEvents();
            _appenders.Remove(_memoryAppender);
            _memoryAppender = null; // Reset the memory appender after capturing

            return events;
        }

        [Conditional("DEBUG")]
        public void Debug(string message)
        {
            if (IndentLevel != 0)
                message = message.PadLeft(message.Length + IndentLevel * IndentToken.Length);

            _appenders.ForEach(appender => appender.Debug(message));
            _log.Debug(message);
        }

        public void Info(string message)
        {
            if (IndentLevel != 0)
                message = message.PadLeft(message.Length + IndentLevel * IndentToken.Length);

            _appenders.ForEach(appender => appender.Info(message));
            _log.Info(message);
        }

        public void Warn(string message)
        {
            if (IndentLevel != 0)
                message = message.PadLeft(message.Length + IndentLevel * IndentToken.Length);

            _appenders.ForEach(appender => appender.Warn(message));
            _log.Warn(message);
        }

        public void Error(string message)
        {
            if (IndentLevel != 0)
                message = message.PadLeft(message.Length + IndentLevel * IndentToken.Length);

            _appenders.ForEach(appender => appender.Error(message));
            _log.Error(message);
        }

        public void Fatal(string message)
        {
            if (IndentLevel != 0)
                message = message.PadLeft(message.Length + IndentLevel * IndentToken.Length);

            _appenders.ForEach(appender => appender.Fatal(message));
            _log.Fatal(message);
        }

        public class ToolLogger
        {
            private List<string> sErrors { get; } = new List<string>();

            private string sFile{ get; set; } = string.Empty;
            private string sLine { get; set; } = string.Empty;

            public string GetFile() => sFile;
            public string GetLine() => sLine;

            public string GetFileLine()
            {
                if (string.IsNullOrEmpty(sFile) && string.IsNullOrEmpty(sLine))
                    return string.Empty;
                else if (string.IsNullOrEmpty(sLine))
                    return sFile;
                else
                    return $"{sFile}({sLine})";
            }

            internal ToolLogger()
            {
            }

            public void Indent()
            {
                Logger.Log.Indent();
            }

            public void Outdent()
            {
                Logger.Log.Outdent();
            }

            public void File(string file, string line)
            {
                if (!string.IsNullOrEmpty(file))
                    sFile = file;
                else
                    sFile = string.Empty;

                if (!string.IsNullOrEmpty(line))
                    sLine = line;
                else
                    sLine = string.Empty;
            }

            public void File(string file) => File(file, string.Empty);

            public void File(DebugInfo debugInfo) => File(debugInfo?.File ?? string.Empty, debugInfo?.Line ?? string.Empty);

            public void Line(string line) => sLine = line;


            public void Debug(string message) => Write(Level.Debug, sFile, sLine, message);

            public void Info(string message) => Write(Level.Info, sFile, sLine, message);

            public void Warn(string message) => Write(Level.Warn, sFile, sLine, message);

            public void Error(string message) => Write(Level.Error, sFile, sLine, message);

            public void Fatal(string message) => Write(Level.Fatal, sFile, sLine, message);

            public void Debug(DebugInfo debugInfo, string message) => Write(Level.Debug, debugInfo?.File, debugInfo?.Line, message);

            public void Info(DebugInfo debugInfo, string message) => Write(Level.Info, debugInfo?.File, debugInfo?.Line, message);

            public void Warn(DebugInfo debugInfo, string message) => Write(Level.Warn, debugInfo?.File, debugInfo?.Line, message);

            public void Error(DebugInfo debugInfo, string message) => Write(Level.Error, debugInfo?.File, debugInfo?.Line, message);

            public void Fatal(DebugInfo debugInfo, string message) => Write(Level.Fatal, debugInfo?.File, debugInfo?.Line, message);

            private void Write(Level level, string file, string line, string message)
            {
                string messageLine = message;
                if (!string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(line))
                    messageLine = $"{file}(Line: {line}) -> {message}";
                else if (!string.IsNullOrEmpty(line))
                    messageLine = $"{sFile} -> {message}";

                switch (level)
                {
                    case Level.Debug:
                        Logger.Log.Debug(messageLine);
                        break;
                    case Level.Info:
                        Logger.Log.Info(messageLine);
                        break;
                    case Level.Warn:
                        Logger.Log.Warn(messageLine);
                        break;
                    case Level.Error:
                        sErrors.Add(messageLine);
                        Logger.Log.Error(messageLine);
                        break;
                    case Level.Fatal:
                        sErrors.Add(messageLine);
                        Logger.Log.Fatal(messageLine);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(level), level, null);
                }
            }

            public void Clear()
            {
                sFile = string.Empty;
                sLine = string.Empty;
                sErrors.Clear();
            }

            public  int Count() => sErrors.Count;

            public void Summary()
            {
                if (Count() != 0)
                {
                    Logger.Log.Error("");
                    Logger.Log.Error($"ERROR COUNT {Count()}");
                    Logger.Log.Error("");
                    sErrors.ForEach(text => Logger.Log.Error(text));
                    Logger.Log.Error("");
                }
            }
        }
    }

    internal class DummyLog : ILog
    {
        public void Debug(string message) { }
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
        public void Fatal(string message) { }
    }
}