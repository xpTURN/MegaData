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
    public static class xpDateTimeUtils
    {
        private const int KindShift = 62;

        public static ulong GetDateData(this DateTime dateTime)
        {
            return (ulong)dateTime.Ticks | ((ulong)(uint)dateTime.Kind << KindShift);
        }

        private static long ToTicks(ulong dateData)
        {
            return (long)(dateData & ~(ulong)(3UL << KindShift));
        }

        private static DateTimeKind ToDateTimeKind(ulong dateData)
        {
            return (DateTimeKind)((dateData >> KindShift) & 3UL);
        }

        public static DateTime ToDateTime(ulong dateData)
        {
            return new DateTime(ToTicks(dateData), ToDateTimeKind(dateData));
        }
    }
}
