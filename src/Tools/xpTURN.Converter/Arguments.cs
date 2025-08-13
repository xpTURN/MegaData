using System;
using CommandLine;

namespace xpTURN.Converter
{
    public class Arguments
    {
        [Option("input", Required = true, HelpText = "Folder containing data files.")]
        public string Input { get; set; }

        [Option("output", Required = true, HelpText = "Folder to save converted results.")]
        public string Output { get; set; }

        [Option("namespace", Required = true, HelpText = "Namespace where TableSet is defined.")]
        public string Namespace { get; set; }

        [Option("tableset", Required = true, HelpText = "TableSet class name.")]
        public string TableSetName { get; set; }

        [Option("target-date", Required = false, HelpText = "Convert target date. Used as the reference date for including data.")]
        public DateTime TargetDate { get; set; }

        public Arguments()
        {
            Input = string.Empty;
            Output = string.Empty;
            TargetDate = DateTime.MinValue;
        }
    }
}
