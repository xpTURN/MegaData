using System;
using String = System.String;
using System.Runtime.Serialization;

using xpTURN.Common;

namespace xpTURN.MegaData
{
    public abstract partial class Data
    {
        virtual public int GetId() => 0;
        virtual public string GetAlias() => String.Empty;

        public string ToJson() => JsonWrapper.ToJson(this);

        [IgnoreDataMember]
        public DebugInfo DebugInfo { get; set; }
    }
}