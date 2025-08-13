using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using CommandLine;

using xpTURN.Common;
using static xpTURN.Common.FileUtils;

namespace xpTURN
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

            //
            Stopwatch timer = new Stopwatch(); timer.Start();

            try
            {
                Arguments ARGs = new Arguments();
                var result = Parser.Default.ParseArguments<Arguments>(args).WithParsed(o => ARGs = o)
                    .WithNotParsed(errors =>
                    {
                        Environment.Exit(1);
                    });

                //
                ignoreFiles.Add("^~$*"); // Exclude Excel backup files
                ignoreFiles.Add(@"^#.*"); // Exclude files

                ignoreFolders.Add(@"\[Result\]"); // Exclude Result files

                string searchPattern = ARGs.InputType?.ToLower() switch
                {
                    "xls" => "*.xls*",
                    "proto" => "*.proto",
                    _ => "*.xls*"
                };
                List<string> searchPatterns = new List<string> { searchPattern };

                inputFiles = GetTartgetFile(new List<string> { ARGs.Input }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

                if (string.IsNullOrEmpty(ARGs.ImportPath))
                {
                    importPaths.Add(Directory.GetCurrentDirectory());
                }
                else
                {
                    importPaths.Add(ARGs.ImportPath);
                }

                importPaths.Add(Path.GetDirectoryName(typeof(Program).Assembly.Location));

                var options = new Dictionary<string, string>
                {
                    { "Namespace", ARGs.Namespace },
                    { "TableSetName", ARGs.TableSetName },
                    { "Output", ARGs.Output },
                    { "OutputSplit", ARGs.OutputSplit.ToString() },
                    { "OutputComment", ARGs.OutputComment.ToString() },
                    { "OutputType", ARGs.OutputType },
                    { "ForDataTable", ARGs.ForDataTable.ToString() },
                };

                //
                Logger.Log.Info("CENVERT START");
                Logger.Log.Info("------------------------------------------------------");

                if (ARGs.InputType == "xls")
                    xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);
                else if (ARGs.InputType == "proto")
                    xpTURN.ProtoGen.ProtoGen.DoGenerate(inputFiles, importPaths, options);

                //
                Logger.Log.Info("------------------------------------------------------");
                Logger.Log.Info($"CONVERT END");
                Logger.Log.Info("------------------------------------------------------");
                Logger.Log.Info("");
            }
            catch (Exception e)
            {
                //
                Logger.Log.Error(e.Message);
                Logger.Log.Error(e.StackTrace);

                //
                Environment.Exit(1);
            }

            Logger.Log.Info($"TableGen Time : {timer.ElapsedMilliseconds / 1000f}s");

            Environment.Exit(Logger.Log.Tool.Count() != 0 ? 1 : 0);
        }
    }
}
