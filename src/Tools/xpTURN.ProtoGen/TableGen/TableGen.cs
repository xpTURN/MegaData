using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.CodeDom.Compiler;
using System.Reflection;

using xpTURN.Common;
using xpTURN.Protobuf;
using xpTURN.MegaData;

using xpTURN.Tool.Common;
using static xpTURN.TableGen.Utils.TableGenUtils;

namespace xpTURN.TableGen
{
    public enum OPTION
    {
        DEFENDED_CLASS = 1,
        DEFENDED_ENUM = 2,
        CONTAIN_DUPLICATE = 4,
    }

    public static class TableGen
    {
        public static readonly string sTYPE = "Type";
        public static readonly string sDEFINE = "Define";
        public static readonly string sTABLE = "Table";
        public static readonly string sDATA = "Data";
        public static readonly string sENUM = "Enum";
        public static readonly string sNUM = "Num";
    
        public static readonly int sRESERVED_NUM_BEGIN = 18000;
        public static readonly int sNUM_META_NESTED_DATA = 18999; // Reserved number for MetaNestedData
        public static readonly int sRESERVED_NUM_END = 18999;

        static ExcelReader _excelFile = new ExcelReader();
        static List<TableDesc> _listTable = new List<TableDesc>();
        static Dictionary<string, TableDesc> _mapTable = new Dictionary<string, TableDesc>();

        static TableGen()
        {
            xpTURN.MegaData.JsonWrapper.FromJsonMethod = xpTURN.Tool.Common.JsonUtils.FromJson;
            xpTURN.MegaData.JsonWrapper.ToJsonMethod = xpTURN.Tool.Common.JsonUtils.ToJson;
        }

        static TableDesc GetTableDefine(string name)
        {
            if (!_mapTable.ContainsKey(name))
            {
                return null;
            }

            return _mapTable[name];
        }

        //
        static void AddTable(TableDesc table)
        {
            //
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            if (!provider.IsValidIdentifier(table.Name))
            {
                Logger.Log.Tool.Error(table.DebugInfo, $"Invaild table name, C# Language identifier : '{table.Name}'");
                return;
            }

            //
            if (_mapTable.ContainsKey(table.Name))
            {
                Logger.Log.Tool.Error(table.DebugInfo, $"Duplicated table : '{table.Name}'");
                return;
            }

            //
            if (!string.IsNullOrEmpty(table.ExtraOptions))
            {
                var opts = xpTURN.MegaData.JsonWrapper.FromJson(table.ExtraOptions, typeof(TableDesc.Options)) as TableDesc.Options;
                if (opts != null)
                {
                    table.ParsedExtraOpts = opts;
                }
                else
                {
                    Logger.Log.Tool.Error(table.DebugInfo, $"Invalid {table.Name}.ExtraOptions : '{table.ExtraOptions}'");
                }
            }

            _mapTable.Add(table.Name, table);
            _listTable.Add(table);
        }

        //
        static bool LoadExcel(string file)
        {
            //
            TableDesc table = new TableDesc();

            //
            Dictionary<int, FieldInfo> descMatching = new Dictionary<int, FieldInfo>();
            Dictionary<int, FieldInfo> fieldMatching = new Dictionary<int, FieldInfo>();

            //
            bool opened = _excelFile.OpenTableSheet(file, sDEFINE);
            if (!opened)
            {
                Logger.Log.Info($"Skip");
                return true;
            }

            //
            int lastX = _excelFile.LastX;
            int lastY = _excelFile.LastY;
            for (int lineY = 1; lineY <= lastY; ++lineY)
            {
                //
                string line = "";
                List<string> list = new List<string>();
                for (int x = 1; x <= lastX; ++x)
                {
                    var value = _excelFile.GetTrimCellString(x, lineY);
                    value = string.IsNullOrEmpty(value) ? string.Empty : value;

                    line += value + ",";
                    list.Add(value);
                }

                //
                int Num = 0;
                string name = list.Count > 1 ? list[0].Trim() : "";
                bool isDigit = int.TryParse(name, out Num);

                //
                if (string.IsNullOrEmpty(name))
                {
                }
                // Type header
                else if (name == sTYPE)
                {
                    Matching<TableDesc>(descMatching, list);
                    if (table.IsEmpty == false)
                    {
                        AddTable(table);

                        // Initialize for next table definition
                        table = new TableDesc();
                    }
                }
                // Message/Enum definition
                else if (name == sTABLE || name == sDATA || name == sENUM)
                {
                    MergeTo(descMatching, list, table);
                }
                // Field header
                else if (name == sNUM)
                {
                    Matching<FieldDesc>(fieldMatching, list);
                    if (table.IsEmpty == false)
                    {
                        AddTable(table);

                        // Initialize for next field definition
                        table = new TableDesc();
                    }
                }
                // Field definition
                else if (isDigit)
                {
                    FieldDesc field = new();
                    MergeTo(fieldMatching, list, field);
                    if (!string.IsNullOrEmpty(field.Name))
                    {
                        table.Add(field);
                    }
                }
                else
                {
                    Logger.Log.Tool.Error($"Unknown Data '{line}'");
                }
            }

            //
            if (table.IsEmpty == false)
            {
                AddTable(table);
            }

            return true;
        }

        static xpFieldTypes GetUserType(string typeName)
        {
            var table = GetTableDefine(typeName);
            if (table == null)
            {
                return xpFieldTypes.Type_Unknown;
            }

            //
            if (table.IsEnum)
            {
                return xpFieldTypes.Type_Enum;
            }

            if (table.IsMessage)
            {
                return xpFieldTypes.Type_Message;
            }

            return xpFieldTypes.Type_Unknown;
        }

        static bool GetEnumDefaultValue(string typeName, ref string Default)
        {
            var table = GetTableDefine(typeName);
            if (table == null || table.IsEmpty)
            {
                Default = string.Empty;
                return false;
            }

            if (string.IsNullOrEmpty(Default))
            {
                Default = string.Empty;
                return true;
            }

            if (Default.StartsWith($"{typeName}."))
            {
                Default = Default.Substring($"{typeName}.".Length);
            }

            if (table.FindField(Default) != null)
            {
                Default = $"{typeName}.{Default}";
                return true;
            }

            return false;
        }

        static void Defended(OPTION opts, TableDesc table, ref List<string> listDefended)
        {
            //
            var listField = table.GetListField();
            foreach (var field in listField)
            {
                if (opts.HasFlag(OPTION.DEFENDED_CLASS) && field.FValueType == xpFieldTypes.Type_Message
                    || opts.HasFlag(OPTION.DEFENDED_ENUM) && field.FValueType == xpFieldTypes.Type_Enum)
                {
                    if (listDefended.Find(item => item == field.FValueTypeName) == null)
                    {
                        listDefended.Add(field.FValueTypeName);

                        //
                        var childTable = GetTableDefine(field.FValueTypeName);
                        if (childTable == null)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Unknown Type '{field.FValueTypeName}' in {table.Name}.{field.Name}");
                            continue;
                        }

                        Defended(opts, childTable, ref listDefended);
                    }
                }
            }
        }

        static void PostProcess(bool forDataTable)
        {
            Logger.Log.Info("--------------------------------------------------------");
            Logger.Log.Info("");
            Logger.Log.Info($"[PostProcess]");
            Logger.Log.Info("");

            _listTable.Sort((a, b) =>
            {
                if (a.IsEnum && !b.IsEnum)
                {
                    return -1; // Sort so that enum tables come first
                }
                else if (!a.IsEnum && b.IsEnum)
                {
                    return 1; // Sort so that enum tables come first
                }
                return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
            });

            // Remove obsolete tables
            _listTable.RemoveAll(table => table.Obsolete == FieldObsolete.Delete);

            // Remove obsolete fields
            foreach (var table in _listTable)
            {
                var listField = table.GetListField();
                listField.RemoveAll(field => field.Obsolete == FieldObsolete.Delete);
            }

            // ExtraOptions validation
            foreach (var table in _listTable)
            {
                if (!table.IsMessage)
                    continue;

                if (table.IsTable)
                {
                    if (table.ParsedExtraOpts.WeakRef)
                    {
                        if (!table.ParsedExtraOpts.OnDemand) // WeakRef requires the OnDemand property.
                        {
                            Logger.Log.Tool.Warn(table.DebugInfo, $"Table '{table.Name}' is marked as WeakRef but OnDemand is not set.");
                            table.ParsedExtraOpts.OnDemand = true;
                        }
                    }
                }

                if (!table.IsTable)
                {
                    if (table.ParsedExtraOpts.OnDemand)
                    {
                        Logger.Log.Tool.Error(table.DebugInfo, $"Data '{table.Name}' is marked as OnDemand but is not a Table type.");
                    }

                    if (table.ParsedExtraOpts.WeakRef)
                    {
                        Logger.Log.Tool.Error(table.DebugInfo, $"Data '{table.Name}' is marked as WeakRef but is not a Table type.");
                    }
                }
            }

            // Check for reserved table names
            foreach (var table in _listTable)
            {
                if (IsReservedType(table.Name))
                {
                    Logger.Log.Tool.Error(table.DebugInfo, $"Reserved table name '{table.Name}' is not allowed.");
                }

                if (forDataTable && IsReservedTableType(table.Name))
                {
                    Logger.Log.Tool.Error(table.DebugInfo, $"Reserved table name '{table.Name}' is not allowed.");
                }
            }

            //	If the field variable type is classified as unknown, check if a table definition exists and classify as class/enum.
            //	If the type is enum, validate the default value and set the default value.
            foreach (var table in _listTable)
            {
                if (table.IsEnum)
                {
                    continue;
                }

                //	Defended Class, Enum, ParsedExtraOpts
                var listField = table.GetListField();
                foreach (var field in listField)
                {
                    //	If the field variable type is classified as unknown, check if a table definition exists and classify as class/enum.
                    if (field.FKeyType == xpFieldTypes.Type_Unknown && !string.IsNullOrEmpty(field.FKeyTypeName))
                    {
                        field.FKeyType = GetUserType(field.FKeyTypeName);
                        if (field.FKeyType == xpFieldTypes.Type_Unknown)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Unknown Type '{field.FKeyTypeName}'");
                        }
                    }

                    //	If the field variable type is classified as unknown, check if a table definition exists and classify as class/enum.
                    if (field.FValueType == xpFieldTypes.Type_Unknown && !string.IsNullOrEmpty(field.FValueTypeName))
                    {
                        field.FValueType = GetUserType(field.FValueTypeName);
                        if (field.FValueType == xpFieldTypes.Type_Unknown)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Unknown Type '{field.FValueTypeName}'");
                        }
                    }

                    //	If the type is enum, validate the default value and set the default value.
                    if (field.FValueType == xpFieldTypes.Type_Enum && field.Collections != FieldCollections.List)
                    {
                        bool result = GetEnumDefaultValue(field.FValueTypeName, ref field.Default);
                        if (result == false)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Invalid Enum default value '{table.Name}.{field.Name} = {field.Default}'");
                        }
                    }

                    //
                    if (!string.IsNullOrEmpty(field.ExtraOptions))
                    {
                        var opts = xpTURN.MegaData.JsonWrapper.FromJson(field.ExtraOptions, typeof(FieldDesc.Options)) as FieldDesc.Options;
                        if (opts != null)
                        {
                            field.ParsedExtraOpts = opts;
                        }
                        else
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Invalid {table.Name}.{field.Name}.ExtraOptions : '{field.ExtraOptions}'");
                        }
                    }
                }
            }

            //	Name validation, Field number validation and duplicate check
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            foreach (var table in _listTable)
            {
                if (!table.IsEnum && !table.IsMessage)
                {
                    continue;
                }

                Dictionary<string, FieldDesc> dicFieldByName = new Dictionary<string, FieldDesc>();
                Dictionary<int, FieldDesc> dicFieldByNum = new Dictionary<int, FieldDesc>();
                var listField = table.GetListField();
                foreach (var field in listField)
                {
                    // Name validation
                    if (!provider.IsValidIdentifier(field.Name))
                    {
                        Logger.Log.Tool.Error(field.DebugInfo, $"Invaild Field Name, C# Language identifier : '{table.Name}.{field.Name}'.");
                    }

                    // Name validation
                    if (dicFieldByName.ContainsKey(field.Name))
                    {
                        Logger.Log.Tool.Error(field.DebugInfo, $"Assigning Field Name '{table.Name}.{field.Name}' must be unique.");
                        continue;
                    }

                    // Number validation
                    if (table.IsMessage)
                    {
                        if (field.Num < 1 || 536870911 < field.Num)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Assigning Field Numbers '{table.Name}.{field.Name}' number = {field.Num} OutOfRange.");
                        }

                        if (19000 <= field.Num && field.Num <= 19999)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Assigning Field Numbers '{table.Name}.{field.Name}' number = {field.Num} Reserved Number.");
                        }

                        if (sRESERVED_NUM_BEGIN <= field.Num && field.Num <= sRESERVED_NUM_END)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Assigning Field Numbers '{table.Name}.{field.Name}' number = {field.Num} Reserved Number.");
                        }
                    }

                    // Duplicate number check
                    if (!table.IsEnum && dicFieldByNum.ContainsKey(field.Num))
                    {
                        Logger.Log.Tool.Error(field.DebugInfo, $"Assigning Field Numbers '{table.Name}.{field.Name}' number = {field.Num} Duplicated Number.");
                        continue;
                    }

                    //
                    dicFieldByName[field.Name] = field;
                    dicFieldByNum[field.Num] = field;
                }

                if (forDataTable && table.IsTable)
                {
                    // Only one enumerable NestedData field is allowed
                    var nestedDataFields = listField.FindAll(field => field.Collections == FieldCollections.Map &&
                                                            field.FValueType == xpFieldTypes.Type_Message);
                    if (nestedDataFields.Count > 1)
                    {
                        Logger.Log.Tool.Error(table.DebugInfo, $"Table '{table.Name}' has multiple Enumerable NestedData fields. Only one Enumerable NestedData field is allowed.");
                    }

                    if (nestedDataFields.Count == 0)
                    {
                        Logger.Log.Tool.Error(table.DebugInfo, $"Table '{table.Name}' must have at least one Enumerable NestedData field.");
                    }

                    nestedDataFields = listField.FindAll(field => field.Collections == FieldCollections.Map &&
                                                            field.FValueType == xpFieldTypes.Type_Message);
                    foreach (var field in nestedDataFields)
                    {
                        var keyType = field.FKeyType;
                        var dataDesc = GetTableDefine(field.FValueTypeName);
                        if (dataDesc == null)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Field '{table.Name}.{field.Name}' has a Map field with an unknown type '{field.FValueTypeName}'.");
                            continue;
                        }

                        var keyField = dataDesc.GetKeyField();
                        if (keyField == null)
                        {
                            Logger.Log.Tool.Error(field.DebugInfo, $"Field '{table.Name}.{field.Name}' has a Map field with no key field defined in the child Data '{field.FValueTypeName}'.");
                            continue;
                        }

                        var keyFieldType = keyField.FValueType;
                        if (TableTypeUtils.IsIntKeyType(keyType))
                        {
                            if (!TableTypeUtils.IsIntKeyType(keyFieldType))
                            {
                                Logger.Log.Tool.Error(field.DebugInfo, $"Field '{table.Name}.{field.Name}' has a Map field with a non-primitive key type. Only Int32, SInt32, and SFixed32 types are allowed as key types.");
                            }
                        }

                        if (keyType == xpFieldTypes.Type_Enum)
                        {
                            if (keyFieldType != xpFieldTypes.Type_Enum)
                            {
                                Logger.Log.Tool.Error(field.DebugInfo, $"Field '{table.Name}.{field.Name}' has a Map field with a non-primitive key type. Only Enum types are allowed as key types.");
                            }
                        }

                        if (keyType == xpFieldTypes.Type_String)
                        {
                            if (keyFieldType != xpFieldTypes.Type_String)
                            {
                                Logger.Log.Tool.Error(field.DebugInfo, $"Field '{table.Name}.{field.Name}' has a Map field with a non-primitive key type. Only String types are allowed as key types.");
                            }
                        }
                    }
                }
            }

            //	If the table is a nested data, set the IsNestedData property to true.
            foreach (var table in _listTable)
            {
                if (!table.IsMessage)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(table.GetNestedDataName()))
                {
                    var nestedData = _listTable.Find(item => item.Name == table.GetNestedDataName());
                    if (nestedData != null)
                    {
                        nestedData.IsNestedData = true;
                    }

                    if (forDataTable && table.IsTable)
                    {
                        FieldDesc metaNestedDataField = new FieldDesc
                        {
                            File = Logger.Log.Tool.GetFile(),
                            Num = sNUM_META_NESTED_DATA,
                            Name = "MetaNestedData",
                            FType = "MetaNestedData",
                            FValueType = xpFieldTypes.Type_Message,
                            FValueTypeName = "MetaNestedData",
                            Collections = FieldCollections.None,
                            NonProtoField = true,
                        };

                        table.Add(metaNestedDataField);
                    }
                }
            }
        }

        static bool DoProtoCodeGenerate(Dictionary<string, string> options)
        {
            Logger.Log.Info("");
            Logger.Log.Info($"[DoProtoCodeGenerate]");
            Logger.Log.Indent();
            Logger.Log.Info("");

            //	
            var dirPath = options.GetCustomOption("Output");
            if (!System.IO.Directory.Exists(dirPath))
            {
                Logger.Log.Error($"outputPath is invalid : {dirPath}");
                return false;
            }

            var nameSpace = options.GetCustomOption("Namespace");
            var tableSetName = options.GetCustomOption("TableSetName");
            var outputFile = Path.Combine(dirPath, tableSetName + ".AutoGenerated.proto");

            // 
            if (_listTable.Count == 0)
            {
                Logger.Log.Error($"No table definitions found.");
                return false;
            }

            //
            using (var ctx = new GeneratorContext(new StringWriter(), options))
            {
                var codeGenerator = new ProtoCodeGenerator();

                string md5 = codeGenerator.ExportFile(ctx, outputFile, _listTable);
                Logger.Log.Outdent();
                Logger.Log.Info("");
                Logger.Log.Info($"Save file : {outputFile}");
                Logger.Log.Info($"MD5 : {md5}");
                Logger.Log.Info("");
            }

            return true;
        }

        static bool DoSharpCodeGenerate(Dictionary<string, string> options)
        {
            Logger.Log.Info("");
            Logger.Log.Info($"[DoSharpCodeGenerate]");
            Logger.Log.Info("");

            //	
            var dirPath = options.GetCustomOption("Output");
            if (string.IsNullOrEmpty(dirPath))
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"outputPath is invalid : {dirPath}");
                return false;
            }

            if (_listTable.Count == 0)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"No table definitions found.");
                return false;
            }

            //	Generate .cs file
            var nameSpace = options.GetCustomOption("Namespace");
            var tableSetName = options.GetCustomOption("TableSetName");
            if (options.IsEnabled("OutputSplit"))
            {
                //	Generate only Table
                var listTable = _listTable.FindAll(item => item.IsTable);
                if (listTable.Count != 0)
                {
                    using (var ctx = new GeneratorContext(new StringWriter(), options))
                    {
                        var codeGenerator = new ProtobufCSharpCodeGenerator();

                        Logger.Log.Info($"> For Table");
                        Logger.Log.Indent();
                        string outputFile = Path.Combine(dirPath, tableSetName + ".Table.AutoGenerated.cs");
                        string tableMD5 = codeGenerator.ExportFile(ctx, outputFile, listTable);
                        Logger.Log.Outdent();
                        Logger.Log.Info("");
                        Logger.Log.Info($"Save file : {outputFile}");
                        Logger.Log.Info($"MD5 : {tableMD5}");
                        Logger.Log.Info("");
                    }
                }

                //	Generate only Data / SubData
                var listData = _listTable.FindAll(item => item.IsData);
                if (listData.Count != 0)
                {
                    using (var ctx = new GeneratorContext(new StringWriter(), options))
                    {
                        var codeGenerator = new ProtobufCSharpCodeGenerator();

                        Logger.Log.Info($"> For Data");
                        Logger.Log.Indent();
                        string outputFile = Path.Combine(dirPath, tableSetName + ".Data.AutoGenerated.cs");
                        string tableMD5 = codeGenerator.ExportFile(ctx, outputFile, listData);
                        Logger.Log.Outdent();
                        Logger.Log.Info("");
                        Logger.Log.Info($"Save file : {outputFile}");
                        Logger.Log.Info($"MD5 : {tableMD5}");
                        Logger.Log.Info("");
                    }
                }

                //	Generate only Enum
                var listEnum = _listTable.FindAll(item => item.IsEnum);
                if (listEnum.Count != 0)
                {
                    using (var ctx = new GeneratorContext(new StringWriter(), options))
                    {
                        var codeGenerator = new ProtobufCSharpCodeGenerator();

                        Logger.Log.Info($"> For Enum");
                        Logger.Log.Indent();
                        string outputFile = Path.Combine(dirPath, tableSetName + ".Enum.AutoGenerated.cs");
                        string enumMD5 = codeGenerator.ExportFile(ctx, outputFile, listEnum);
                        Logger.Log.Outdent();
                        Logger.Log.Info("");
                        Logger.Log.Info($"Save file : {outputFile}");
                        Logger.Log.Info($"MD5 : {enumMD5}");
                        Logger.Log.Info("");
                    }
                }
            }
            else
            {
                //	Generate all
                using (var ctx = new GeneratorContext(new StringWriter(), options))
                {
                    var codeGenerator = new ProtobufCSharpCodeGenerator();

                    Logger.Log.Info($"> For All");
                    Logger.Log.Indent();
                    string outputFile = Path.Combine(dirPath, tableSetName + ".All.AutoGenerated.cs");
                    string md5 = codeGenerator.ExportFile(ctx, outputFile, _listTable);
                    Logger.Log.Outdent();
                    Logger.Log.Info("");
                    Logger.Log.Info($"Save file : {outputFile}");
                    Logger.Log.Info($"MD5 : {md5}");
                    Logger.Log.Info("");
                }
            }

            if (options.IsEnabled("ForDataTable"))
            {
                using (var ctx = new GeneratorContext(new StringWriter(), options))
                {
                    var megaDataGenerator = new TableSetCSharpCodeGenerator();

                    Logger.Log.Info($"> For {tableSetName}TableSet");
                    Logger.Log.Indent();
                    var md5 = megaDataGenerator.ExportFile(ctx, Path.Combine(dirPath, $"{tableSetName}.TableSet.AutoGenerated.cs"), _listTable);
                    Logger.Log.Outdent();
                    Logger.Log.Info("");
                    Logger.Log.Info($"Save file : {tableSetName}.AutoGenerated.cs");
                    Logger.Log.Info($"MD5 : {md5}");
                    Logger.Log.Info("");
                }
            }

            Logger.Log.Info($"[DoSharpCodeGenerate] Done");

            return true;
        }


        public static void Clear()
        {
            Logger.Log.Tool.Clear();

            _excelFile = new ExcelReader();
            _listTable.Clear();
            _mapTable.Clear();
        }

        public static bool DoGenerate(List<string> files, Dictionary<string, string> options)
        {
            //
            Logger.Log.Info("");
            Logger.Log.Info($"[Definition File Load]");
            Logger.Log.Info("");
            foreach (var file in files)
            {
                Logger.Log.Info($"Excel file load : {file}");

                bool result = LoadExcel(file);
                if (result == false)
                {
                    Logger.Log.Error($"Excel load failed {file}");
                    Environment.Exit(1);
                    return false;
                }

                Logger.Log.Tool.File(string.Empty);
            }

            //
            PostProcess(options.IsEnabled("ForDataTable"));

            //	Skip subsequent actions if there are table-related errors
            if (Logger.Log.Tool.Count() != 0)
            {
                Logger.Log.Tool.File(string.Empty);
                Logger.Log.Info($"-------------------------------------------------------");
                Logger.Log.Tool.Summary();
                return false;
            }

            // Export the results
            options.GetCustomOption("OutputType").Split(';').ToList().ForEach(type =>
            {
                switch (type.Trim().ToLower())
                {
                    case "cs":
                        Logger.Log.Info($"[OutputType: {type}]");
                        DoSharpCodeGenerate(options);
                        break;
                    case "proto":
                        Logger.Log.Info($"[OutputType: {type}]");
                        DoProtoCodeGenerate(options);
                        break;
                    case "none":
                        Logger.Log.Info($"[OutputType: {type}]");
                        break;
                    default:
                        Logger.Log.Error($"Error outputType : {type}");
                        return;
                }
            });

            //	Skip subsequent actions if there are table-related errors
            if (Logger.Log.Tool.Count() != 0)
            {
                Logger.Log.Tool.File(string.Empty);
                Logger.Log.Info($"-------------------------------------------------------");
                Logger.Log.Tool.Summary();
                return false;
            }

            Logger.Log.Tool.File(string.Empty);
            return true;
        }
    }
}
