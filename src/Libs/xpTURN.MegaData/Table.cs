using System;
using System.Runtime.Serialization;

namespace xpTURN.MegaData
{
    public abstract partial class Table : Data, IDisposable
    {
        [IgnoreDataMember]
        public bool IsOndemand { get; set; } = false;

        [IgnoreDataMember]
        public bool IsWeakRef { get; set; } = false;

        [IgnoreDataMember]
        public bool IsLoaded { get; set; } = false;

        virtual public IMapIntWrapper GetMap() => NullMapIntWrapper.Default;
        virtual public IMapStringWrapper GetMapAlias() => NullMapStringWrapper.Default;

        virtual public MetaNestedData GetMetaNestedData() => null;
        
        public void Dispose() => GetMetaNestedData()?.Dispose();
    }
}