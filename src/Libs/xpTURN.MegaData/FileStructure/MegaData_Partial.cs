using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

using xpTURN.Common;
using xpTURN.Protobuf.Collections;

namespace xpTURN.MegaData
{
    public partial class MetaNestedData : IDisposable
    {
        [IgnoreDataMember]
        public IMapIntWrapper MapIntWrapper { get; private set; } = null;

        [IgnoreDataMember]
        public IMapStringWrapper MapStringWrapper { get; private set; } = null;

        [DataMember]
        public List<Data> ListOnDemandData { get; set; } = null;

        [IgnoreDataMember]
        public Stream StreamForOndemand { get; private set; } = null;

        [IgnoreDataMember]
        public long TableLocation { get; set; } = 0;

        /// <summary>
        /// Sets up the integer map wrapper for the MetaNestedData.
        /// Set isWeakRef to true if the data type should be managed with weak references and you are in runtime mode.
        /// </summary>
        /// <param name="mapIntWrapper"></param>
        /// <param name="isWeakRef"></param>
        public void SetupIntWrapper(IMapIntWrapper mapIntWrapper, bool isWeakRef = false)
        {
            if (isWeakRef)
                MapIntWrapper = new WeakMapIntWrapper<Data>(new Dictionary<int, WeakReference<Data>>());
            else
                MapIntWrapper = mapIntWrapper;
        }

        /// <summary>
        /// Sets up the string map wrapper for the MetaNestedData.
        /// Set isWeakRef to true if the data type should be managed with weak references and you are in runtime mode.
        /// </summary>
        /// <param name="mapStringWrapper"></param>
        /// <param name="isWeakRef"></param>
        public void SetupStringWrapper(IMapStringWrapper mapStringWrapper, bool isWeakRef = false)
        {
            if (isWeakRef)
                MapStringWrapper = new WeakMapStringWrapper<Data>(new Dictionary<string, WeakReference<Data>>());
            else
                MapStringWrapper = mapStringWrapper;
        }

        public void InitStream(Stream stream, long tableLocation)
        {
            StreamForOndemand = stream;
            TableLocation = tableLocation;
        }

        public void Dispose()
        {
            StreamForOndemand = null;
        }

        public void AddOnDemandData(Data data)
        {
            if (ListOnDemandData == null)
            {
                ListOnDemandData = new List<Data>();
            }

            ListOnDemandData.Add(data);
        }

        public void ClearOnDemandData()
        {
            if (ListOnDemandData != null)
            {
                ListOnDemandData.Clear();
                ListOnDemandData = null;
            }
        }
    }

    public partial class SubsetDataTable
    {
        string _fileName = string.Empty;

        void CheckData()
        {
            HashSet<string> tableNames = new HashSet<string>();
            foreach (var data in Map.Values)
            {
                foreach (var table in data.Tables)
                {
                    if (tableNames.Contains(table))
                    {
                        Logger.Log.Tool.Error(DebugInfo.Empty, $"Duplicate table name found: {table} in {_fileName}");
                    }
                    tableNames.Add(table);
                }
            }
        }

        public static SubsetDataTable Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return new SubsetDataTable();
            }

            try
            {
                using (var reader = File.OpenText(fileName))
                {
                    var table = JsonWrapper.FromJson(reader.ReadToEnd(), typeof(SubsetDataTable)) as SubsetDataTable;
                    table._fileName = fileName;
                    table.CheckData();
                    return table;
                }
            }
            catch
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"Failed to load SubsetDataTable from {fileName}");
            }

            return new SubsetDataTable();
        }
    }
}
