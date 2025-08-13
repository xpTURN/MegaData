using System;
using System.IO;
using System.Collections.Generic;

using xpTURN.Common;
using xpTURN.MegaData;

using xpTURN.Tool.Common;
using static xpTURN.Converter.Utils.ConverterUtils;
using System.Linq;

namespace xpTURN.Converter
{
    // This converter processes Excel files to create data tables.
    // It reads the structure of the table from the first sheet, matches fields, and extracts data.
    // The expected structure of the Excel file is as follows:
    //
    // | A       | B               | C     | D             | E               | F     | G                  | H                | I     | J                 |
    // |         | Comments        |       |               |                 |       |                    |                  |       |                   |
    // |         | SampleDataTable | Id    | IdAlias       | {NestData1List} | Id    | IdAlias            | {NestData2List}  | Id    | IdAlias           |
    // |  Hide   | SampleData      | 1     | "IdsSample01" | NestData1       | 23    | "IdsNestData1_01"  | NestData2        | 30    | "IdsNestData2_01" |
    // |         | SampleData      | 2     | "IdsSample02" | NestData1       | 24    | "IdsNestData1_02"  | NestData2        | 31    | "IdsNestData2_02" |
    public class Converter
    {
        string _spaceName = "";
        string _tableSetName = "";
        ExcelReader _excelFile = new ExcelReader();
        DataMapping DataMapper { get; set; } = new();

        public static Converter Default { get; } = new Converter();

        private Converter()
        {
            JsonWrapper.FromJsonMethod = JsonUtils.FromJson;
            JsonWrapper.ToJsonMethod = JsonUtils.ToJson;
        }

        void Reset()
        {
            _spaceName = string.Empty;
            _tableSetName = string.Empty;
            _excelFile = new ExcelReader();
            DataMapper = new DataMapping();
        }

        string ReadString(DataCell dataCell) => ReadString(dataCell.column, dataCell.row);
        string ReadString(int cellX, int cellY) => _excelFile.GetTrimCellString(cellX, cellY);

        void MatchField(TableDesc tableDesc, int curX, int curY)
        {
            DataMapper.Clear();

            string ownerName = string.Empty;
            for (; curX <= _excelFile.LastX; ++curX)
            {
                string text = ReadString(curX, curY);
                if (string.IsNullOrEmpty(text))
                    continue;

                // Ignore comments
                if (text.StartsWith("#"))
                    continue;

                FieldDesc fieldDesc;
                if (tableDesc.Name == text)
                {
                    fieldDesc = tableDesc.NestFieldDesc;
                }
                else
                {
                    // If a new NestedData FieldDesc needs to be found
                    if (IsNestedField(text))
                        ownerName = string.Empty;

                    string regularName = RegularName(text);
                    fieldDesc = tableDesc.FindFieldDesc(ownerName, regularName);
                    if (fieldDesc == null)
                    {
                        Logger.Log.Tool.Error($"Field not found: {ExcelReader.CellName(curX, curY)} - {text}");
                        continue;
                    }
                }

                TryGetMapKey(text, out string key);
                if (!DataMapper.Add(new FieldMapping(curX, key, fieldDesc)))
                {
                    Logger.Log.Tool.Error($"Field already exists: {text}");
                }

                if (IsNestedField(text))
                {
                    // Update ownerName (Instead of ShortName)
                    ownerName = fieldDesc.FullName;
                }
            }

            var fieldMappings = DataMapper.ListFieldMapper.Values.ToList();
            var hasNested = fieldMappings.FindAll(fm => !fm.FieldDesc.OwnerIsTable && fm.FieldDesc.HasNested);
            var maxDepth = hasNested.Count > 0 ? hasNested.Max(fm => fm.FieldDesc.Depth) : 0;

            // Validate that there is only one collection field per depth
            for (int depth = 0; depth < maxDepth; ++depth)
            {
                var found = hasNested.FindAll(fm => fm.FieldDesc.IsCollection &&
                                                    fm.FieldDesc.Depth == depth);
                if (found.Count > 1)
                {
                    Logger.Log.Tool.Error($"Multiple collection fields found at depth {depth}. Only one collection field is allowed per depth.");
                }
            }

            // Validate that nested fields are collections
            foreach (var fieldMapping in hasNested)
            {
                if (!fieldMapping.FieldDesc.IsCollection)
                {
                    var found = hasNested.FindAll(fm => fm.FieldDesc.Owner == fieldMapping.FieldDesc.NestedDataDesc);
                    if (found.Count > 0)
                    {
                        Logger.Log.Tool.Error($"Nested field '{fieldMapping.FieldDesc.FullName}' is not a collection, but has nested fields. Only collection fields can have nested fields.");
                    }
                }
            }
        }

        void ReadData(int cellX, int cellY)
        {
            int firstValidX = -1;
            for (int curX = cellX; curX <= _excelFile.LastX; ++curX)
            {
                string text = ReadString(curX, cellY);
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                if (firstValidX == -1)
                {
                    firstValidX = curX;
                    DataMapper.ResetData(firstValidX);
                }

                DataMapper.SetOrAddValue(curX, text);
            }
        }

        Table LoadTableFromXls(string file, TableSet tableSet, DateTime targetDate)
        {
            //
            bool opened = _excelFile.OpenTableSheet(file, TABLE_SHEET);
            if (!opened)
            {
                Logger.Log.Info($"Skip");
                return null;
            }

            //
            string tableName = ReadString(sTABLE_NAME_CELL);
            string fullName = string.IsNullOrEmpty(_spaceName) ? $"{tableName}" : $"{_spaceName}.{tableName}";

            var type = AssemblyUtils.GetTypeByName(fullName);
            if (type == null)
            {
                Logger.Log.Tool.Error($"Type not found: {fullName}");
                return null;
            }

            //
            TableDesc tableDesc = new TableDesc(type);

            //
            Logger.Log.Info($"Import Start : TableType: {type.Name}, DataType: {tableDesc.NestedDataType.Name}");

            //
            MatchField(tableDesc, sTABLE_DESC_CELL.column, sTABLE_DESC_CELL.row);

            //
            tableDesc.CreateTable(tableSet);

            //
            for (int curY = sDATA_NAME_CELL.row; curY <= _excelFile.LastY; ++curY)
            {
                // Get DataName
                string dataName = ReadString(sDATA_NAME_CELL.column, curY);

                // Get ExcludeDataOption
                var excludeOption = ReadString(sDATA_EXCLUDE_CELL.column, curY);

                // Check ExcludeData Condition
                if (IsExcludeData(excludeOption, targetDate, dataName))
                {
                    Logger.Log.Info($"Exclude Data: {dataName} at {ExcelReader.CellName(sDATA_NAME_CELL.column, curY)} in {_excelFile.fileName}");
                    do
                    {
                        if (curY + 1 > _excelFile.LastY)
                            break;

                        dataName = ReadString(sDATA_NAME_CELL.column, curY + 1);
                        if (!string.IsNullOrEmpty(dataName))
                        {
                            break;
                        }

                        curY++;
                    } while (curY <= _excelFile.LastY);

                    continue;
                }

                // Read Data
                ReadData(sDATA_NAME_CELL.column, curY);
            }

            // Register any remaining data after the last loop
            DataMapper.SetOrAddData();

            //
            Logger.Log.Info($"import End : Table: {tableDesc.TableType.Name}");

            return tableDesc.Table;
        }

        public Table LoadTableFromJson(string file, TableSet tableSet, DateTime targetDate)
        {
            var table = JsonUtils.FromJsonFile(file) as Table;

            // Record loading source
            var debugInfo = new DebugInfo { File = file };
            SetDebugInfo(debugInfo, table);

            return table;
        }

        public bool DoConvert(List<string> files, Arguments ARGs, SubsetDataTable subsetDataTable = null)
        {
            Reset();

            //
            Logger.Log.Info("");
            Logger.Log.Info("------------------------------------------------------");
            Logger.Log.Info("CENVERT START");
            Logger.Log.Info("------------------------------------------------------");
            Logger.Log.Info("");

            // Validate input files
            _spaceName = ARGs.Namespace;
            _tableSetName = ARGs.TableSetName;
            string fullName = string.IsNullOrEmpty(_spaceName) ? $"{_tableSetName}" : $"{_spaceName}.{_tableSetName}";

            var tableSet = GetTableSet(fullName);
            if (tableSet == null)
            {
                return false;
            }

            // Set to false for design-time processing
            tableSet.Reset(false);

            Logger.Log.Info($"TableSet Type : {fullName}");
            Logger.Log.Info($"");
            Logger.Log.Info("------------------------------------------------------");
            Logger.Log.Info($"");

            //
            foreach (var file in files)
            {
                // Process each file here
                Logger.Log.Info($"Processing file : {file}");
                Logger.Log.Indent();

                //
                Table table = null;
                if (Path.GetExtension(file).Contains(".xls"))
                    table = LoadTableFromXls(file, tableSet, ARGs.TargetDate);
                else if (Path.GetExtension(file).Contains("json"))
                    table = LoadTableFromJson(file, tableSet, ARGs.TargetDate);

                if (table == null)
                {
                    Logger.Log.Tool.File(string.Empty);
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Failed to load table from file : {file}");
                    continue;
                }

                var tableName = table.GetType().Name;
                var orgTable = tableSet.GetTable(tableName);
                if (orgTable == null)
                {
                    tableSet.AddTable(tableName, table);
                }
                else
                {
                    Logger.Log.Info($"Table '{tableName}' already exists. Merging data.");
                    orgTable.MergeFrom(table);
                }

                Logger.Log.Outdent();
                Logger.Log.Info($"Summary: {table.GetType().Name}, DataCount: {table.GetMap().Count + table.GetMapAlias().Count}");
                Logger.Log.Info($"");
            }

            Logger.Log.Info("------------------------------------------------------");

            // Post-process the table set
            tableSet.PostProcess();

            //
            if (Logger.Log.Tool.Count() != 0)
            {
                Logger.Log.Info($"-------------------------------------------------------");
                Logger.Log.Tool.Summary();
                Logger.Log.Tool.Error(DebugInfo.Empty, "There are errors in the input files. Please check the logs.");
                return false;
            }

            // Save the TableSet (Default)
            string outputFile = Path.Combine(ARGs.Output, $"{_tableSetName}.bytes");
            bool result = tableSet.Save(outputFile, string.Empty, subsetDataTable);
            if (result == false)
            {
                Logger.Log.Info($"-------------------------------------------------------");
                Logger.Log.Tool.Summary();
                Logger.Log.Tool.Error(DebugInfo.Empty, "TableSet Save Failed.");
                return false;
            }

            // Save the TableSet (Subset)
            if (subsetDataTable != null)
            {
                foreach (var pair in subsetDataTable.Map)
                {
                    var subsetName = pair.Key;
                    var subsetTables = pair.Value.Tables;
                    if (subsetTables.Count == 0)
                    {
                        Logger.Log.Info($"No tables found for subset '{subsetName}'.");
                        continue;
                    }

                    //
                    Logger.Log.Info($"Subset: {subsetName}, Tables: {string.Join(", ", subsetTables)}");

                    string subsetFile = Path.Combine(ARGs.Output, $"{_tableSetName}.{subsetName}.bytes");
                    result = tableSet.Save(subsetFile, subsetName, subsetDataTable);
                    if (result == false)
                    {
                        Logger.Log.Info($"-------------------------------------------------------");
                        Logger.Log.Tool.Summary();
                        Logger.Log.Tool.Error(DebugInfo.Empty, "TableSet Save Subset Failed.");
                        return false;
                    }
                }
            }

            //
            Logger.Log.Info("------------------------------------------------------");
            Logger.Log.Info($"CONVERT END");
            Logger.Log.Tool.Summary();
            Logger.Log.Info("------------------------------------------------------");
            Logger.Log.Info("");

            return true;
        }
    }
}
