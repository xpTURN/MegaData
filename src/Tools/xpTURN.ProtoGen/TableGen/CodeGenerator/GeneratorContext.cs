using System;
using System.IO;
using System.Collections.Generic;

namespace xpTURN.TableGen
{
    public static class GeneratorOptions
    {
        public static string GetCustomOption(this Dictionary<string, string> dict, string key)
        {
            if (dict == null || !dict.TryGetValue(key, out var value))
            {
                return string.Empty;
            }
            return value;
        }

        public static bool IsEnabled(this Dictionary<string, string> dict, string key, bool defaultIfMissing = false)
            => IsEnabledValue(dict.GetCustomOption(key), defaultIfMissing);
        
        private static bool IsEnabledValue(string option, bool defaultIfMissing = false)
        {
            if (string.IsNullOrWhiteSpace(option)) return defaultIfMissing;

            option = option.Trim();
            if (option == "1") return true;
            if (string.Equals("yes", option, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals("true", option, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals("on", option, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
    
    public class GeneratorContext : IDisposable
    {
        private readonly Dictionary<string, string> _options;

        public string GetCustomOption(string key) => _options.GetCustomOption(key);
        public bool IsEnabled(string key, bool defaultIfMissing = false) => _options.IsEnabled(key, defaultIfMissing);

        public string IndentToken { get; } = "    "; // 4 spaces
        public int IndentLevel { get; private set; }

        public TextWriter Output { get; }

        public GeneratorContext(TextWriter output, Dictionary<string, string> options, string indentToken = "    ")
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            IndentToken = indentToken ?? throw new ArgumentNullException(nameof(indentToken));
            IndentLevel = 0;
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public GeneratorContext WriteLine()
        {
            Output.WriteLine();
            return this;
        }

        public GeneratorContext WriteLine(string line)
        {
            var indentLevel = IndentLevel;
            var target = Output;
            while (indentLevel-- > 0)
            {
                target.Write(IndentToken);
            }
            target.WriteLine(line);
            return this;
        }

        public TextWriter Write(string value)
        {
            var indentLevel = IndentLevel;
            var target = Output;
            while (indentLevel-- > 0)
            {
                target.Write(IndentToken);
            }
            target.Write(value);
            return target;
        }

        public GeneratorContext Indent()
        {
            IndentLevel++;
            return this;
        }

        public GeneratorContext Outdent()
        {
            IndentLevel--;
            return this;
        }

        public void Dispose()
        {
            Output?.Flush();
            Output?.Close();
        }
    }
}
