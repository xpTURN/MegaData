using System;

using CommandLine;

namespace xpTURN
{
    public class Arguments
    {
        [Option("import", Required = false, HelpText = "Specifies the folder path containing external proto definition files. (e.g., .proto files)")]
        public string ImportPath { get; set; }

        [Option("input", Required = true, HelpText = "Specifies the folder path containing proto definition files. (e.g., .xls, .proto files)")]
        public string Input { get; set; }

        [Option("input-type", Required = false, HelpText = "Input type of the proto definition files. (e.g., xls, proto - default is xls)")]
        public string InputType { get; set; }

        [Option("output", Required = true, HelpText = "Specifies the folder path where the generated code will be saved.")]
        public string Output { get; set; }

        [Option("output-type", Required = true, HelpText = "Output type. (e.g., cs, proto - default is cs;proto)")]
        public string OutputType { get; set; }

        [Option("output-split", Required = false, HelpText = "Saves output files separately for Data, Table, and Enum types. (default is false)")]
        public bool OutputSplit { get; set; }

        [Option("output-comment", Required = false, HelpText = "Includes comments in the output files. (default is true)")]
        public bool OutputComment { get; set; }

        [Option("namespace", Required = true, HelpText = "Specifies the namespace where the TableSet class is defined.")]
        public string Namespace { get; set; }

        [Option("tableset", Required = true, HelpText = "Specifies the TableSet class name.")]
        public string TableSetName { get; set; }

        [Option("for-datatable", Required = false, HelpText = "Generates source code for DataTable usage. (default is false)")]
        public bool ForDataTable { get; set; }

        public Arguments()
        {
            Input = ".";
            InputType = "xls";

            Output = ".";
            OutputType = "cs,proto";
            OutputSplit = false;
            OutputComment = true;

            Namespace = "MySpace";
            TableSetName = "MyTableSet";
            ForDataTable = false;
        }
    }
}
