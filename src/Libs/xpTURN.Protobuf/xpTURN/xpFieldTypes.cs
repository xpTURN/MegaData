#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion
using System;

namespace xpTURN.Protobuf
{
    public enum xpFieldTypes
    {
        Type_Unknown,

        Type_Enum, // int32, Variable size
        Type_Bool, // byte, 1Bytes Fixed Length

        Type_Int32, // int, Variable size
        Type_SInt32, // int, Variable size, ZigZag encoding
        Type_SFixed32, // int, 4Bytes Fixed Length

        Type_UInt32, // uint, Variable size
        Type_Fixed32, // uInt, 4Bytes Fixed Length

        Type_Int64, // long, Variable size
        Type_SInt64, // long, Variable size, ZigZag encoding
        Type_SFixed64, // long, 8Bytes Fixed Length

        Type_UInt64, // ulong, Variable size
        Type_Fixed64, // ulong, 8Bytes Fixed Length

        Type_Float, // float, 4Bytes Fixed Length
        Type_Double, // double, 8Bytes Fixed Length

        Type_String, // string, Length Delimited
        Type_Bytes, // bytes, Length Delimited
        Type_Message, // Message, Length Delimited

        // Special Types
        Type_DateTime, // ulong(UInt64), DateTime.Ticks | DateTime.Kind << 62
        Type_TimeSpan, // long(Int64), TimeSpan.Ticks
        Type_Guid, // string(String), Length Delimited, Guid.ToString("D") format
        Type_Uri, // string(String), Length Delimited, Uri.ToString() format

        // Type Lengths
        Type_Max,
    }
}
