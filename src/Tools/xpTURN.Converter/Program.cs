using System;
using System.Collections.Generic;
using System.IO;

using CommandLine;
using xpTURN.Common;
using xpTURN.Tool.Common;
using static xpTURN.Common.FileUtils;
using xpTURN.MegaData;

namespace xpTURN.Converter
{
    public class Program
    {
        static List<string> inputFiles = new List<string>(); // {XLS_FILES}, {PROTO_FILES}
        static List<string> importPaths = new List<string>(); // {PATH}
        static List<string> ignoreFiles = new List<string>(); // {IGNORE_FILES}
        static List<string> ignoreFolders = new List<string>(); // {IGNORE_FOLDERS}

        static void Main(string[] args)
        {
            //
            Logger.Log.SetLogger(new xpTURN.Tool.Common.Log(typeof(Program)), true);
            JsonWrapper.FromJsonMethod = JsonUtils.FromJson;
            JsonWrapper.ToJsonMethod = JsonUtils.ToJson;
            try
            {
                Arguments ARGs = new Arguments();
                var result = Parser.Default.ParseArguments<Arguments>(args).WithParsed(o => ARGs = o).WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });

                //
                Logger.Log.Info("------------------------------------------------------");
                Logger.Log.Info($"Input Directory: {ARGs.Input}");
                Logger.Log.Info($"Output Directory: {ARGs.Output}");

                //
                Logger.Log.Info("------------------------------------------------------");
                Logger.Log.Info($"Loading Assemblies, for Table Type Discovery...");
                AssemblyUtils.LoadAllDependencies();

                //
                ignoreFiles.Add("^~$*"); // Exclude Excel backup files
                ignoreFiles.Add(@"^#.*"); // Exclude files
                ignoreFiles.Add(@"^Subset.json"); // Exclude Subset JSON files

                //
                ignoreFolders.Add(@"\[Define\]"); // Exclude Define files
                ignoreFolders.Add(@"\[Result\]"); // Exclude Result files

                List<string> searchPatterns = new List<string>();
                searchPatterns.Add("*.xls*"); // Default Search Pattern for Excel files
                searchPatterns.Add("*.json"); // Default Search Pattern for JSON files

                inputFiles = GetTartgetFile(new List<string> { ARGs.Input }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

                var subsetDataTable = SubsetDataTable.Load(Path.Combine(ARGs.Input, "Subset.json"));
            
                Converter.Default.DoConvert(inputFiles, ARGs, subsetDataTable);
            }
            catch (Exception e)
            {
                //
                Logger.Log.Error(e.Message);
                Logger.Log.Error(e.StackTrace);

                //
                Environment.Exit(1);
            }

            Environment.Exit(Logger.Log.Tool.Count() != 0 ? 1 : 0);
        }
    }
}
