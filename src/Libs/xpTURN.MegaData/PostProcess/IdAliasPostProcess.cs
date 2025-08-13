using System;
using System.Reflection;

using xpTURN.Common;
using static xpTURN.MegaData.TableTypeUtils;
using System.Collections.Generic;

namespace xpTURN.MegaData
{
    [TableSetPostProcess(typeof(TableSet), Order = -100)]
    public class IdAliasPostProcess : TableSetPostProcess
    {
        readonly string sId = "Id";
        readonly string sIdAlias = "IdAlias";
        readonly string sRefIdAlias = "RefIdAlias";

        private Dictionary<string, AliasData> _mapAliasData = new ();

        public override void PostProcess(IPostProcess.Context context, Data data)
        {
            if (!context.IsMainData)
                return;

            if (!context.IsEnumerable)
                return; // Skip if not processing an enumerable context

            var tableId = TableSet.GetTableId(Table.GetType().Name);
            var dataType = data.GetType();
            var fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(string) || !field.Name.EndsWith(sIdAlias))
                    continue; // Skip non-string fields or fields not ending with "IdAlias"

                if (field.Name.EndsWith(sRefIdAlias))
                    continue; // Skip RefIdAlias fields

                var aliasField = field;
                string alias = (string)aliasField.GetValue(data);
                if (string.IsNullOrEmpty(alias))
                {
                    Logger.Log.Tool.Error(data.DebugInfo, $"Field '{aliasField.Name}' in type '{dataType.Name}' is null or empty.");
                    continue;
                }

                var idField = dataType.GetField(aliasField.Name.Substring(0, aliasField.Name.Length - sIdAlias.Length + sId.Length), BindingFlags.Public | BindingFlags.Instance);
                if (idField == null || !IsAcceptableIdType(idField.FieldType))
                {
                    Logger.Log.Tool.Error(data.DebugInfo, $"Field '{aliasField.Name}' in type '{dataType.Name}' is not a valid ID field Type.{idField?.FieldType.Name ?? "null"}.");
                    continue;
                }

                object idValue = idField.GetValue(data);
                if (idValue == null)
                {
                    Logger.Log.Tool.Error(data.DebugInfo, $"Field '{idField.Name}' in type '{dataType.Name}' is null.");
                    continue;
                }

                int id = (int)ConvertTypeUtils.ConvertToType(idValue, typeof(int));
                if (id == 0)
                {
                    Logger.Log.Tool.Error(data.DebugInfo, $"Field '{idField.Name}' in type '{dataType.Name}' has an invalid ID value: {id}. It must be non-negative.");
                    continue;
                }

                var found = _mapAliasData.TryGetValue(alias, out AliasData aliasData);
                if (found)
                {
                    Logger.Log.Tool.Error(data.DebugInfo, $"Field '{idField.Name}' in type '{dataType.Name}' has an invalid Alias '{alias}' already exists with ID {aliasData.Id}.");
                    continue;
                }

                _mapAliasData[alias] = new AliasData { Id = id, TableId = tableId };

                //
                aliasField.SetValue(data, string.Empty); // Clear alias field to avoid writing it back to the file
            }
        }

        public override void End(IPostProcess.Context context, TableSet tableSet)
        {
            var metaDataTable = tableSet.GetMetaDataTable();

            // Add collected aliases to the MetaDataTable
            foreach (var kvp in _mapAliasData)
            {
                if (!metaDataTable.MapAliasData.ContainsKey(kvp.Key))
                {
                    metaDataTable.MapAliasData[kvp.Key] = kvp.Value;
                }
                else
                {
                    Logger.Log.Error($"Alias '{kvp.Key}' already exists in MetaDataTable with ID {metaDataTable.MapAliasData[kvp.Key].Id}.");
                }
            }

            // Clear the map after processing
            _mapAliasData.Clear();
        }
    }
}