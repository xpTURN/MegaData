using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace xpTURN.Common
{
    public class FileUtils
    {
        private static bool IsIgnoreFile(string path, List<string> ignoreFiles)
        {
            var fileName = Path.GetFileName(path);
            if (ignoreFiles == null) return false;
            return ignoreFiles.Any(pattern => Regex.IsMatch(fileName, pattern));
        }

        public static bool IsIgnoreFolder(string path, List<string> ignoreFolders)
        {
            var dir = Path.GetDirectoryName(path);
            if (ignoreFolders == null) return false;
            return ignoreFolders.Any(pattern => Regex.IsMatch(dir, pattern));
        }

        public static List<string> GetTartgetFile(IList<string> paths, IList<string> searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly, List<string> ignoreFiles = null, List<string> ignoreFolders = null)
        {
            List<string> targetFiles = new List<string>();
            foreach (var path in paths)
            {
                if (System.IO.Directory.Exists(paths[0]) == false)
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

                        // Add file to targetFiles if it is not ignored
                        targetFiles.Add(filePath);
                    }
                }
            }

            return targetFiles;
        }
    }
}
