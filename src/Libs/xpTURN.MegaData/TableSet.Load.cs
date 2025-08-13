using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using xpTURN.Common;
using xpTURN.Protobuf;

namespace xpTURN.MegaData
{
    public abstract partial class TableSet : IDisposable
    {
        #region Load Public Methods
        /// <summary>
        /// Loads the table set from the specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to load.</param>
        /// <param name="prepareAll">Whether to prepare all data. (On-demand data loading)</param>
        /// <returns>True if the load was successful; otherwise, false.</returns>
        public bool Load(string fileName, bool prepareAll = false)
        {
            return Load(fileName, additive: false, prepareAll: prepareAll);
        }

        /// <summary>
        /// Loads the table set from the specified file in additive mode.
        /// </summary>
        /// <param name="fileName">The name of the file to load.</param>
        /// <returns>True if the load was successful; otherwise, false.</returns>
        public bool LoadAdditive(string fileName)
        {
            return Load(fileName, additive: true, prepareAll: IsPrepareAll);
        }

        protected bool Load(string fileName, bool additive, bool prepareAll)
        {
            if (!additive)
            {
                EnableWeakRef = !prepareAll;
                IsPrepareAll = prepareAll;

                Reset(true);
            }

            if (!System.IO.File.Exists(fileName))
            {
                Logger.Log.Error($"File not found: {fileName}");
                return false;
            }

            var alreadyLoadedTables = new HashSet<string>();
            foreach (var pair in Tables)
            {
                var table = pair.Value;
                if (table == null || !table.IsLoaded)
                    continue;

                alreadyLoadedTables.Add(pair.Key);
            }

            try
            {
                FileName = fileName;
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    bool result = Load(fileStream, alreadyLoadedTables);
                    if (!result)
                    {
                        return false;
                    }
                }

                // Initialize the stream for on-demand data
                var ondemand = Tables.Keys.Any(tableName => !alreadyLoadedTables.Contains(tableName) && Tables[tableName]?.IsOndemand == true);
                if (ondemand)
                {
                    StreamForOndemand = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                }

                foreach (var pair in Tables)
                {
                    var tableName = pair.Key;
                    var table = pair.Value;
                    if (!table.IsOndemand && alreadyLoadedTables.Contains(pair.Key))
                        continue;

                    table.GetMetaNestedData().InitStream(StreamForOndemand, TableLocation);

                    if (IsPrepareAll)
                    {
                        var tableId = GetTableId(tableName);
                        foreach (var key in table.GetMetaNestedData().MapIdOffset.Keys)
                        {
                            GetOrLoadDataById(table, tableId, key);
                        }

                        foreach (var key in table.GetMetaNestedData().MapAliasOffset.Keys)
                        {
                            GetOrLoadDataByAlias(table, tableId, key);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error loading Header: {ex.Message}");
                Logger.Log.Error(ex.StackTrace);
                return false;
            }
        }
        #endregion

        #region Load Private Methods
        private bool Load(Stream readStream, HashSet<string> alreadyLoadedTables)
        {
            bool result = LoadHeader(readStream);
            if (!result)
            {
                return false;
            }

            result = LoadMetaData(readStream);
            if (!result)
            {
                return false;
            }

            result = LoadTable(readStream, alreadyLoadedTables);
            if (!result)
            {
                return false;
            }

            return true;
        }

        private bool LoadHeader(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    return LoadHeader(fileStream);
                }
            }
            catch
            {
                return false;
            }
        }

        private bool LoadHeader(Stream readStream)
        {
            try
            {
                readStream.Seek(HeaderLocation, SeekOrigin.Begin);
                HeaderSize = xpParseUtils.ReadDelimitedFrom(Header, readStream);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error loading Header: {ex.Message}");
                Logger.Log.Error(ex.StackTrace);
                return false;
            }
        }

        private bool LoadMetaData(Stream readStream)
        {
            try
            {
                var metaDataTable = new MetaDataTable();
                readStream.Seek(MetaLocation, SeekOrigin.Begin);
                MetaSize = xpParseUtils.ReadDelimitedFrom(metaDataTable, readStream);

                MetaDataTable.MergeFrom(metaDataTable);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error loading MetaData: {ex.Message}");
                Logger.Log.Error(ex.StackTrace);
                return false;
            }
        }

        private bool LoadTable(Stream readStream, HashSet<string> alreadyLoadedTables)
        {
            try
            {
                Tables.Capacity = Tables.Count + MetaDataTable.MapMetaData.Count;
                foreach (var metaData in MetaDataTable.MapMetaData.Values)
                {
                    var tableId = GetTableId(metaData.Name);

                    // When loading legacy data, there may be cases where deleted tableIds cannot be found
                    if (tableId == 0)
                        continue;

                    if (alreadyLoadedTables.Contains(metaData.Name))
                    {
                        // If the table is already loaded, skip it
                        continue;
                    }

                    // Create table instance
                    Table table = CreateTableById(tableId);
                    if (table == null)
                        return false;

                    // Seek to the table's location in the file
                    readStream.Seek(TableLocation + metaData.Offset, SeekOrigin.Begin);

                    // Deserialize the table data
                    IMessage tableMsg = (IMessage)table;
                    xpParseUtils.ReadDelimitedFrom(tableMsg, readStream);

                    // Table assignment
                    table.IsLoaded = true;
                    Tables[metaData.Name] = table;
                }

                // Even tables with no saved data must be registered. Consider data lookup / table indexing. 
                foreach (var pair in TableAlias)
                {
                    var tableName = pair.Key;
                    int tableId = pair.Value;
                    if (!Tables.ContainsKey(tableName))
                    {
                        // Create table instance
                        Table table = CreateTableById(tableId);
                        if (table == null)
                            return false;

                        // Table assignment
                        table.IsLoaded = false;
                        Tables[tableName] = table;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error loading Table: {ex.Message}");
                Logger.Log.Error(ex.StackTrace);
                return false;
            }
        }

        private Data LoadOnDemand(Table table, int tableId, int dataId = 0, string dataAlias = null)
        {
            var offset = GetDataOffset(table, dataId, dataAlias);
            if (offset == -1L)
            {
                // No metadata found for the data ID or alias
                Logger.Log.Error($"No metadata found for data Id '{dataId}' or Alias '{dataAlias}' in table '{table.GetType().Name}'.");
                return null;
            }

            var metaData = table.GetMetaNestedData();
            if (metaData == null)
            {
                Logger.Log.Error($"MetaData for on-demand data is not initialized for table '{table.GetType().Name}'.");
                return null;
            }

            var stream = metaData.StreamForOndemand;
            if (stream == null)
            {
                Logger.Log.Error($"Stream for on-demand data is not initialized for table '{table.GetType().Name}'.");
                return null;
            }

            try
            {
                stream.Seek(metaData.TableLocation + offset, SeekOrigin.Begin);

                // Deserialize the table data
                var data = CreateDataById(tableId);
                if (data == null)
                {
                    return null;
                }

                IMessage dataMsg = (IMessage)data;
                xpParseUtils.ReadDelimitedFrom(dataMsg, stream);

                return data;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error loading on-demand data: {ex.Message}");
                Logger.Log.Error(ex.StackTrace);
            }

            return null;
        }
        
        private (ByteString Meta, ByteString Data) RetrieveFileHash(string fileName)
        {
            bool result = LoadHeader(fileName);
            if (!result)
            {
                return (ByteString.Empty, ByteString.Empty);
            }

            return (Header.MetaHash, Header.DataHash);
        }
        #endregion
    }
}
