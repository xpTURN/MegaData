
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using xpTURN.Common;
using xpTURN.MegaData;
using xpTURN.Tool.Common;

namespace xpTURN.Converter.Utils
{
    public static class ConverterUtils
    {
        public static readonly DataCell sTABLE_NAME_CELL = new DataCell(2, 2);
        public static readonly DataCell sTABLE_DESC_CELL = new DataCell(2, 2);
        public static readonly DataCell sDATA_NAME_CELL = new DataCell(2, 3);
        public static readonly DataCell sDATA_EXCLUDE_CELL = new DataCell(1, 3);

        public static readonly string TABLE_SHEET = "Table";
        public static readonly string sAlias = "Alias";
        public static readonly string sIdAlias = "IdAlias";
        public static readonly string sRefIdAlias = "RefIdAlias";

        public static string RegularName(string name)
        {
            var match = Regex.Match(name, @"^{(?<field>[^}]+)}$");
            if (match.Success)
            {
                name = match.Groups["field"].Value;
            }

            match = Regex.Match(name, @"^(?<field>[^<]+)<(?<key>[^>]+)>$");
            if (match.Success)
            {
                name = match.Groups["field"].Value;
            }

            return name;
        }

        public static bool TryGetMapKey(string cellString, out string key)
        {
            var match = Regex.Match(cellString, @"^(?<field>[^<]+)<(?<key>[^>]+)>$");
            if (match.Success)
            {
                key = match.Groups["key"].Value;
                return true;
            }

            key = null;
            return false;
        }

        public static bool IsNestedField(string cellString)
        {
            var match = Regex.Match(cellString, @"^{(?<field>[^}]+)}$");
            if (match.Success)
            {
                return true;
            }

            return false;
        }

        public static DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return DateTime.MinValue; // No date specified
            }

            bool success = DateTime.TryParseExact(
                dateString,
                "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate
            );

            if (!success)
            {
                return DateTime.MinValue; // Invalid date format
            }

            return parsedDate;
        }

        public static bool IsExcludeData(string excludeOption, DateTime targetDate, string dataName)
        {
            if (string.IsNullOrEmpty(excludeOption))
            {
                return false;
            }

            if (string.IsNullOrEmpty(dataName))
            {
                Logger.Log.Tool.Error($"Exclude options can only be specified for top-level data items: '{excludeOption}'");
                return false; // No data name, do not exclude
            }

            // Check if the cell is a comment
            if (string.Compare(excludeOption, "del", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(excludeOption, "delete", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(excludeOption, "hide", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            if (excludeOption.StartsWith("del_", StringComparison.OrdinalIgnoreCase))
            {
                if (targetDate == DateTime.MinValue)
                {
                    Logger.Log.Tool.Error($"Target date is not specified for exclude option: '{excludeOption}'");
                    return false; // No target date specified, do not exclude
                }

                // After filter, check if the target date is after the specified date
                string dateString = excludeOption.Substring(4).Trim();

                DateTime dDate = ParseDate(dateString);
                if (dDate == DateTime.MinValue)
                {
                    Logger.Log.Tool.Error($"Invalid date format in exclude option: '{excludeOption}'");
                    return false; // Invalid date format, do not exclude
                }

                return dDate <= targetDate; // Delete data after the specified date
            }
            else if (excludeOption.StartsWith("add_", StringComparison.OrdinalIgnoreCase))
            {
                if (targetDate == DateTime.MinValue)
                {
                    Logger.Log.Tool.Error($"Target date is not specified for exclude option: '{excludeOption}'");
                    return false; // No target date specified, do not exclude
                }

                // Before filter, check if the target date is before the specified date
                string dateString = excludeOption.Substring(4).Trim();

                DateTime dDate = ParseDate(dateString);
                if (dDate == DateTime.MinValue)
                {
                    Logger.Log.Tool.Error($"Invalid date format in exclude option: '{excludeOption}'");
                    return false; // Invalid date format, do not exclude
                }

                return targetDate < dDate; // Delete data before the specified date
            }
            else
            {
                Logger.Log.Tool.Error($"Invalid exclude option format : {excludeOption}");
            }

            return false;
        }

        public static TableSet GetTableSet(string fullName)
        {
            Type type = AssemblyUtils.GetTypeByName(fullName);
            if (type == null)
            {
                Logger.Log.Tool.Error($"TableSet Type not found : {fullName}");
                return null;
            }

            var prop = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (prop == null)
            {
                Logger.Log.Tool.Error($"TableSet Instance property not found in type : {fullName}");
                return null;
            }

            return (TableSet)prop.GetValue(null);
        }

        public static void SetDebugInfo(DebugInfo debugInfo, Data target)
        {
            if (debugInfo == null || target == null)
            {
                return;
            }

            target.DebugInfo = debugInfo;

            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // NestedData : Dictionary<key?, Message>
            var mapFields = fields.ToList().FindAll(field => field.IsDictionary() &&
                                                    typeof(Data).IsAssignableFrom(field.FieldType.GetCollectionElementType()));
            foreach (var field in mapFields)
            {
                var mapObj = field.GetValue(target) as System.Collections.IDictionary;
                var collection = mapObj.Values;
                foreach (var obj in collection)
                {
                    SetDebugInfo(debugInfo, obj as Data);
                }
            }

            // NestedData : Message field
            var dataFields = fields.ToList().FindAll(field => typeof(Data).IsAssignableFrom(field.FieldType));
            foreach (var field in dataFields)
            {
                var nestedData = field.GetValue(target) as Data;
                if (nestedData == null) continue;

                SetDebugInfo(debugInfo, nestedData);
            }

            // NestedData : List<Message> field
            var listFields = fields.ToList().FindAll(field => field.IsListArg<Data>());
            foreach (var field in listFields)
            {
                var list = InvokeUtils.GetFieldListEnumerable<Data>(field, target);
                if (list == null) continue;

                foreach (var item in list)
                {
                    SetDebugInfo(debugInfo, item as Data);
                }
            }
        }

        public static bool MergeFrom(this Table table, Table otherTable)
        {
            string tableName = table.GetType().Name;

            foreach (var data in otherTable.GetMap())
            {
                var dataId = data.GetId();
                var duplicatedData = table.GetMap().Get(dataId);
                if (duplicatedData != null)
                {
                    // If the data already exists in the new table, skip it
                    Logger.Log.Tool.Error(duplicatedData.DebugInfo, $"Data with ID {dataId} already exists in the '{tableName}'.");
                }
            }

           foreach (var data in otherTable.GetMapAlias())
            {
                var dataAlias = data.GetAlias();
                var duplicatedData = table.GetMapAlias().Get(dataAlias);
                if (duplicatedData != null)
                {
                    // If the data already exists in the new table, skip it
                    Logger.Log.Tool.Error(duplicatedData.DebugInfo, $"Data with Alias {dataAlias} already exists in the '{tableName}'.");
                    break;
                }
            }

            // To call the MergeFrom method directly, type casting is required, so InvokeUtils is used as an alternative.
            // Since this is only called in the convert tool, performance cost is not considered.
            InvokeUtils.InvokeFunc(table, "MergeFrom", new Type[] { table.GetType() }, new object[] { otherTable });

            return true;
        }
    }
}