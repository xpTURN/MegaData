using System;
using System.Collections;
using System.Reflection;

using xpTURN.Common;
using static xpTURN.MegaData.TableTypeUtils;

namespace xpTURN.MegaData
{
    [TableSetPostProcess(typeof(TableSet), Order = -90)]
    public class RefIdAliasPostProcess : TableSetPostProcess
    {
        readonly string sRefId = "RefId";
        readonly string sRefIdAlias = "RefIdAlias";

        private void ForStringField(Data data, FieldInfo aliasField)
        {
            var type = data.GetType();
            string alias = (string)aliasField.GetValue(data);
            if (string.IsNullOrEmpty(alias))
            {
                Logger.Log.Tool.Error(data.DebugInfo, $"Field '{aliasField.Name}' in type '{type.Name}' is null or empty.");
                return;
            }

            var idField = type.GetField(aliasField.Name.Substring(0, aliasField.Name.Length - sRefIdAlias.Length + sRefId.Length), BindingFlags.Public | BindingFlags.Instance);
            if (idField == null || !IsAcceptableIdType(idField.FieldType))
            {
                Logger.Log.Tool.Error(data.DebugInfo, $"Field '{aliasField.Name}' in type '{type.Name}' is not a valid ID field Type.{idField?.FieldType.Name ?? "null"}.");
                return;
            }

            var mapAliasData = TableSet.GetMetaDataTable().MapAliasData;
            mapAliasData.TryGetValue(alias, out AliasData aliasData);
            if (aliasData == null)
            {
                Logger.Log.Tool.Error(data.DebugInfo, $"Field '{type.Name}.{idField.Name}' has an invalid Alias '{alias}' not found.");
                return;
            }

            idField.SetValue(data, aliasData.Id); // Set the ID field to the corresponding ID value
            aliasField.SetValue(data, string.Empty); // Clear alias field to avoid writing it back to the file
        }

        private void ForListField(Data data, FieldInfo listField)
        {
            var type = data.GetType();
            var aliasList = (IList)listField.GetValue(data);
            if (aliasList == null)
            {
                Logger.Log.Tool.Error(data.DebugInfo, $"Field '{listField.Name}' in type '{data.GetType().Name}' is null.");
                return;
            }

            var idField = type.GetField(listField.Name.Substring(0, listField.Name.Length - sRefIdAlias.Length + sRefId.Length), BindingFlags.Public | BindingFlags.Instance);
            if (idField == null)
            {
                Logger.Log.Tool.Error(data.DebugInfo, $"Field '{listField.Name}' in type '{type.Name}' is not a valid ID field Type.{idField?.FieldType.Name ?? "null"}.");
                return;
            }

            if (idField.IsList() && idField.FieldType.GetGenericArguments()[0] != typeof(int))
            {
                Logger.Log.Tool.Error(data.DebugInfo, $"Field '{listField.Name}' in type '{type.Name}' is not a valid ID list Type.{idField.FieldType.Name}.");
                return;
            }

            var idList = (IList)idField.GetValue(data);
            if (idList == null)
            {
                Logger.Log.Tool.Error(data.DebugInfo, $"Field '{idField.Name}' in type '{data.GetType().Name}' is null.");
                return;
            }

            idList.Clear(); // Clear the ID list to prepare for new values

            for (int i = 0; i < aliasList.Count; i++)
            {
                string alias = (string)aliasList[i];
                if (string.IsNullOrEmpty(alias))
                {
                    continue; // Skip empty aliases
                }

                var mapAliasData = TableSet.GetMetaDataTable().MapAliasData;
                mapAliasData.TryGetValue(alias, out AliasData aliasData);
                if (aliasData == null)
                {
                    Logger.Log.Tool.Error(data.DebugInfo, $"Element {i} in field '{type.Name}.{listField.Name}' has an invalid Alias '{alias}' not found.");
                    continue;
                }

                idList.Add(aliasData.Id); // Set the ID value
            }

            aliasList.Clear(); // Clear alias list to avoid writing it back to the file
        }

        public override void PostProcess(IPostProcess.Context context, Data data)
        {
            var type = data.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (!field.Name.EndsWith(sRefIdAlias))
                    continue; // Skip fields that do not end with "RefIdAlias"

                if (field.FieldType == typeof(string))
                {
                    ForStringField(data, field);
                    continue;
                }

                if (field.IsList() && field.FieldType.GetGenericArguments()[0] == typeof(string))
                {
                    ForListField(data, field);
                    continue;
                }
            }
        }
    }
}