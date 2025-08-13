using System;
using System.Collections.Generic;
using System.Globalization;

using xpTURN.Common;
using xpTURN.MegaData;

namespace Tests.RefId
{
    [TableSetPostProcess(typeof(RefIdTableSet), 100)]
    public class LocaleTablePostProcess : TableSetPostProcess
    {
        Dictionary<int, TextDataTable> _mapTextTable = new Dictionary<int, TextDataTable>();
        Dictionary<string, CultureInfo> _mapCultureInfo = new Dictionary<string, CultureInfo>();

        CultureInfo GetCultureInfo(string cultureName)
        {
            if (_mapCultureInfo.TryGetValue(cultureName, out var cultureInfo))
                return cultureInfo;

            try
            {
                cultureInfo = new CultureInfo(cultureName);
                _mapCultureInfo[cultureInfo.Name] = cultureInfo;
                return cultureInfo;
            }
            catch (CultureNotFoundException)
            {
                return null;
            }
        }

        void AddData(TableSet tableSet, int localeId, TranslatedData translatedData, string text)
        {
            if (!_mapTextTable.TryGetValue(localeId, out var textTable))
            {
                textTable = tableSet.CreateTableByName(nameof(TextDataTable)) as TextDataTable;
                textTable.Id = localeId;
                _mapTextTable[localeId] = textTable;
            }

            // 
            TextData textData = tableSet.CreateDataByName(nameof(TextDataTable)) as TextData;
            textData.Id = translatedData.Id;
            textData.IdAlias = translatedData.IdAlias;
            textData.Text = text;

            // Check for duplicate Id
            if (textTable.GetMap().Get(textData.Id) != null)
            {
                Logger.Log.Tool.Error(translatedData.DebugInfo, $"LocaleTableSet_PostProcess: Duplicate TextData.Id '{textData.Id}':'{textData.IdAlias}'.");
                return;
            }

            textTable.GetMap().Add(textData.Id, textData);
        }

        public override void PostProcess(IPostProcess.Context context, Table table)
        {
            TranslatedDataTable translatedTable = table as TranslatedDataTable;
            if (translatedTable == null)
                return;

            foreach (var data in translatedTable.GetMap())
            {
                if (data is TranslatedData translatedData)
                {
                    foreach (var keyValuePair in translatedData.Map)
                    {
                        var cultureInfo = GetCultureInfo(keyValuePair.Key);
                        if (cultureInfo == null)
                        {
                            Logger.Log.Tool.Error(translatedData.DebugInfo, $"LocaleTableSet_PostProcess: Culture '{keyValuePair.Key}' not found for TranslatedData '{translatedData.IdAlias}'.");
                            continue;
                        }

                        AddData(TableSet, cultureInfo.LCID, translatedData, keyValuePair.Value);
                    }
                }
            }
        }

        public override void End(IPostProcess.Context context, TableSet tableSet)
        {
            //
            var refIdTableSet = tableSet as RefIdTableSet;
            if (refIdTableSet == null)
                return;

            // Create LocaleDataTable and populate it with collected LocaleData
            var localeDataTable = tableSet.CreateTableByName(nameof(LocaleDataTable));
            foreach (var keyValuePair in _mapTextTable)
            {
                localeDataTable.GetMap().Add(keyValuePair.Key, keyValuePair.Value);
            }

            refIdTableSet.AddTable(nameof(LocaleDataTable), localeDataTable);

            // Remove the original TranslatedDataTable
            refIdTableSet.DelTable(nameof(TranslatedDataTable));

            // Update the TableId to the new LocaleDataTable
            var translatedDataTableId = refIdTableSet.GetTableId(nameof(TranslatedDataTable));
            var localeDataTableId = refIdTableSet.GetTableId(nameof(LocaleDataTable));
            foreach (var keyValuePair in refIdTableSet.GetMetaDataTable().MapAliasData)
            {
                if (keyValuePair.Value.TableId == translatedDataTableId)
                {
                    keyValuePair.Value.TableId = localeDataTableId;
                }
            }
        }
    }
}