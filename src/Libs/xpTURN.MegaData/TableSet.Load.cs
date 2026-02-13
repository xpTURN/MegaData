using System;
using System.Buffers.Binary;
using System.IO;
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
                var ondemand = false;
                foreach (var pair in Tables)
                {
                    if (!alreadyLoadedTables.Contains(pair.Key) && pair.Value?.IsOnDemand == true)
                    {
                        ondemand = true;
                        break;
                    }
                }

                if (ondemand)
                {
                    StreamForOnDemand = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                }

                foreach (var pair in Tables)
                {
                    var tableName = pair.Key;
                    var table = pair.Value;
                    if (!table.IsOnDemand && alreadyLoadedTables.Contains(pair.Key))
                        continue;

                    var metaNestedData = table.GetMetaNestedData();
                    metaNestedData?.InitStream(StreamForOnDemand, TableLocation);

                    if (IsPrepareAll && metaNestedData != null)
                    {
                        var tableId = GetTableId(tableName);
                        foreach (var key in metaNestedData.MapIdOffset.Keys)
                        {
                            GetOrLoadDataById(table, tableId, key);
                        }

                        foreach (var key in metaNestedData.MapAliasOffset.Keys)
                        {
                            GetOrLoadDataByAlias(table, tableId, key);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error loading data file");
#if DEBUG
                Logger.Log.Debug($"Exception: {ex.Message}");
                Logger.Log.Debug($"StackTrace: {ex.StackTrace}");
#endif
                // Clean up on-demand stream if an error occurs
                StreamForOnDemand?.Dispose();
                StreamForOnDemand = null;
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
            catch (Exception ex)
            {
                Logger.Log.Error($"Error loading Header: {ex.Message}");
                return false;
            }
        }

        private bool LoadHeader(Stream readStream)
        {
            try
            {
                readStream.Seek(0, SeekOrigin.Begin);

                // Magic number verification
                byte[] magicBytes = new byte[4];
                if (readStream.Read(magicBytes, 0, 4) != 4)
                {
                    Logger.Log.Error("Invalid file: unable to read magic number");
                    return false;
                }

                uint magic = BinaryPrimitives.ReadUInt32LittleEndian(magicBytes);
                if (magic != MAGIC_NUMBER)
                {
                    Logger.Log.Error($"Invalid file format: magic number mismatch (expected {MAGIC_NUMBER:X}, got {magic:X})");
                    return false;
                }

                // Version verification
                byte[] versionBytes = new byte[4];
                if (readStream.Read(versionBytes, 0, 4) != 4)
                {
                    Logger.Log.Error("Invalid file: unable to read version");
                    return false;
                }
                
                uint version = BinaryPrimitives.ReadUInt32LittleEndian(versionBytes);
                if (version > CURRENT_VERSION)
                {
                    Logger.Log.Error($"Unsupported version: {version}");
                    return false;
                }

                readStream.Seek(HeaderLocation, SeekOrigin.Begin);
                HeaderSize = xpParseUtils.ReadDelimitedFrom(Header, readStream);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error loading Header");
#if DEBUG
                Logger.Log.Debug($"Exception: {ex.Message}");
                Logger.Log.Debug($"StackTrace: {ex.StackTrace}");
#endif
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
                Logger.Log.Error("Error loading MetaData");
#if DEBUG
                Logger.Log.Debug($"Exception: {ex.Message}");
                Logger.Log.Debug($"StackTrace: {ex.StackTrace}");
#endif
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
                Logger.Log.Error("Error loading Table");
#if DEBUG
                Logger.Log.Debug($"Exception: {ex.Message}");
                Logger.Log.Debug($"StackTrace: {ex.StackTrace}");
#endif
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

            var stream = metaData.StreamForOnDemand;
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
                Logger.Log.Error("Error loading on-demand data");
#if DEBUG
                Logger.Log.Debug($"Exception: {ex.Message}");
                Logger.Log.Debug($"StackTrace: {ex.StackTrace}");
#endif
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
