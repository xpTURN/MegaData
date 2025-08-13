using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;


using xpTURN.Common;
using xpTURN.Protobuf;
using xpTURN.MegaData;

namespace Tests.Locale.Type2
{
    [TableSetPostProcess(typeof(Locale2TableSet), 100)]
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
            var localeTableSet = tableSet as Locale2TableSet;
            if (localeTableSet == null)
                return;

            // Create LocaleDataTable and populate it with collected LocaleData
            var localeDataTable = tableSet.CreateTableByName(nameof(LocaleDataTable));
            foreach (var keyValuePair in _mapTextTable)
            {
                localeDataTable.GetMap().Add(keyValuePair.Key, keyValuePair.Value);
            }

            localeTableSet.AddTable(nameof(LocaleDataTable), localeDataTable);

            // Remove the original TranslatedDataTable
            localeTableSet.DelTable(nameof(TranslatedDataTable));

            // Update the TableId to the new LocaleDataTable
            var translatedDataTableId = localeTableSet.GetTableId(nameof(TranslatedDataTable));
            var localeDataTableId = localeTableSet.GetTableId(nameof(LocaleDataTable));
            foreach (var keyValuePair in localeTableSet.GetMetaDataTable().MapAliasData)
            {
                if (keyValuePair.Value.TableId == translatedDataTableId)
                {
                    keyValuePair.Value.TableId = localeDataTableId;
                }
            }
        }
    }
}