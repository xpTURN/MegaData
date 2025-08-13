using System;
using System.Collections.Generic;
using System.Globalization;

using xpTURN.Common;
using xpTURN.Protobuf;
using xpTURN.MegaData;

namespace Tests.Locale.Type1
{
    [TableSetPostProcess(typeof(Locale1TableSet), 100)]
    public class LocaleTablePostProcess : TableSetPostProcess
    {
        Dictionary<int, LocaleData> _mapLocaleDataTable = new Dictionary<int, LocaleData>();
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
            if (!_mapLocaleDataTable.TryGetValue(localeId, out var localeData))
            {
                localeData = tableSet.CreateDataByName(nameof(LocaleDataTable)) as LocaleData;
                localeData.Id = localeId;
                _mapLocaleDataTable[localeId] = localeData;
            }

            var textData = new TextData
            {
                Id = translatedData.Id,
                Text = text
            };

            if (localeData.Map.ContainsKey(textData.Id))
            {
                Logger.Log.Tool.Error(translatedData.DebugInfo, $"LocaleTableSet_PostProcess: Duplicate TextData.Id '{textData.Id}'.");
                return;
            }

            localeData.Map.Add(textData.Id, textData);
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
            var localeTableSet = tableSet as Locale1TableSet;
            if (localeTableSet == null)
                return;

            // Create LocaleDataTable and populate it with collected LocaleData
            var localeDataTable = new LocaleDataTable();
            foreach (var keyValuePair in _mapLocaleDataTable)
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