using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;


using xpTURN.Common;
using xpTURN.Protobuf;
using xpTURN.Protobuf.Collections;
using xpTURN.MegaData;

namespace xpTURN.Protobuf.TestCases
{
    public class ProtobufTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            xpTURN.MegaData.JsonWrapper.FromJsonMethod = xpTURN.Tool.Common.JsonUtils.FromJson;
            xpTURN.MegaData.JsonWrapper.ToJsonMethod = xpTURN.Tool.Common.JsonUtils.ToJson;

            //
            Logger.Log.SetLogger(new xpTURN.Tool.Common.Log(typeof(ProtobufTests)), true);
        }

        [SetUp]
        public void Setup()
        {
            Logger.Log.Tool.Clear();
        }

        [Test]
        public void DoProtobuf_Tests_AllTypes()
        {
            Logger.Log.Info("DoProtobuf_AllTypes Test Start");

            // Arrange
            Tests.AllTypes.AllTypesDataTable tableX0 = new();
            tableX0.GetMap().Set(10001, new Tests.AllTypes.AllTypesData
            {
                Id = 10001,
                IdAlias = "ids_sample01",
                Alias_1 = "TestString1",
                Bytes_1 = ByteString.CopyFrom(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }),
                Int32_1 = 12345,
                Int64_1 = 1234567890123L,
                UInt32_1 = 123456789U,
                UInt64_1 = 1234567890123456789UL,
                SFixed32_1 = 123456,
                SFixed64_1 = 1234567890123456789L,
                Double_1 = 99.9D,
                Float_1 = 99.98F,
                Boolean_1 = true,
                Enum_1 = Tests.AllTypes.SAMPLE_NUMBER.N01_001,
                SampleInfo_1 = new Tests.AllTypes.SampleInfo { InfoId = 30001 },
                ListTextAlias_2 = new List<string> { "haha", "nana" },
                ListBytes_2 = new List<ByteString>
                {
                    ByteString.CopyFrom(new byte[] { 0x06, 0x07, 0x08 }),
                    ByteString.CopyFrom(new byte[] { 0x09, 0x0A, 0x0B })
                },
                ListInt32_2 = new List<int> { 1, 2, 3 },
                ListInt64_2 = new List<long> { 100, 200, 300 },
                ListUInt32_2 = new List<uint> { 1000, 2000, 3000 },
                ListUInt64_2 = new List<ulong> { 10000, 20000, 30000 },
                ListSFixed32_2 = new List<int> { 1, 2, 3 },
                ListSFixed64_2 = new List<long> { 100, 200, 300 },
                ListDouble_2 = new List<double> { 11.1, 22.2, 33.3 },
                ListFloat_2 = new List<float> { 1.1F, 2.2F, 3.3F },
                ListBoolean_2 = new List<bool> { true, false, true },
                ListEnum_2 = new List<Tests.AllTypes.SAMPLE_NUMBER> { Tests.AllTypes.SAMPLE_NUMBER.N02_007, Tests.AllTypes.SAMPLE_NUMBER.N01_001 },
                MapInfo_3 = new Dictionary<int, Tests.AllTypes.SampleInfo>
                {
                    { 40001, new Tests.AllTypes.SampleInfo { InfoId = 40001 } },
                    { 40002, new Tests.AllTypes.SampleInfo { InfoId = 40002 } }
                },
                MapString_3 = new Dictionary<int, string>
                {
                    { 40001, "info_0002" },
                    { 40002, "info_0003" }
                },
                MapAlias_3 = new Dictionary<string, string>
                {
                    { "40001", "alias_info_0002" },
                    { "40002", "alias_info_0003" }
                },
                MapEnum_3 = new Dictionary<Tests.AllTypes.SAMPLE_NUMBER, string>
                {
                    { Tests.AllTypes.SAMPLE_NUMBER.N02_007, "enum_info_0002" },
                    { Tests.AllTypes.SAMPLE_NUMBER.N01_001, "enum_info_0001" }
                }
            });

            // Act
            var outputX0 = new MemoryStream();
            {
                tableX0.WriteTo(outputX0);
            }

            outputX0.Position = 0; // Reset the stream position to the beginning
            Tests.AllTypes.AllTypesDataTable tableX1 = new();
            tableX1.MergeFrom(outputX0);

            outputX0.Position = 0; // Reset the stream position to the beginning
            Tests.AllTypes.EmptyDataTable tableX2 = new();
            tableX2.MergeFrom(outputX0); // Skip Field Test

            // Assert
            Assert.That(tableX0.ToJson(), Is.EqualTo(tableX1.ToJson()));
            Assert.That(tableX0.Equals(tableX1), Is.EqualTo(true));

            Logger.Log.Info($"DoProtobuf_AllTypes Test End");
        }

        [Test]
        public void DoProtobuf_WriteTo_MergeFrom()
        {
            Logger.Log.Info("DoProtobuf_WriteTo_MergeFrom Test Start");

            // Arrange
            Tests.Multi1.SampleDataTable tableX0 = new();
            tableX0.GetMap().Set(10001, new Tests.Multi1.SampleData
            {
                SampleId = 10001,
                SampleIdAlias = "ids_sample01",
                Alias_1 = "TestString1",
                Int32_1 = 12345,
                Int64_1 = 1234567890123L,
                UInt32_1 = 123456789U,
                UInt64_1 = 1234567890123456789UL,
                SFixed32_1 = 123456,
                SFixed64_1 = 1234567890123456789L,
                Double_1 = 99.9D,
                Float_1 = 99.98F,
                Boolean_1 = true,
                Enum_1 = Tests.Multi1.SAMPLE_NUMBER.N01_001,
                ListTextAlias_2 = new List<string> { "haha", "nana" },
                ListInt32_2 = new List<int> { 1, 2, 3 },
                ListInt64_2 = new List<long> { 100, 200, 300 },
                ListUInt32_2 = new List<uint> { 1000, 2000, 3000 },
                ListUInt64_2 = new List<ulong> { 10000, 20000, 30000 },
                ListSFixed32_2 = new List<int> { 1, 2, 3 },
                ListSFixed64_2 = new List<long> { 100, 200, 300 },
                ListDouble_2 = new List<double> { 11.1, 22.2, 33.3 },
                ListFloat_2 = new List<float> { 1.1F, 2.2F, 3.3F },
                ListBoolean_2 = new List<bool> { true, false, true },
                ListEnum_2 = new List<Tests.Multi1.SAMPLE_NUMBER> { Tests.Multi1.SAMPLE_NUMBER.N02_007, Tests.Multi1.SAMPLE_NUMBER.N01_001 },
                Enum_3 = Tests.Multi1.SAMPLE_NUMBER.N02_010,
                SampleInfo_4 = new Tests.Multi1.SampleInfo { InfoId = 30001 },
                MapInfo_4 = new Dictionary<int, Tests.Multi1.SampleInfo>
                {
                    { 40001, new Tests.Multi1.SampleInfo { InfoId = 40001 } },
                    { 40002, new Tests.Multi1.SampleInfo { InfoId = 40002 } }
                },
                Bytes_4 = ByteString.CopyFrom(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }),
                ListBytes_4 = new List<ByteString>
                {
                    ByteString.CopyFrom(new byte[] { 0x06, 0x07, 0x08 }),
                    ByteString.CopyFrom(new byte[] { 0x09, 0x0A, 0x0B })
                },
            });

            // Act
            var outputX0 = new MemoryStream();
            {
                tableX0.WriteTo(outputX0);
                outputX0.Position = 0; // Reset the stream position to the beginning
            }

            Tests.Multi1.SampleDataTable tableX1 = new();
            tableX1.MergeFrom(outputX0);

            // Assert
            Assert.That(tableX0.ToJson(), Is.EqualTo(tableX1.ToJson()));
            Assert.That(tableX0.Equals(tableX1), Is.EqualTo(true));

            Logger.Log.Info($"DoProtobuf_WriteTo_MergeFrom Test End");
        }

        [Test]
        public void DoProtobuf_CalculateSize()
        {
            Logger.Log.Info("DoProtobuf_CalculateSize Test Start");

            // Arrange
            Tests.Multi1.SampleDataTable tableX0 = new();
            tableX0.GetMap().Set(10001, new Tests.Multi1.SampleData
            {
                SampleId = 10001,
                SampleIdAlias = "ids_sample01",
                Alias_1 = "TestString1",
                Int32_1 = 12345,
                Int64_1 = 1234567890123L,
                UInt32_1 = 123456789U,
                UInt64_1 = 1234567890123456789UL,
                SFixed32_1 = 123456,
                SFixed64_1 = 1234567890123456789L,
                Double_1 = 99.9D,
                Float_1 = 99.98F,
                Boolean_1 = true,
                Enum_1 = Tests.Multi1.SAMPLE_NUMBER.N01_001,
                ListTextAlias_2 = new List<string> { "haha", "nana" },
                ListInt32_2 = new List<int> { 1, 2, 3 },
                ListInt64_2 = new List<long> { 100, 200, 300 },
                ListUInt32_2 = new List<uint> { 1000, 2000, 3000 },
                ListUInt64_2 = new List<ulong> { 10000, 20000, 30000 },
                ListSFixed32_2 = new List<int> { 1, 2, 3 },
                ListSFixed64_2 = new List<long> { 100, 200, 300 },
                ListDouble_2 = new List<double> { 11.1, 22.2, 33.3 },
                ListFloat_2 = new List<float> { 1.1F, 2.2F, 3.3F },
                ListBoolean_2 = new List<bool> { true, false, true },
                ListEnum_2 = new List<Tests.Multi1.SAMPLE_NUMBER> { Tests.Multi1.SAMPLE_NUMBER.N02_007, Tests.Multi1.SAMPLE_NUMBER.N01_001 },
                Enum_3 = Tests.Multi1.SAMPLE_NUMBER.N02_010,
                SampleInfo_4 = new Tests.Multi1.SampleInfo { InfoId = 30001 },
                MapInfo_4 = new Dictionary<int, Tests.Multi1.SampleInfo>
                {
                    { 40001, new Tests.Multi1.SampleInfo { InfoId = 40001 } },
                    { 40002, new Tests.Multi1.SampleInfo { InfoId = 40002 } }
                }
            });

            // Act
            var outputX0 = new MemoryStream();
            {
                tableX0.WriteTo(outputX0);
                outputX0.Position = 0; // Reset the stream position to the beginning
            }

            Tests.Multi1.SampleDataTable tableX1 = new();
            tableX1.MergeFrom(outputX0);

            // Assert
            Assert.That(tableX0.CalculateSize(), Is.EqualTo(tableX1.CalculateSize()));
            Assert.That(tableX0.CalculateSize(), Is.EqualTo(outputX0.Length));

            Logger.Log.Info($"DoProtobuf_CalculateSize Test End");
        }

        [Test]
        public void DoProtobuf_MergeFrom()
        {
            Logger.Log.Info("DoProtobuf_MergeFrom Test Start");

            // Arrange
            Tests.Multi1.SampleDataTable tableX0 = new();
            tableX0.GetMap().Set(10001, new Tests.Multi1.SampleData
            {
                SampleId = 10001,
                SampleIdAlias = "ids_sample01",
                Alias_1 = "TestString1",
                Int32_1 = 12345,
                Int64_1 = 1234567890123L,
                UInt32_1 = 123456789U,
                UInt64_1 = 1234567890123456789UL,
                SFixed32_1 = 123456,
                SFixed64_1 = 1234567890123456789L,
                Double_1 = 99.9D,
                Float_1 = 99.98F,
                Boolean_1 = true,
                Enum_1 = Tests.Multi1.SAMPLE_NUMBER.N01_001,
                ListTextAlias_2 = new List<string> { "haha", "nana" },
                ListInt32_2 = new List<int> { 1, 2, 3 },
                ListInt64_2 = new List<long> { 100, 200, 300 },
                ListUInt32_2 = new List<uint> { 1000, 2000, 3000 },
                ListUInt64_2 = new List<ulong> { 10000, 20000, 30000 },
                ListSFixed32_2 = new List<int> { 1, 2, 3 },
                ListSFixed64_2 = new List<long> { 100, 200, 300 },
                ListDouble_2 = new List<double> { 11.1, 22.2, 33.3 },
                ListFloat_2 = new List<float> { 1.1F, 2.2F, 3.3F },
                ListBoolean_2 = new List<bool> { true, false, true },
                ListEnum_2 = new List<Tests.Multi1.SAMPLE_NUMBER> { Tests.Multi1.SAMPLE_NUMBER.N02_007, Tests.Multi1.SAMPLE_NUMBER.N01_001 },
                Enum_3 = Tests.Multi1.SAMPLE_NUMBER.N02_010,
                SampleInfo_4 = new Tests.Multi1.SampleInfo { InfoId = 30001 },
                MapInfo_4 = new Dictionary<int, Tests.Multi1.SampleInfo>
                {
                    { 40001, new Tests.Multi1.SampleInfo { InfoId = 40001 } },
                    { 40002, new Tests.Multi1.SampleInfo { InfoId = 40002 } }
                }
            });

            Tests.Multi1.SampleDataTable tableX1 = new();
            tableX1.MergeFrom(tableX0);

            // Assert
            Assert.That(tableX0.ToJson(), Is.EqualTo(tableX1.ToJson()));
            Assert.That(tableX0.CalculateSize(), Is.EqualTo(tableX1.CalculateSize()));

            Logger.Log.Info($"DoProtobuf_MergeFrom Test End");
        }

        [Test]
        public void DoProtobuf_Clone()
        {
            Logger.Log.Info("DoProtobuf_Clone Test Start");

            // Arrange
            Tests.Multi1.SampleDataTable tableX0 = new();
            tableX0.GetMap().Set(10001, new Tests.Multi1.SampleData
            {
                SampleId = 10001,
                SampleIdAlias = "ids_sample01",
                Alias_1 = "TestString1",
                Int32_1 = 12345,
                Int64_1 = 1234567890123L,
                UInt32_1 = 123456789U,
                UInt64_1 = 1234567890123456789UL,
                SFixed32_1 = 123456,
                SFixed64_1 = 1234567890123456789L,
                Double_1 = 99.9D,
                Float_1 = 99.98F,
                Boolean_1 = true,
                Enum_1 = Tests.Multi1.SAMPLE_NUMBER.N01_001,
                ListTextAlias_2 = new List<string> { "haha", "nana" },
                ListInt32_2 = new List<int> { 1, 2, 3 },
                ListInt64_2 = new List<long> { 100, 200, 300 },
                ListUInt32_2 = new List<uint> { 1000, 2000, 3000 },
                ListUInt64_2 = new List<ulong> { 10000, 20000, 30000 },
                ListSFixed32_2 = new List<int> { 1, 2, 3 },
                ListSFixed64_2 = new List<long> { 100, 200, 300 },
                ListDouble_2 = new List<double> { 11.1, 22.2, 33.3 },
                ListFloat_2 = new List<float> { 1.1F, 2.2F, 3.3F },
                ListBoolean_2 = new List<bool> { true, false, true },
                ListEnum_2 = new List<Tests.Multi1.SAMPLE_NUMBER> { Tests.Multi1.SAMPLE_NUMBER.N02_007, Tests.Multi1.SAMPLE_NUMBER.N01_001 },
                Enum_3 = Tests.Multi1.SAMPLE_NUMBER.N02_010,
                SampleInfo_4 = new Tests.Multi1.SampleInfo { InfoId = 30001 },
                MapInfo_4 = new Dictionary<int, Tests.Multi1.SampleInfo>
                {
                    { 40001, new Tests.Multi1.SampleInfo { InfoId = 40001 } },
                    { 40002, new Tests.Multi1.SampleInfo { InfoId = 40002 } }
                }
            });

            // Act
            var tableX1 = tableX0.Clone();

            var tableX2 = tableX1.Clone();
            var sampleData = tableX2.Map[10001];
            var info = sampleData.MapInfo_4[40001];
            info.DateTime_1 = DateTime.Now; // Modify a field to create a difference

            // Assert
            Assert.That(tableX0.Equals(tableX1), Is.EqualTo(true));
            Assert.That(tableX0.Equals(tableX2), Is.Not.EqualTo(true));

            Logger.Log.Info($"DoProtobuf_Clone Test End");
        }

        [Test]
        public void DoProtobuf_Equals()
        {
            Logger.Log.Info("DoProtobuf_Equals Test Start");

            // Arrange
            Tests.Multi1.SampleDataTable tableX0 = new();
            tableX0.GetMap().Set(10001, new Tests.Multi1.SampleData
            {
                SampleId = 10001,
                SampleIdAlias = "ids_sample01",
                Alias_1 = "TestString1",
                Int32_1 = 12345,
                Int64_1 = 1234567890123L,
                UInt32_1 = 123456789U,
                UInt64_1 = 1234567890123456789UL,
                SFixed32_1 = 123456,
                SFixed64_1 = 1234567890123456789L,
                Double_1 = 99.9D,
                Float_1 = 99.98F,
                Boolean_1 = true,
                Enum_1 = Tests.Multi1.SAMPLE_NUMBER.N01_001,
                ListTextAlias_2 = new List<string> { "haha", "nana" },
                ListInt32_2 = new List<int> { 1, 2, 3 },
                ListInt64_2 = new List<long> { 100, 200, 300 },
                ListUInt32_2 = new List<uint> { 1000, 2000, 3000 },
                ListUInt64_2 = new List<ulong> { 10000, 20000, 30000 },
                ListSFixed32_2 = new List<int> { 1, 2, 3 },
                ListSFixed64_2 = new List<long> { 100, 200, 300 },
                ListDouble_2 = new List<double> { 11.1, 22.2, 33.3 },
                ListFloat_2 = new List<float> { 1.1F, 2.2F, 3.3F },
                ListBoolean_2 = new List<bool> { true, false, true },
                ListEnum_2 = new List<Tests.Multi1.SAMPLE_NUMBER> { Tests.Multi1.SAMPLE_NUMBER.N02_007, Tests.Multi1.SAMPLE_NUMBER.N01_001 },
                Enum_3 = Tests.Multi1.SAMPLE_NUMBER.N02_010,
                SampleInfo_4 = new Tests.Multi1.SampleInfo { InfoId = 30001 },
                MapInfo_4 = new Dictionary<int, Tests.Multi1.SampleInfo>
                {
                    { 40001, new Tests.Multi1.SampleInfo { InfoId = 40001 } },
                    { 40002, new Tests.Multi1.SampleInfo { InfoId = 40002 } }
                }
            });

            // Act
            var tableX1 = tableX0.Clone();
            var tableX2 = tableX0.Clone();

            var info = tableX2.Map[10001].MapInfo_4[40001];
            info.DateTime_2.Add(DateTime.Now); // Modify a field to create a difference

            // Assert
            Assert.That(tableX0.Equals(tableX1), Is.EqualTo(true));
            Assert.That(tableX0.Equals(tableX2), Is.Not.EqualTo(true));

            Logger.Log.Info($"DoProtobuf_Equals Test End");
        }

        [Test]
        public void DoProtobuf_HashCode()
        {
            Logger.Log.Info("DoProtobuf_HashCode Test Start");

            // Arrange
            Tests.Multi1.SampleDataTable tableX0 = new();
            tableX0.GetMap().Set(10001, new Tests.Multi1.SampleData
            {
                SampleId = 10001,
                SampleIdAlias = "ids_sample01",
                Alias_1 = "TestString1",
                Int32_1 = 12345,
                Int64_1 = 1234567890123L,
                UInt32_1 = 123456789U,
                UInt64_1 = 1234567890123456789UL,
                SFixed32_1 = 123456,
                SFixed64_1 = 1234567890123456789L,
                Double_1 = 99.9D,
                Float_1 = 99.98F,
                Boolean_1 = true,
                Enum_1 = Tests.Multi1.SAMPLE_NUMBER.N01_001,
                ListTextAlias_2 = new List<string> { "haha", "nana" },
                ListInt32_2 = new List<int> { 1, 2, 3 },
                ListInt64_2 = new List<long> { 100, 200, 300 },
                ListUInt32_2 = new List<uint> { 1000, 2000, 3000 },
                ListUInt64_2 = new List<ulong> { 10000, 20000, 30000 },
                ListSFixed32_2 = new List<int> { 1, 2, 3 },
                ListSFixed64_2 = new List<long> { 100, 200, 300 },
                ListDouble_2 = new List<double> { 11.1, 22.2, 33.3 },
                ListFloat_2 = new List<float> { 1.1F, 2.2F, 3.3F },
                ListBoolean_2 = new List<bool> { true, false, true },
                ListEnum_2 = new List<Tests.Multi1.SAMPLE_NUMBER> { Tests.Multi1.SAMPLE_NUMBER.N02_007, Tests.Multi1.SAMPLE_NUMBER.N01_001 },
                Enum_3 = Tests.Multi1.SAMPLE_NUMBER.N02_010,
                SampleInfo_4 = new Tests.Multi1.SampleInfo { InfoId = 30001 },
                MapInfo_4 = new Dictionary<int, Tests.Multi1.SampleInfo>
                {
                    { 40001, new Tests.Multi1.SampleInfo { InfoId = 40001 } },
                    { 40002, new Tests.Multi1.SampleInfo { InfoId = 40002 } }
                }
            });

            // Act
            var tableX1 = tableX0.Clone();

            var tableX2 = tableX0.Clone();
            var info = tableX2.Map[10001].MapInfo_4[40001];
            info.DateTime_1 = DateTime.Now; // Modify a field to create a difference

            var tableX3 = tableX0.Clone();
            var data = tableX3.Map[10001];
            data.SampleIdAlias = "ids_sample02"; // Modify a different field to create another difference

            var tableX4 = tableX0.Clone();
            data = tableX4.GetMap()[10001] as Tests.Multi1.SampleData;
            data.ListUInt32_2[0] = 9999; // Modify another field to create a difference

            var tableX5 = tableX0.Clone();
            data = tableX5.GetMap()[10001] as Tests.Multi1.SampleData;
            data.Float_3 = 333.456F; // Modify another field to create a difference

            // Assert
            Assert.That(tableX0.GetHashCode(), Is.EqualTo(tableX1.GetHashCode()));

            Assert.That(tableX0.GetHashCode(), Is.Not.EqualTo(tableX2.GetHashCode()));

            Assert.That(tableX0.GetHashCode(), Is.Not.EqualTo(tableX3.GetHashCode()));

            Assert.That(tableX0.GetHashCode(), Is.Not.EqualTo(tableX4.GetHashCode()));

            Assert.That(tableX0.GetHashCode(), Is.Not.EqualTo(tableX5.GetHashCode()));

            Logger.Log.Info($"DoProtobuf_HashCode Test End");
        }

        [Test]
        public void DoProtobuf_Type_DateTime()
        {
            Logger.Log.Info("DoProtobuf_Type_DateTime Test Start");

            // Arrange
            Tests.Multi1.SampleInfo infoX0 = new();
            infoX0.DateTime_1 = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc);
            infoX0.DateTime_2.AddRange(new List<DateTime> { new DateTime(2024, 10, 1, 12, 0, 0, DateTimeKind.Local), new DateTime(2025, 10, 1, 12, 0, 0, DateTimeKind.Local) });

            // Act
            var outputX0 = new MemoryStream();
            {
                infoX0.WriteTo(outputX0);
                outputX0.Position = 0; // Reset the stream position to the beginning
            }
            Tests.Multi1.SampleInfo infoX1 = new();
            infoX1.MergeFrom(outputX0);

            // Assert
            Assert.That(infoX0.ToJson(), Is.EqualTo(infoX1.ToJson()));
            Assert.That(infoX0.Equals(infoX1), Is.EqualTo(true));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(infoX1.CalculateSize()));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(outputX0.Length));
            Assert.That(infoX0.DateTime_1, Is.EqualTo(infoX1.DateTime_1));
            Assert.That(infoX0.DateTime_2.Count, Is.EqualTo(infoX1.DateTime_2.Count));
            Assert.That(infoX0.DateTime_2[0], Is.EqualTo(infoX1.DateTime_2[0]));
            Assert.That(infoX0.DateTime_2[1], Is.EqualTo(infoX1.DateTime_2[1]));
            Assert.That(infoX0.GetHashCode(), Is.EqualTo(infoX1.GetHashCode()));

            Logger.Log.Info($"DoProtobuf_Type_DateTime Test End");
        }

        [Test]
        public void DoProtobuf_Type_TimeSpan()
        {
            Logger.Log.Info("DoProtobuf_Type_TimeSpan Test Start");

            // Arrange
            Tests.Multi1.SampleInfo infoX0 = new();
            infoX0.TimeSpan_1 = new TimeSpan(1, 2, 3, 4, 5);
            infoX0.TimeSpan_2.AddRange(new List<TimeSpan> { new TimeSpan(5, 4, 3, 2, 1), new TimeSpan(6, 7, 8, 9, 10) });

            // Act
            var outputX0 = new MemoryStream();
            {
                infoX0.WriteTo(outputX0);
                outputX0.Position = 0; // Reset the stream position to the beginning
            }
            Tests.Multi1.SampleInfo infoX1 = new();
            infoX1.MergeFrom(outputX0);

            // Assert
            Assert.That(infoX0.ToJson(), Is.EqualTo(infoX1.ToJson()));
            Assert.That(infoX0.Equals(infoX1), Is.EqualTo(true));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(infoX1.CalculateSize()));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(outputX0.Length));
            Assert.That(infoX0.TimeSpan_1, Is.EqualTo(infoX1.TimeSpan_1));
            Assert.That(infoX0.TimeSpan_2.Count, Is.EqualTo(infoX1.TimeSpan_2.Count));
            Assert.That(infoX0.TimeSpan_2[0], Is.EqualTo(infoX1.TimeSpan_2[0]));
            Assert.That(infoX0.TimeSpan_2[1], Is.EqualTo(infoX1.TimeSpan_2[1]));
            Assert.That(infoX0.GetHashCode(), Is.EqualTo(infoX1.GetHashCode()));

            Logger.Log.Info($"DoProtobuf_Type_TimeSpan Test End");
        }

        [Test]
        public void DoProtobuf_Type_Uri()
        {
            Logger.Log.Info("DoProtobuf_Type_Uri Test Start");
            // Arrange
            Tests.Multi1.SampleInfo infoX0 = new();
            infoX0.Uri_1 = new Uri("https://xpturn.com/mediawiki/index.php");
            infoX0.Uri_2.AddRange(new List<Uri> { new Uri("https://xpturn.com/mediawiki/index.php?title=C#_Code_Conventions"),
                new Uri("https://xpturn.com/mediawiki/index.php?title=Unity_Indepth"),
                new Uri("https://xpturn.com/mediawiki/index.php?title=Unity_Indepth"),
                new Uri("https://xpturn.com/mediawiki/index.php?title=Unity_Indepth") });

            // Act
            var outputX0 = new MemoryStream();
            {
                infoX0.WriteTo(outputX0);
                outputX0.Position = 0; // Reset the stream position to the beginning
            }
            Tests.Multi1.SampleInfo infoX1 = new();
            infoX1.MergeFrom(outputX0);

            //
            Logger.Log.Info($"infoX0.CalculateSize(): {infoX0.CalculateSize()}");
            Logger.Log.Info($"infoX1.CalculateSize(): {infoX1.CalculateSize()}");
            Logger.Log.Info($"outputX0.Length: {outputX0.Length}");

            // Assert
            Assert.That(infoX0.ToJson(), Is.EqualTo(infoX1.ToJson()));
            Assert.That(infoX0.Equals(infoX1), Is.EqualTo(true));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(infoX1.CalculateSize()));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(outputX0.Length));
            Assert.That(infoX0.Uri_1, Is.EqualTo(infoX1.Uri_1));
            Assert.That(infoX0.Uri_2.Count, Is.EqualTo(infoX1.Uri_2.Count));
            Assert.That(infoX0.Uri_2[0], Is.EqualTo(infoX1.Uri_2[0]));
            Assert.That(infoX0.Uri_2[1], Is.EqualTo(infoX1.Uri_2[1]));
            Assert.That(infoX0.GetHashCode(), Is.EqualTo(infoX1.GetHashCode()));

            Logger.Log.Info($"DoProtobuf_Type_Uri Test End");
        }

        [Test]
        public void DoProtobuf_Type_Guid()
        {
            Logger.Log.Info("DoProtobuf_Type_Guid Test Start");

            // Arrange
            Tests.Multi1.SampleInfo infoX0 = new();
            infoX0.Guid_1 = Guid.NewGuid();
            infoX0.Guid_2.AddRange(new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });

            // Act
            var outputX0 = new MemoryStream();
            {
                infoX0.WriteTo(outputX0);
                outputX0.Position = 0; // Reset the stream position to the beginning
            }
            Tests.Multi1.SampleInfo infoX1 = new();
            infoX1.MergeFrom(outputX0);

            //
            Logger.Log.Info($"infoX0.CalculateSize(): {infoX0.CalculateSize()}");
            Logger.Log.Info($"infoX1.CalculateSize(): {infoX1.CalculateSize()}");
            Logger.Log.Info($"outputX0.Length: {outputX0.Length}");

            // Assert
            Assert.That(infoX0.ToJson(), Is.EqualTo(infoX1.ToJson()));
            Assert.That(infoX0.Equals(infoX1), Is.EqualTo(true));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(infoX1.CalculateSize()));
            Assert.That(infoX0.CalculateSize(), Is.EqualTo(outputX0.Length));
            Assert.That(infoX0.Guid_1, Is.EqualTo(infoX1.Guid_1));
            Assert.That(infoX0.Guid_2.Count, Is.EqualTo(infoX1.Guid_2.Count));
            Assert.That(infoX0.Guid_2[0], Is.EqualTo(infoX1.Guid_2[0]));
            Assert.That(infoX0.Guid_2[1], Is.EqualTo(infoX1.Guid_2[1]));
            Assert.That(infoX0.GetHashCode(), Is.EqualTo(infoX1.GetHashCode()));

            Logger.Log.Info($"DoProtobuf_Type_Guid Test End");
        }

        [Test]
        public void DoProtobuf_CustomFieldDataTable()
        {
            Logger.Log.Info("DoProtobuf_CustomFieldDataTable Test Start");

            // Arrange
            Tests.CustomField.CustomFieldDataTable table = new();
            table.GetMap().Set(10001,
                new Tests.CustomField.CustomFieldData
                {
                    Id = 10001,
                    DateTime_1 = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc),
                    TimeSpan_1 = new TimeSpan(1, 2, 3, 4, 5),
                    Uri_1 = new Uri("https://xpturn.com/mediawiki/index.php"),
                    Guid_1 = Guid.NewGuid(),
                });
            table.GetMap().Set(10002,
                new Tests.CustomField.CustomFieldData
                {
                    Id = 10002,
                    DateTime_1 = new DateTime(2024, 10, 1, 12, 0, 0, DateTimeKind.Utc),
                    TimeSpan_1 = new TimeSpan(5, 4, 3, 2, 1),
                    Uri_1 = new Uri("https://xpturn.com/mediawiki/index.php?title=C#_Code_Conventions"),
                    Guid_1 = Guid.NewGuid(),
                });

            // Act
            var output = new MemoryStream();
            {
                table.WriteTo(output);
                output.Position = 0; // Reset the stream position to the beginning
            }

            Tests.CustomField.CustomFieldDataTable table1 = new();
            table1.MergeFrom(output);

            // Assert
            Assert.That(table.Equals(table1), Is.EqualTo(true));
            Assert.That(table.ToJson(), Is.EqualTo(table1.ToJson()));

            Logger.Log.Info($"DoProtobuf_CustomFieldDataTable Test End");
        }
    }
}