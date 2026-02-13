using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace xpTURN.Common
{
    public class FileUtils
    {
        /// <summary>
        /// Matches the file name against ignore patterns. Patterns are interpreted as regular expressions.
        /// </summary>
        private static bool IsIgnoreFile(string path, IList<string> ignoreFiles)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var fileName = Path.GetFileName(path);
            if (ignoreFiles == null) return false;
            return ignoreFiles.Any(pattern => Regex.IsMatch(fileName, pattern));
        }

        /// <summary>
        /// Matches the file's directory path against ignore patterns. Patterns are interpreted as regular expressions.
        /// </summary>
        /// <param name="path">Full path of the file (directory part is used for matching).</param>
        /// <param name="ignoreFolders">Regex patterns to match against the directory path. Null is treated as no ignore.</param>
        private static bool IsIgnoreFolder(string path, IList<string> ignoreFolders)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var dir = Path.GetDirectoryName(path);
            if (ignoreFolders == null) return false;
            return ignoreFolders.Any(pattern => Regex.IsMatch(dir ?? "", pattern));
        }

        /// <summary>
        /// Enumerates files under the given paths that match search patterns and are not excluded by ignore rules.
        /// </summary>
        /// <param name="paths">Directories to search.</param>
        /// <param name="searchPatterns">File search patterns (e.g. "*.xlsx"). Used by <see cref="Directory.EnumerateFiles"/>.</param>
        /// <param name="searchOption">TopDirectoryOnly or AllDirectories.</param>
        /// <param name="ignoreFiles">Regex patterns matched against file names. Matching files are excluded. Null = no file ignore.</param>
        /// <param name="ignoreFolders">Regex patterns matched against directory paths. Files under matching folders are excluded. Null = no folder ignore.</param>
        public static List<string> GetTargetFile(IList<string> paths, IList<string> searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly, IList<string> ignoreFiles = null, IList<string> ignoreFolders = null)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));
            if (searchPatterns == null)
                throw new ArgumentNullException(nameof(searchPatterns));

            var targetFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                if (Directory.Exists(path) == false)
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Target Directory does not exist: {path}");
                    continue;
                }

                foreach (var searchPattern in searchPatterns)
                {
                    foreach (var filePath in Directory.EnumerateFiles(path, searchPattern, searchOption))
                    {
                        if (IsIgnoreFile(filePath, ignoreFiles) || IsIgnoreFolder(filePath, ignoreFolders))
                        {
                            continue;
                        }

                        targetFiles.Add(filePath);
                    }
                }
            }

            return targetFiles.ToList();
        }
    }
}
