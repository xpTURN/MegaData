using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;


using xpTURN.Common;
using xpTURN.Protobuf;
using static xpTURN.Protobuf.CodedOutputStream;

namespace xpTURN.MegaData
{
    public abstract partial class TableSet
    {
        #region Save Public Methods
        public MetaDataTable GetMetaDataTable() => MetaDataTable;
        
        protected SortedList<string, Table> GetSubsetTables(string subset, SubsetDataTable subsetDataTable)
        {
            SortedList<string, Table> subsetTables = new SortedList<string, Table>(Tables);
            if (subsetDataTable != null)
            {
                // If subsetDataTable is provided, filter tables based on the subset
                if (!string.IsNullOrEmpty(subset))
                {
                    subsetDataTable.Map.TryGetValue(subset, out var subsetData);
                    if (subsetData == null)
                    {
                        Logger.Log.Tool.Error(DebugInfo.Empty, $"No subset data found for '{subset}'.");
                        return null; // No changes, no need to save
                    }

                    // Filter tables based on subset
                    subsetTables = Tables.Where(t => subsetData.Tables.Contains(t.Key)).ToSortedList(t => t.Key, t => t.Value);
                    if (subsetTables.Count == 0)
                    {
                        Logger.Log.Info($"No tables found for subset '{subset}'.");
                        return null; // No changes, no need to save
                    }
                }
                else
                {
                    // If subset is empty, use the default subset
                    HashSet<string> subsetTableNames = new HashSet<string>();
                    foreach (var pair in subsetDataTable.Map)
                    {
                        subsetTableNames.UnionWith(pair.Value.Tables);
                    }

                    // Filter tables based on subset
                    subsetTables = Tables.Where(t => !subsetTableNames.Contains(t.Key)).ToSortedList(t => t.Key, t => t.Value);
                    if (subsetTables.Count == 0)
                    {
                        Logger.Log.Info($"No tables found for subset 'Default'.");
                        return null; // No changes, no need to save
                    }
                }
            }

            return subsetTables;
        }

        public bool Save(string fileName, string subset = "", SubsetDataTable subsetDataTable = null, bool force = true)
        {
            // Ensure output directory exists.
            var path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Error creating directory '{path}': {ex.Message}");
                    return false;
                }
            }

            // metadata table must be initialized before saving
            MetaDataTable.MapMetaData.Clear();

            // Retrieve original file hash if it exists
            var orgHash = RetrieveFileHash(fileName);

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
                    {
                        Logger.Log.Error($"Failed to create table for alias '{tableName}' with ID {tableId}.");
                        return false;
                    }

                    // Table assignment
                    Tables[tableName] = table;
                }
            }

            // Get subset tables to save.
            SortedList<string, Table> subsetTables = GetSubsetTables(subset, subsetDataTable);
            if (subsetTables == null)
            {
                return false; // No changes, no need to save
            }

            // Build table and metadata streams.
            int offset = 0;
            MemoryStream tableStream = new MemoryStream();
            {
                // Write on-demand data first.
                WriteOnDemand(tableStream, subsetTables, ref offset);

                // Write table data.
                WriteTables(tableStream, subsetTables, ref offset);
            }

            MemoryStream metaDataStream = new MemoryStream();
            {
                // Write metadata table
                MetaDataTable.WriteDelimitedTo(metaDataStream);

                // Update meta size for header.
                MetaSize = MetaDataTable.CalculateSize();
            }

            // Create header
            Header = new Header
            {
                Space = GetType().Namespace,
                Name = GetType().Name,
                Subset = string.IsNullOrEmpty(subset) ? "Default" : subset,
                MetaHash = ByteString.FromBase64(HashUtils.ComputeSHA256Hash(metaDataStream)),
                DataHash = ByteString.FromBase64(HashUtils.ComputeSHA256Hash(tableStream)),
            };

            HeaderSize = Header.CalculateSize();

            if (!force && Header.MetaHash == orgHash.Meta && Header.DataHash == orgHash.Data)
            {
                Logger.Log.Info($"");
                Logger.Log.Info($"No changes detected.");
                Logger.Log.Info($"Already up-to-date File: '{fileName}'");
                return true; // No changes, no need to save
            }
            else
            {
                Logger.Log.Info($"");
                Logger.Log.Info($"");
                Logger.Log.Info($"Saving TableSet '{GetType().Name}' to file: {fileName}");
                Logger.Log.Info($"Space: {GetType().Namespace}");
                Logger.Log.Info($"TableSet Name: {GetType().Name}");
                Logger.Log.Info($"SHA256: {Header.DataHash.ToBase64()}");
            }

            // Save to file
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    // Write magic number and version.
                    byte[] magicBytes = new byte[4];
                    BinaryPrimitives.WriteUInt32LittleEndian(magicBytes, MAGIC_NUMBER);
                    fileStream.Write(magicBytes, 0, 4);

                    byte[] versionBytes = new byte[4];
                    BinaryPrimitives.WriteUInt32LittleEndian(versionBytes, CURRENT_VERSION);
                    fileStream.Write(versionBytes, 0, 4);

                    // Write header
                    Header.WriteDelimitedTo(fileStream);

                    // Write metadata
                    metaDataStream.WriteTo(fileStream);

                    // Write data tables
                    tableStream.WriteTo(fileStream);

                    Logger.Log.Info($"");
                    Logger.Log.Info($"Saved to: {fileName}");
                    Logger.Log.Info($"SHA256: {Header.DataHash.ToBase64()}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Error saving data: {ex.Message}");
                    return false;
                }
            }

            // Optionally save JSON representation.
            if (JsonWrapper.ToJsonMethod != null)
            {
                var result = SaveToJson(fileName, subsetTables);
                if (!result)
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, "Failed to save JSON representation of the TableSet.");
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Save Methods
        private void WriteOnDemandData(MemoryStream tableStream, ref int offset, MetaNestedData metaNestedData, Data nestedItem)
        {
            // Retrieve message information
            IMessage msg = (IMessage)nestedItem;

            // Save message
            msg.WriteDelimitedTo(tableStream);

            // Next Table offset
            var size = msg.CalculateSize();
            offset += ComputeLengthSize(size) + size; // Length + MessageSize

            // Track for on-demand reload.
            metaNestedData.AddOnDemandData(nestedItem);
        }

        private void WriteOnDemandTable(MemoryStream tableStream, ref int offset, Table table)
        {
            MetaNestedData metaData = null;
            if (table.IsOnDemand)
            {
                metaData = InvokeUtils.GetFieldValue(table, "MetaNestedData") as MetaNestedData;
                if (metaData == null)
                {
                    metaData = new MetaNestedData();
                    InvokeUtils.SetFieldValue(table, "MetaNestedData", metaData);
                }
            }

            // by GetMap()
            foreach (var nestedItem in table.GetMap())
            {
                // If the nested item is a Table, cast it to Table.
                // Before saving the message, process child nested data to ensure offsets are stored correctly.
                var nestedTable = nestedItem as Table;
                if (nestedTable != null)
                {
                    WriteOnDemandTable(tableStream, ref offset, nestedTable);
                }

                if (table.IsOnDemand)
                {
                    // Metadata
                    metaData.MapIdOffset.Add(nestedItem.GetId(), offset);

                    // Save message
                    WriteOnDemandData(tableStream, ref offset, metaData, nestedItem);
                }
            }

            // by GetMapAlias()
            foreach (var nestedItem in table.GetMapAlias())
            {
                // If the nested item is a Table, cast it to Table
                // Before saving the message, process child nested data to ensure offsets are stored correctly.
                var nestedTable = nestedItem as Table;
                if (nestedTable != null)
                {
                    WriteOnDemandTable(tableStream, ref offset, nestedTable);
                }

                if (table.IsOnDemand)
                {
                    // Metadata
                    metaData.MapAliasOffset.Add(nestedItem.GetAlias(), offset);

                    // Save message
                    WriteOnDemandData(tableStream, ref offset, metaData, nestedItem);
                }
            }

            if (table.IsOnDemand)
            {
                // Clear nested data after saving
                table.GetMap().Clear();
                table.GetMapAlias().Clear();
            }
        }

        private void WriteOnDemand(MemoryStream tableStream, SortedList<string,Table> tables, ref int offset)
        {
            // Write each table's on-demand data.
            foreach (var table in tables.Values)
            {
                WriteOnDemandTable(tableStream, ref offset, table);
            }
        }

        private void WriteTables(MemoryStream tableStream, SortedList<string,Table> tables, ref int offset)
        {
            foreach (var pair in tables)
            {
                // Retrieve message information
                IMessage msg = (IMessage)pair.Value;

                // Save message
                msg.WriteDelimitedTo(tableStream);

                // Metadata
                var metaData = GetOrCreateMetaData(pair.Key);
                metaData.Offset = offset;

                // Next Table offset
                var size = msg.CalculateSize();
                offset += ComputeLengthSize(size) + size; // Length + MessageSize
            }
        }

        private bool SaveToJson(string fileName, SortedList<string,Table> subsetTables)
        {
            // Output path for JSON file.
            string jsonFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".json");
            using (var jsonStream = new FileStream(jsonFileName, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    var tableSet = (TableSet)Activator.CreateInstance(GetType(), true);
                    if (tableSet == null)
                    {
                        Logger.Log.Tool.Error(DebugInfo.Empty, "Failed to create TableSet instance for JSON export.");
                        return false;
                    }

                    tableSet.Header = this.Header;
                    tableSet.MetaDataTable = this.MetaDataTable;
                    tableSet.Tables = subsetTables;

                    string json = tableSet.ToJson();
                    var bytes = Encoding.UTF8.GetBytes(json);
                    jsonStream.Write(bytes, 0, bytes.Length);

                    Logger.Log.Info($"");
                    Logger.Log.Info($"TableSet saved to: {jsonFileName}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Info($"");
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Error saving data: {ex.Message}");
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
