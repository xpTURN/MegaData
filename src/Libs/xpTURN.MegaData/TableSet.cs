using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

using xpTURN.Common;
using static xpTURN.Protobuf.CodedOutputStream;

namespace xpTURN.MegaData
{
    [DataContract]
    public abstract partial class TableSet : IDisposable
    {
        #region Protobuf Serialize
        [DataMember]
        protected Header Header { get; set; } = new();
        [DataMember]
        protected MetaDataTable MetaDataTable { get; set; } = new();
        [DataMember]
        protected SortedList<string, Table> Tables { get; set; } = new();
        #endregion

        #region FileSturcture
        private string FileName { get; set; } = string.Empty;
        private int HeaderSize { get; set; } = 0;
        private int MetaSize { get; set; } = 0;
        private long HeaderLocation => 0;
        private int MetaLocation => HeaderSize != 0 ? ComputeInt32Size(HeaderSize) + HeaderSize : 0;
        private long TableLocation => MetaLocation + (MetaSize != 0 ? ComputeLengthSize(MetaSize) + MetaSize : 0);
        #endregion

        #region Runtime Properties
        public bool IsRuntimeMode { get; private set; } = true;
        public bool IsPrepareAll { get; private set; } = false;
        public bool EnableWeakRef { get; private set; } = true;
        private Stream StreamForOndemand { get; set; } = null;
        #endregion

        #region Public Method
        virtual public void Reset(bool isRuntimeMode)
        {
            Dispose();

            IsRuntimeMode = isRuntimeMode;

            if (!isRuntimeMode)
            {
                EnableWeakRef = false;
            }

            FileName = string.Empty;
            Header = new Header();
            MetaDataTable = new MetaDataTable();
            Tables = new SortedList<string, Table>();
            HeaderSize = 0;
            MetaSize = 0;
        }

        public void Dispose()
        {
            foreach (var table in Tables.Values)
            {
                table.Dispose();
            }

            StreamForOndemand?.Dispose();
            StreamForOndemand = null;
        }

        public string ToJson() => JsonWrapper.ToJson(this);
        #endregion

        #region NonPublic Methods
        abstract protected Table RawCreateTable(int tableId);
        virtual protected Data RawCreateData(int tableId) => null;
        abstract protected SortedDictionary<string, int> TableAlias { get; }
        virtual protected bool IsOndemandTable(int tableId) => false;
        virtual protected bool IsWeakRefTable(int tableId) => false;

        public int GetTableId(string tableAlias)
        {
            if (TableAlias.TryGetValue(tableAlias, out int tableId))
            {
                return tableId;
            }

            Logger.Log.Error($"Table alias '{tableAlias}' not found.");
            return 0;
        }

        protected long GetDataOffset(Table table, int dataId, string dataAlias)
        {
            var metaData = table.GetMetaNestedData();
            if (!table.IsOndemand || metaData == null)
            {
                return -1L;
            }

            long offset = -1L;
            if (dataId != 0 && metaData.MapIdOffset.TryGetValue(dataId, out offset))
                return offset;

            if (dataAlias != null && metaData.MapAliasOffset.TryGetValue(dataAlias, out offset))
                return offset;

            return -1L;
        }

        public Table CreateTableByName(string tableName)
        {
            var tableId = GetTableId(tableName);
            if (tableId == 0)
            {
                Logger.Log.Error($"Table alias '{tableName}' not found.");
                return null;
            }

            return CreateTableById(tableId);
        }

        protected Table CreateTableById(int tableId)
        {
            var table = RawCreateTable(tableId);
            if (table == null)
            {
                Logger.Log.Error($"Failed to create table for ID {tableId}.");
                return null;
            }

            table.IsOndemand = IsOndemandTable(tableId);
            table.IsWeakRef = IsWeakRefTable(tableId);

            return table;
        }

        public Data CreateDataByName(string tableName)
        {
            var tableId = GetTableId(tableName);
            if (tableId == 0)
            {
                Logger.Log.Error($"Table alias '{tableName}' not found.");
                return null;
            }

            return CreateDataById(tableId);
        }

        protected Data CreateDataById(int tableId)
        {
            var data = RawCreateData(tableId);
            if (data == null)
            {
                Logger.Log.Error($"Failed to create table for ID {tableId}.");
                return null;
            }

            return data;
        }
        #endregion

        #region Get/Set Data Methods
        /// <summary>
        ///  Retrieves a table by its ID. Can only be used if all defined tables are registered in Tables.
        ///  Can only be used in runtime mode.
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        protected Table GetTable(int tableId)
        {
            if (TableAlias.Count != Tables.Count)
            {
                Logger.Log.Error($"GetTable can only be called if all defined tables are registered.");
                throw new InvalidOperationException("GetTable can only be called if all defined tables are registered.");
            }

            if (tableId <= 0 || tableId > Tables.Count)
            {
                Logger.Log.Error($"Table ID '{tableId}' is out of range.");
                return null;
            }

            // Table IDs start from 1, so we need to adjust the index
            int idx = tableId - 1;

            // SortedList.Values[idx] -> O(1)
            return (Table)Tables.Values[idx];
        }

        protected Data GetDataById(int tableId, int dataId)
        {
            Table table = GetTable(tableId);
            return GetOrLoadDataById(table, tableId, dataId);
        }

        private Data GetOrLoadDataById(Table table, int tableId, int dataId)
        {
            var data = GetDataById(table, dataId);
            if (data != null)
                return data; // Data already loaded

            if (!table.IsOndemand || table.GetMetaNestedData() == null)
                return null;

            data = LoadOnDemand(table, tableId, dataId: dataId);
            if (data == null)
            {
                // Failed to load OnDemand Data
                Logger.Log.Error($"Failed to load OnDemand Data for Id {dataId} in table '{table.GetType().Name}'.");
                return null;
            }

            // Successfully loaded OnDemand Data
            Logger.Log.Debug($"OnDemand: Loaded data for ID {dataId} in table '{table.GetType().Name}'.");
            SetDataById(table, data);
            return data;
        }

        private Data GetDataById(Table table, int dataId)
        {
            var data = table.GetMap().Get(dataId);
            if (data != null)
            {
                // Data already loaded
                return data;
            }

            return null;
        }

        private void SetDataById(Table table, Data data)
    {
            // Add to the regular map wrapper
            table.GetMap().Set(data.GetId(), data);
        }

        protected Data GetDataByAlias(int tableId, string dataAlias)
        {
            Table table = GetTable(tableId);
            return GetOrLoadDataByAlias(table, tableId, dataAlias);
        }

        private Data GetOrLoadDataByAlias(Table table, int tableId, string dataAlias)
        {
            var data = GetDataByAlias(table, dataAlias);
            if (data != null)
            {
                // Data already loaded
                return data;
            }

            data = LoadOnDemand(table, tableId, dataAlias: dataAlias);
            if (data == null)
            {
                // Failed to load OnDemand Data
                Logger.Log.Error($"Failed to load OnDemand Data for Alias {dataAlias} in table '{table.GetType().Name}'.");
                return null;
            }

            // Successfully loaded OnDemand Data
            Logger.Log.Debug($"OnDemand: Loaded data for Alias {dataAlias} in table '{table.GetType().Name}'.");
            SetDataByAlias(table, data);
            return data;
        }

        private Data GetDataByAlias(Table table, string dataAlias)
        {
            var data = table.GetMapAlias().Get(dataAlias);
            if (data != null)
            {
                // Data already loaded
                return data;
            }

            return null;
        }

        private void SetDataByAlias(Table table, Data data)
        {
            // Add to the regular map wrapper
            table.GetMapAlias().Set(data.GetAlias(), data);
        }

        public int GetId(string alias)
        {
            if (MetaDataTable.MapAliasData.TryGetValue(alias, out AliasData aliasData))
            {
                return aliasData.Id;
            }
            return 0;
        }

        public Table GetTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "Table name cannot be null or empty.");
            }

            if (Tables.TryGetValue(tableName, out Table table))
            {
                return table;
            }

            return null;
        }

        public void DelTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "Table name cannot be null or empty.");
            }

            if (Tables.ContainsKey(tableName))
            {
                var table = Tables[tableName];
                table.Dispose();

                Tables.Remove(tableName);
            }
        }

        public void AddTable(string tableName, Table table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), "Data table cannot be null.");
            }

            var tableId = TableAlias[tableName];
            table.IsOndemand = IsOndemandTable(tableId);
            table.IsWeakRef = IsWeakRefTable(tableId);

            // If the table is on-demand, initialize the stream for on-demand data
            if (table.IsOndemand)
            {
                table.GetMetaNestedData()?.InitStream(StreamForOndemand, TableLocation);
            }

            // If it doesn't exist, add the new data table
            Tables[tableName] = table;
        }

        private MetaData GetOrCreateMetaData(string name)
        {
            if (MetaDataTable.MapMetaData.TryGetValue(name, out MetaData metaData))
            {
                return metaData;
            }

            metaData = new MetaData
            {
                Name = name,
                Offset = 0,
            };
            MetaDataTable.MapMetaData[name] = metaData;

            return metaData;
        }
        #endregion
    }
}
