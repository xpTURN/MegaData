using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using NUnit.Framework;
using xpTURN.Common;
using xpTURN.MegaData;
using xpTURN.Tool.Common;

namespace xpTURN.Converter.TestCases
{
    [TestFixture]
    public class ConverterTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            xpTURN.MegaData.JsonWrapper.FromJsonMethod = xpTURN.Tool.Common.JsonUtils.FromJson;
            xpTURN.MegaData.JsonWrapper.ToJsonMethod = xpTURN.Tool.Common.JsonUtils.ToJson;

            //
            Logger.Log.SetLogger(new xpTURN.Tool.Common.Log(typeof(ConverterTests)), true);

            //
            AssemblyUtils.LoadAllDependencies();

            //
            var path = TestContext.CurrentContext.TestDirectory;
            path = Path.Combine(path, "../");
            path = Path.Combine(path, "../");
            path = Path.Combine(path, "../");
            path = Path.Combine(path, "../");
            path = Path.GetFullPath(path);
            Environment.CurrentDirectory = path;
        }

        [SetUp]
        public void Setup()
        {
            Logger.Log.Tool.Clear();
        }

        [Test]
        public void DoConvert_Tests_AllTypes()
        {
            Logger.Log.Info("DoConvert_Tests_AllTypes Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/AllTypes/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.AllTypes.AllTypesTableSet).Namespace,
                TableSetName = typeof(Tests.AllTypes.AllTypesTableSet).Name,
                Output = "./Tests/DataSet/AllTypes/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var jsonOrg = Tests.AllTypes.AllTypesTableSet.Instance.ToJson();

            var isLoaded = Tests.AllTypes.AllTypesTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var jsonLoaded = Tests.AllTypes.AllTypesTableSet.Instance.ToJson();

            // Assert
            Assert.That(jsonOrg, Is.EqualTo(jsonLoaded));

            var data = Tests.AllTypes.AllTypesTableSet.Instance.GetAllTypesData("ids_mark04");
            Assert.That(data, Is.Not.Null);

            var info = (Tests.AllTypes.SampleInfo)data.MapInfo_3[20024];
            Assert.That(info, Is.Not.Null);

            Assert.That(info.Guid_1, Is.EqualTo(new Guid("a02ce092-0444-42a2-8fd7-530639716e7e")));
            Assert.That(info.Uri_1.ToString(), Is.EqualTo("https://xpturn.com/"));
            Assert.That(info.DateTime_1, Is.EqualTo(new DateTime(2023, 10, 12)));
            Assert.That(info.TimeSpan_1, Is.EqualTo(new TimeSpan(23, 59, 59)));
            Assert.That(info.DateTime_2[1], Is.EqualTo(new DateTime(2025, 10, 12)));

            Logger.Log.Info("DoConvert_Tests_AllTypes Test End");
        }

        [Test]
        public void DoConvert_Tests_Depth()
        {
            Logger.Log.Info("DoConvert_Tests_Depth Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Depth/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Depth.DepthTableSet).Namespace,
                TableSetName = typeof(Tests.Depth.DepthTableSet).Name,
                Output = "./Tests/DataSet/Depth/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            // Assert
            var isLoaded = Tests.Depth.DepthTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var lfirstData = Tests.Depth.DepthTableSet.Instance.GetLFirstData(10003);
            Assert.That(lfirstData, Is.Not.Null);

            Assert.That(lfirstData.F1Data.Location, Is.EqualTo("3F"));
            Assert.That(lfirstData.F2Data.Grade, Is.EqualTo("CCC"));
            Assert.That(lfirstData.SecondList[1].Second2, Is.EqualTo("sec66"));
            Assert.That(lfirstData.SecondList[1].S1Data.Name, Is.EqualTo("na6"));
            Assert.That(lfirstData.SecondList[1].S2Data.Unit, Is.EqualTo("F"));
            Assert.That(lfirstData.SecondList[1].ThirdList[2].Third2, Is.EqualTo("th2018"));
            Assert.That(lfirstData.SecondList[1].ThirdList[2].T1Data.Name, Is.EqualTo("na27"));
            Assert.That(lfirstData.SecondList[1].ThirdList[2].T2Data.Unit, Is.EqualTo("AB"));

            lfirstData = Tests.Depth.DepthTableSet.Instance.GetLFirstData(10006);
            Assert.That(lfirstData, Is.Not.Null);

            Assert.That(lfirstData.F1Data.Location, Is.EqualTo("6F"));
            Assert.That(lfirstData.F2Data.Grade, Is.EqualTo("FFF"));
            Assert.That(lfirstData.SecondList[1].Second2, Is.EqualTo("sec333"));
            Assert.That(lfirstData.SecondList[1].S1Data.Name, Is.EqualTo("na12"));
            Assert.That(lfirstData.SecondList[1].S2Data.Unit, Is.EqualTo("R"));
            Assert.That(lfirstData.SecondList[1].ThirdList[2].Third2, Is.EqualTo("th2036"));
            Assert.That(lfirstData.SecondList[1].ThirdList[2].T1Data.Name, Is.EqualTo("na45"));
            Assert.That(lfirstData.SecondList[1].ThirdList[2].T2Data.Unit, Is.EqualTo("AB"));

            var firstData = Tests.Depth.DepthTableSet.Instance.GetFirstData(10003);
            Assert.That(firstData, Is.Not.Null);

            Assert.That(firstData.F1Data.Location, Is.EqualTo("3F"));
            Assert.That(firstData.F2Data.Grade, Is.EqualTo("CCC"));
            Assert.That(firstData.SecondMap[20006].Second2, Is.EqualTo("sec66"));
            Assert.That(firstData.SecondMap[20006].S1Data.Name, Is.EqualTo("na6"));
            Assert.That(firstData.SecondMap[20006].S2Data.Unit, Is.EqualTo("F"));
            Assert.That(firstData.SecondMap[20006].ThirdMap[30018].Third2, Is.EqualTo("th2018"));
            Assert.That(firstData.SecondMap[20006].ThirdMap[30018].T1Data.Name, Is.EqualTo("na27"));
            Assert.That(firstData.SecondMap[20006].ThirdMap[30018].T2Data.Unit, Is.EqualTo("AB"));

            firstData = Tests.Depth.DepthTableSet.Instance.GetFirstData(10006);
            Assert.That(firstData, Is.Not.Null);

            Assert.That(firstData.F1Data.Location, Is.EqualTo("6F"));
            Assert.That(firstData.F2Data.Grade, Is.EqualTo("FFF"));
            Assert.That(firstData.SecondMap[20012].Second2, Is.EqualTo("sec333"));
            Assert.That(firstData.SecondMap[20012].S1Data.Name, Is.EqualTo("na12"));
            Assert.That(firstData.SecondMap[20012].S2Data.Unit, Is.EqualTo("R"));
            Assert.That(firstData.SecondMap[20012].ThirdMap[30036].Third2, Is.EqualTo("th2036"));
            Assert.That(firstData.SecondMap[20012].ThirdMap[30036].T1Data.Name, Is.EqualTo("na45"));
            Assert.That(firstData.SecondMap[20012].ThirdMap[30036].T2Data.Unit, Is.EqualTo("AB"));

            firstData = Tests.Depth.DepthTableSet.Instance.GetFirstData(10009);
            Assert.That(firstData, Is.Not.Null);

            Assert.That(firstData.F1Data.Location, Is.EqualTo("3F"));
            Assert.That(firstData.F2Data.Grade, Is.EqualTo("CCC"));
            Assert.That(firstData.SecondMap[20026].Second2, Is.EqualTo("sec66"));
            Assert.That(firstData.SecondMap[20026].S1Data.Name, Is.EqualTo("na6"));
            Assert.That(firstData.SecondMap[20026].S2Data.Unit, Is.EqualTo("F"));
            Assert.That(firstData.SecondMap[20026].ThirdMap[30058].Third2, Is.EqualTo("th2018"));
            Assert.That(firstData.SecondMap[20026].ThirdMap[30058].T1Data.Name, Is.EqualTo("na27"));
            Assert.That(firstData.SecondMap[20026].ThirdMap[30058].T2Data.Unit, Is.EqualTo("AB"));

            firstData = Tests.Depth.DepthTableSet.Instance.GetFirstData(10012);
            Assert.That(firstData, Is.Not.Null);

            Assert.That(firstData.F1Data.Location, Is.EqualTo("6F"));
            Assert.That(firstData.F2Data.Grade, Is.EqualTo("FFF"));
            Assert.That(firstData.SecondMap[20032].Second2, Is.EqualTo("sec333"));
            Assert.That(firstData.SecondMap[20032].S1Data.Name, Is.EqualTo("na12"));
            Assert.That(firstData.SecondMap[20032].S2Data.Unit, Is.EqualTo("R"));
            Assert.That(firstData.SecondMap[20032].ThirdMap[30076].Third2, Is.EqualTo("th2036"));
            Assert.That(firstData.SecondMap[20032].ThirdMap[30076].T1Data.Name, Is.EqualTo("na45"));
            Assert.That(firstData.SecondMap[20032].ThirdMap[30076].T2Data.Unit, Is.EqualTo("AB"));

            var nonFirstData = Tests.Depth.DepthTableSet.Instance.GetNonFirstData(10001);
            Assert.That(nonFirstData, Is.Not.Null);

            Assert.That(nonFirstData.F1Data.Location, Is.EqualTo("1F"));
            Assert.That(nonFirstData.F2Data.Grade, Is.EqualTo("AAA"));

            nonFirstData = Tests.Depth.DepthTableSet.Instance.GetNonFirstData(10006);
            Assert.That(nonFirstData, Is.Not.Null);

            Assert.That(nonFirstData.F1Data.Location, Is.EqualTo("6F"));
            Assert.That(nonFirstData.F2Data.Grade, Is.EqualTo("FFF"));

            Logger.Log.Info("DoConvert_Tests_Depth Test End");
        }

        [Test]
        public void DoConvert_Tests_Alias()
        {
            Logger.Log.Info("DoConvert_Tests_Alias Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Alias/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Alias.AliasTableSet).Namespace,
                TableSetName = typeof(Tests.Alias.AliasTableSet).Name,
                Output = "./Tests/DataSet/Alias/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            // Assert
            var isLoaded = Tests.Alias.AliasTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var firstData = Tests.Alias.AliasTableSet.Instance.GetFirstData("ids_f03");
            Assert.That(firstData, Is.Not.Null);

            Assert.That(firstData.SecondMap["ids_s06"].Second2, Is.EqualTo("sec66"));
            Assert.That(firstData.SecondMap["ids_s06"].ThirdMap["ids_t18"].Third2, Is.EqualTo("th2018"));

            firstData = Tests.Alias.AliasTableSet.Instance.GetFirstData("ids_f06");
            Assert.That(firstData, Is.Not.Null);

            Assert.That(firstData.SecondMap["ids_s12"].Second2, Is.EqualTo("sec333"));
            Assert.That(firstData.SecondMap["ids_s12"].ThirdMap["ids_t36"].Third2, Is.EqualTo("th2036"));

            Logger.Log.Info("DoConvert_Tests_Alias Test End");
        }

        [Test]
        public void DoConvert_Tests_RefId()
        {
            Logger.Log.Info("DoConvert_Tests_RefId Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/RefId/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.RefId.RefIdTableSet).Namespace,
                TableSetName = typeof(Tests.RefId.RefIdTableSet).Name,
                Output = "./Tests/DataSet/RefId/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var isLoaded = Tests.RefId.RefIdTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var cultureInfo = new CultureInfo("en-US");
            Tests.RefId.RefIdTableSet.Instance.SetLocale(cultureInfo.LCID, false);

            // Assert
            var boxData = Tests.RefId.RefIdTableSet.Instance.GetBoxData("box_0004");
            Assert.That(boxData, Is.Not.Null);


            foreach (var pair in Tests.RefId.RefIdTableSet.Instance.GetBoxDataTable().Map)
            {
                var box = pair.Value;
                Console.WriteLine($"Box : {box.Name}");
            }

            Assert.That(string.IsNullOrEmpty(boxData.Name), Is.False);

            Logger.Log.Info($"");
            Logger.Log.Info($"");
            Logger.Log.Info($"Box Name: {boxData.Name}");

            foreach (var box in boxData.List)
            {
                var itemData = Tests.RefId.RefIdTableSet.Instance.GetItemData(box.ItemRefId);
                Assert.That(itemData, Is.Not.Null);
                Logger.Log.Info($"Item : {itemData.ToJson()}");

                Assert.That(string.IsNullOrEmpty(itemData.Name), Is.False);
                Logger.Log.Info($"Item Name: {itemData.Name}");
            }

            var diceData = Tests.RefId.RefIdTableSet.Instance.GetDiceData("dice_box_0001");
            Assert.That(diceData, Is.Not.Null);

            Logger.Log.Info($"");
            Logger.Log.Info($"");
            Logger.Log.Info($"Dice Name: {diceData.Name}");
            foreach (var itemData in diceData.ListItem)
            {
                Assert.That(itemData, Is.Not.Null);
                Logger.Log.Info($"Item : {itemData.ToJson()}");

                Assert.That(string.IsNullOrEmpty(itemData.Name), Is.False);
                Logger.Log.Info($"Item Name: {itemData.Name}");
            }

            Logger.Log.Info("DoConvert_Tests_RefId Test End");
        }

        [Test]
        public void DoConvert_Tests_Repeated_V0_To_Repeated_V1()
        {
            Logger.Log.Info("DoConvert_Tests_Repeated_V0_To_Repeated_V1 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Repeated.V0/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Repeated.V0.RepeatedTableSet).Namespace,
                TableSetName = typeof(Tests.Repeated.V0.RepeatedTableSet).Name,
                Output = "./Tests/DataSet/Repeated.V0/[Result]/"
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            // Assert
            var dataV0 = Tests.Repeated.V0.RepeatedTableSet.Instance.GetRepeatedData("ids_mark03");
            Assert.That(dataV0, Is.Not.Null);

            var isLoaded = Tests.Repeated.V1.RepeatedTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var dataV1 = Tests.Repeated.V1.RepeatedTableSet.Instance.GetRepeatedData("ids_mark03");
            Assert.That(dataV1, Is.Not.Null);

            Assert.That(dataV0.Enum_1.ToString(), Is.EqualTo(dataV1.Enum_1[0].ToString()));
            Assert.That(dataV0.Bool_1, Is.EqualTo(dataV1.Bool_1[0]));
            Assert.That(dataV0.Int32_1, Is.EqualTo(dataV1.Int32_1[0]));
            Assert.That(dataV0.SInt32_1, Is.EqualTo(dataV1.SInt32_1[0]));
            Assert.That(dataV0.SFixed32_1, Is.EqualTo(dataV1.SFixed32_1[0]));
            Assert.That(dataV0.UInt32_1, Is.EqualTo(dataV1.UInt32_1[0]));
            Assert.That(dataV0.Fixed32_1, Is.EqualTo(dataV1.Fixed32_1[0]));
            Assert.That(dataV0.Int64_1, Is.EqualTo(dataV1.Int64_1[0]));
            Assert.That(dataV0.SInt64_1, Is.EqualTo(dataV1.SInt64_1[0]));
            Assert.That(dataV0.SFixed64_1, Is.EqualTo(dataV1.SFixed64_1[0]));
            Assert.That(dataV0.UInt64_1, Is.EqualTo(dataV1.UInt64_1[0]));
            Assert.That(dataV0.Fixed64_1, Is.EqualTo(dataV1.Fixed64_1[0]));
            Assert.That(dataV0.Float_1, Is.EqualTo(dataV1.Float_1[0]));
            Assert.That(dataV0.Double_1, Is.EqualTo(dataV1.Double_1[0]));
            Assert.That(dataV0.String_1, Is.EqualTo(dataV1.String_1[0]));
            Assert.That(dataV0.Bytes_1, Is.EqualTo(dataV1.Bytes_1[0]));
            Assert.That(dataV0.DateTime_1, Is.EqualTo(dataV1.DateTime_1[0]));
            Assert.That(dataV0.TimeSpan_1, Is.EqualTo(dataV1.TimeSpan_1[0]));
            Assert.That(dataV0.Guid_1.ToString(), Is.EqualTo(dataV1.Guid_1[0].ToString()));
            Assert.That(dataV0.Uri_1.ToString(), Is.EqualTo(dataV1.Uri_1[0].ToString()));

            Logger.Log.Info("DoConvert_Tests_Repeated_V0_To_Repeated_V1 Test End");
        }

        [Test]
        public void DoConvert_Tests_RepeatedField_V1()
        {
            Logger.Log.Info("DoConvert_Tests_RepeatedField_V1 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Repeated.V1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Repeated.V1.RepeatedTableSet).Namespace,
                TableSetName = typeof(Tests.Repeated.V1.RepeatedTableSet).Name,
                Output = "./Tests/DataSet/Repeated.V1/[Result]/"
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            // Assert
            var dataV1_1 = Tests.Repeated.V1.RepeatedTableSet.Instance.GetRepeatedData("ids_mark03");
            Assert.That(dataV1_1, Is.Not.Null);

            var isLoaded = Tests.Repeated.V1.RepeatedTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var dataV1_2 = Tests.Repeated.V1.RepeatedTableSet.Instance.GetRepeatedData("ids_mark03");
            Assert.That(dataV1_2, Is.Not.Null);

            for (int i = 0; i < 2; ++i)
            {
                Assert.That(dataV1_2.Enum_1[i].ToString(), Is.EqualTo(dataV1_1.Enum_1[i].ToString()));
                Assert.That(dataV1_2.Bool_1[i], Is.EqualTo(dataV1_1.Bool_1[i]));
                Assert.That(dataV1_2.Int32_1[i], Is.EqualTo(dataV1_1.Int32_1[i]));
                Assert.That(dataV1_2.SInt32_1[i], Is.EqualTo(dataV1_1.SInt32_1[i]));
                Assert.That(dataV1_2.SFixed32_1[i], Is.EqualTo(dataV1_1.SFixed32_1[i]));
                Assert.That(dataV1_2.UInt32_1[i], Is.EqualTo(dataV1_1.UInt32_1[i]));
                Assert.That(dataV1_2.Fixed32_1[i], Is.EqualTo(dataV1_1.Fixed32_1[i]));
                Assert.That(dataV1_2.Int64_1[i], Is.EqualTo(dataV1_1.Int64_1[i]));
                Assert.That(dataV1_2.SInt64_1[i], Is.EqualTo(dataV1_1.SInt64_1[i]));
                Assert.That(dataV1_2.SFixed64_1[i], Is.EqualTo(dataV1_1.SFixed64_1[i]));
                Assert.That(dataV1_2.UInt64_1[i], Is.EqualTo(dataV1_1.UInt64_1[i]));
                Assert.That(dataV1_2.Fixed64_1[i], Is.EqualTo(dataV1_1.Fixed64_1[i]));
                Assert.That(dataV1_2.Float_1[i], Is.EqualTo(dataV1_1.Float_1[i]));
                Assert.That(dataV1_2.Double_1[i], Is.EqualTo(dataV1_1.Double_1[i]));
                Assert.That(dataV1_2.String_1[i], Is.EqualTo(dataV1_1.String_1[i]));
                Assert.That(dataV1_2.Bytes_1[i], Is.EqualTo(dataV1_1.Bytes_1[i]));
                Assert.That(dataV1_2.DateTime_1[i], Is.EqualTo(dataV1_1.DateTime_1[i]));
                Assert.That(dataV1_2.TimeSpan_1[i], Is.EqualTo(dataV1_1.TimeSpan_1[i]));
                Assert.That(dataV1_2.Guid_1[i].ToString(), Is.EqualTo(dataV1_1.Guid_1[i].ToString()));
                Assert.That(dataV1_2.Uri_1[i].ToString(), Is.EqualTo(dataV1_1.Uri_1[i].ToString()));
            }

            Logger.Log.Info("DoConvert_Tests_RepeatedField_V1 Test End");
        }

        [Test]
        public void DoConvert_Tests_CustomField()
        {
            Logger.Log.Info("DoConvert_Tests_CustomField Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/customField/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.CustomField.CustomFieldTableSet).Namespace,
                TableSetName = typeof(Tests.CustomField.CustomFieldTableSet).Name,
                Output = "./Tests/DataSet/customField/[Result]/"
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var jsonOrg = Tests.CustomField.CustomFieldTableSet.Instance.ToJson();

            var isLoaded = Tests.CustomField.CustomFieldTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var jsonLoaded = Tests.CustomField.CustomFieldTableSet.Instance.ToJson();

            // Assert
            Assert.That(jsonOrg, Is.EqualTo(jsonLoaded));

            var data = Tests.CustomField.CustomFieldTableSet.Instance.GetCustomFieldData(10007);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.DateTime_1, Is.EqualTo(new DateTime(2025, 11, 11, 23, 59, 00)));
            Assert.That(data.TimeSpan_1, Is.EqualTo(new TimeSpan(12, 00, 00)));
            Assert.That(data.Uri_1.ToString(), Is.EqualTo("https://www.google.com/"));
            Assert.That(data.Guid_1, Is.EqualTo(new Guid("508a5cd0-584a-412a-b1a0-9605cdd4a2c1")));

            Logger.Log.Info("DoConvert_Tests_CustomField Test End");
        }

        [Test]
        public void DoConvert_Tests_Multi1()
        {
            Logger.Log.Info("DoConvert_Tests_Multi1 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Multi1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Multi1.Multi1TableSet).Namespace,
                TableSetName = typeof(Tests.Multi1.Multi1TableSet).Name,
                Output = "./Tests/DataSet/Multi1/[Result]/"
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var jsonOrg = Tests.Multi1.Multi1TableSet.Instance.ToJson();

            var isLoaded = Tests.Multi1.Multi1TableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var jsonLoaded = Tests.Multi1.Multi1TableSet.Instance.ToJson();

            Assert.That(jsonOrg, Is.EqualTo(jsonLoaded));

            var data = Tests.Multi1.Multi1TableSet.Instance.GetSampleData(10004);
            Assert.That(data, Is.Not.Null);

            var info = data.MapInfo_4[20024];

            Assert.That(info.DateTime_1, Is.EqualTo(new DateTime(2023, 10, 12)));
            Assert.That(info.TimeSpan_1, Is.EqualTo(new TimeSpan(23, 59, 59)));
            Assert.That(info.Uri_1.ToString(), Is.EqualTo("https://xpturn.com/"));
            Assert.That(info.Guid_1, Is.EqualTo(new Guid("a02ce092-0444-42a2-8fd7-530639716e7e")));

            Logger.Log.Info("DoConvert_Tests_Multi1 Test End");
        }

        [Test]
        public void DoConvert_Samples_Sample1()
        {
            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Samples/DataSet/Sample1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Samples.Sample1TableSet).Namespace,
                TableSetName = typeof(Samples.Sample1TableSet).Name,
                Input = "./Samples/DataSet/Sample1/",
                Output = "./Samples/DataSet/Sample1/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";
            var loadSubsetFilePath = $"{ARGs.Output}{ARGs.TableSetName}.Locale.bytes";

            var subsetDataTable = SubsetDataTable.Load(Path.Combine(ARGs.Input, "Subset.json"));

            // Act
            var result = Converter.Default.DoConvert(files, ARGs, subsetDataTable);
            Assert.That(result, Is.True);

            var isLoaded = Samples.Sample1TableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            isLoaded = Samples.Sample1TableSet.Instance.LoadAdditive(loadSubsetFilePath);
            Assert.That(isLoaded, Is.True);

            var cultureInfo = new CultureInfo("en-US");
            Samples.Sample1TableSet.Instance.SetLocale(cultureInfo.LCID, false);

            // Assert
            var boxData = Samples.Sample1TableSet.Instance.GetBoxData("box_0004");
            Assert.That(boxData, Is.Not.Null);

            foreach (var pair in Samples.Sample1TableSet.Instance.GetBoxDataTable().Map)
            {
                var box = pair.Value;
                Console.WriteLine($"Box : {box.Name}");
            }

            Assert.That(string.IsNullOrEmpty(boxData.Name), Is.False);

            Logger.Log.Info($"");
            Logger.Log.Info($"");
            Logger.Log.Info($"Box Name: {boxData.Name}");

            foreach (var box in boxData.List)
            {
                var itemData = Samples.Sample1TableSet.Instance.GetItemData(box.ItemRefId);
                Assert.That(itemData, Is.Not.Null);
                Logger.Log.Info($"Item : {itemData.ToJson()}");

                Assert.That(string.IsNullOrEmpty(itemData.Name), Is.False);
                Logger.Log.Info($"Item Name: {itemData.Name}");
            }

            var diceData = Samples.Sample1TableSet.Instance.GetDiceData("dice_box_0001");
            Assert.That(diceData, Is.Not.Null);

            Logger.Log.Info($"");
            Logger.Log.Info($"");
            Logger.Log.Info($"Dice Name: {diceData.Name}");
            foreach (var itemData in diceData.ListItem)
            {
                Assert.That(itemData, Is.Not.Null);
                Logger.Log.Info($"Item : {itemData.ToJson()}");

                Assert.That(string.IsNullOrEmpty(itemData.Name), Is.False);
                Logger.Log.Info($"Item Name: {itemData.Name}");
            }

            boxData = Samples.Sample1TableSet.Instance.GetBoxData(2001005);
            Assert.That(boxData, Is.Not.Null);
            Assert.That(boxData.Name, Is.EqualTo("Treasure Chest 05"));
            Assert.That(boxData.List[3].Slot, Is.EqualTo(4));
            Assert.That(boxData.List[3].Item.Weight, Is.EqualTo(10));
            Assert.That(boxData.List[3].Item.Name, Is.EqualTo("Posion"));

            Logger.Log.Info("DoConvert_Samples_Sample1 Test End");
        }

        [Test]
        public void DoConvert_Tests_OnDemand_Type1()
        {
            Logger.Log.Info("DoConvert_Tests_OnDemand_Type1 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/OnDemand.Type1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.OnDemand.Type1.OnDemand1TableSet).Namespace,
                TableSetName = typeof(Tests.OnDemand.Type1.OnDemand1TableSet).Name,
                Output = "./Tests/DataSet/OnDemand.Type1/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var jsonOrg = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.ToJson();

            var isLoaded = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var jsonLoaded = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.ToJson();

            // Assert
            Assert.That(jsonOrg, Is.EqualTo(jsonLoaded));

            var numData = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.GetNumberData(Tests.OnDemand.Type1.NumberType.STAT_POW_BASE);
            Assert.That(numData, Is.Not.Null);
            Assert.That(numData.Value, Is.EqualTo(24));

            var numsData = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.GetNumbersData(Tests.OnDemand.Type1.NumbersType.BASIC_REWARD_ITEMS_ARCHER);
            Assert.That(numsData, Is.Not.Null);
            Assert.That(numsData.Value[0], Is.EqualTo(10001));
            Assert.That(numsData.Value[5], Is.EqualTo(10053));

            var data = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.GetSampleData("ids_mark04");
            Assert.That(data, Is.Not.Null);

            var info = data.MapInfo_4[20024];
            Assert.That(info, Is.Not.Null);

            Assert.That(info.Guid_1, Is.EqualTo(new Guid("a02ce092-0444-42a2-8fd7-530639716e7e")));
            Assert.That(info.Uri_1.ToString(), Is.EqualTo("https://xpturn.com/"));
            Assert.That(info.DateTime_1, Is.EqualTo(new DateTime(2023, 10, 12)));
            Assert.That(info.TimeSpan_1, Is.EqualTo(new TimeSpan(23, 59, 59)));
            Assert.That(info.DateTime_2[1], Is.EqualTo(new DateTime(2025, 10, 12)));

            Logger.Log.Info("DoConvert_Tests_OnDemand_Type1 Test End");
        }

        [Test]
        public void DoConvert_Tests_WeakRef_Type1()
        {
            Logger.Log.Info("DoConvert_Tests_WeakRef_Type1 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/WeakRef.Type1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.WeakRef.Type1.WeakRef1TableSet).Namespace,
                TableSetName = typeof(Tests.WeakRef.Type1.WeakRef1TableSet).Name,
                Output = "./Tests/DataSet/WeakRef.Type1/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var isLoaded = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            // Assert

            Logger.Log.CaptureStart();
            var numData = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.GetNumberData(Tests.WeakRef.Type1.NumberType.STAT_POW_BASE);
            var logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 14 in table")), Is.True);
#endif
            Assert.That(numData, Is.Not.Null);
            Assert.That(numData.Value, Is.EqualTo(24));

            Logger.Log.CaptureStart();
            var numsData = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.GetNumbersData(Tests.WeakRef.Type1.NumbersType.BASIC_REWARD_ITEMS_ARCHER);
            logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 5 in table")), Is.True);
#endif
            Assert.That(numsData, Is.Not.Null);
            Assert.That(numsData.Value[0], Is.EqualTo(10001));
            Assert.That(numsData.Value[5], Is.EqualTo(10053));

            // 
            Logger.Log.CaptureStart();
            var data = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.GetSampleData("ids_mark04");
            logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 10004 in table")), Is.True);
#endif
            Assert.That(data, Is.Not.Null);
        }

        [Test]
        public void DoConvert_Tests_PrepareAll()
        {
            Logger.Log.Info("DoConvert_Tests_PrepareAll Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/WeakRef.Type1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.WeakRef.Type1.WeakRef1TableSet).Namespace,
                TableSetName = typeof(Tests.WeakRef.Type1.WeakRef1TableSet).Name,
                Output = "./Tests/DataSet/WeakRef.Type1/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var isLoaded = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.Load(loadFilePath, true);
            Assert.That(isLoaded, Is.True);

            // Assert

            Logger.Log.CaptureStart();
            var numData = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.GetNumberData(Tests.WeakRef.Type1.NumberType.STAT_POW_BASE);
            var logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 14 in table")), Is.False);
#endif
            Assert.That(numData, Is.Not.Null);
            Assert.That(numData.Value, Is.EqualTo(24));

            Logger.Log.CaptureStart();
            var numsData = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.GetNumbersData(Tests.WeakRef.Type1.NumbersType.BASIC_REWARD_ITEMS_ARCHER);
            logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 5 in table")), Is.False);
#endif
            Assert.That(numsData, Is.Not.Null);
            Assert.That(numsData.Value[0], Is.EqualTo(10001));
            Assert.That(numsData.Value[5], Is.EqualTo(10053));

            // 
            Logger.Log.CaptureStart();
            var data = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.GetSampleData("ids_mark04");
            logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 10004 in table")), Is.False);
#endif
            Assert.That(data, Is.Not.Null);
        }

        [Test]
        [TestCase("2025-06-25", new[] { 10004, 10005 })]
        [TestCase("2025-06-30", new[] { 10004, 10005 })]
        [TestCase("2025-07-01", new[] { 10003, 10004, 10005 })]
        [TestCase("2025-08-15", new[] { 10003, 10005 })]
        [TestCase("2025-08-20", new[] { 10003, 10005 })]
        public void DoConvert_Tests_Exclude_Type1(string targetDateString, int[] excludeIDs)
        {
            Logger.Log.Info($"DoConvert_Tests_Exclude_Type1 Test Start");

            Logger.Log.Info("");
            Logger.Log.Info("");
            Logger.Log.Info($"Testing with target date: {targetDateString}, Exclude IDs: {string.Join(", ", excludeIDs)}");

            // Arrange
            var targetDate = DateTime.Parse(targetDateString);
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Exclude.Type1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.CustomField.CustomFieldTableSet).Namespace,
                TableSetName = typeof(Tests.CustomField.CustomFieldTableSet).Name,
                Output = $"./Tests/DataSet/Exclude.Type1/[Result]/{targetDate:yyy-MM-dd}/",
                TargetDate = targetDate
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var jsonOrg = Tests.CustomField.CustomFieldTableSet.Instance.ToJson();

            var isLoaded = Tests.CustomField.CustomFieldTableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var jsonLoaded = Tests.CustomField.CustomFieldTableSet.Instance.ToJson();

            // Assert
            Assert.That(jsonOrg, Is.EqualTo(jsonLoaded));

            for (int id = 10001; id <= 10007; ++id)
            {
                var data = Tests.CustomField.CustomFieldTableSet.Instance.GetCustomFieldData(id);

                bool excludeId = Array.Exists(excludeIDs, e => e == id);
                Assert.That(data == null, Is.EqualTo(excludeId));
            }

            Logger.Log.Info("DoConvert_Tests_Exclude_Type1 Test End");
        }

        [Test]
        [TestCase("2025-08-14", new[] { 0 }, new[] { 0 })]
        [TestCase("2025-08-15", new[] { 10002 }, new[] { 20007, 20008, 20009, 20010, 20011, 20012 })]
        public void DoConvert_Tests_Exclude_Type2(string targetDateString, int[] excludeIDs = null, int[] excludeSubIDs = null)
        {
            Logger.Log.Info($"DoConvert_Tests_Exclude_Type2 Test Start");

            Logger.Log.Info("");
            Logger.Log.Info("");
            Logger.Log.Info($"Testing with target date: {targetDateString}, Exclude IDs: {string.Join(", ", excludeIDs)}, subIDs: {string.Join(", ", excludeSubIDs)}");

            // Arrange
            var targetDate = DateTime.Parse(targetDateString);
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Exclude.Type2/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Multi1.Multi1TableSet).Namespace,
                TableSetName = typeof(Tests.Multi1.Multi1TableSet).Name,
                Output = $"./Tests/DataSet/Exclude.Type2/[Result]/{targetDate:yyyyMMdd}/",
                TargetDate = targetDate
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            var jsonOrg = Tests.Multi1.Multi1TableSet.Instance.ToJson();

            var isLoaded = Tests.Multi1.Multi1TableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var jsonLoaded = Tests.Multi1.Multi1TableSet.Instance.ToJson();

            // Assert
            Assert.That(jsonOrg, Is.EqualTo(jsonLoaded));

            for (int id = 10001; id <= 10004; ++id)
            {
                var data = Tests.Multi1.Multi1TableSet.Instance.GetSampleData(id);

                bool excludeId = Array.Exists(excludeIDs, e => e == id);
                Assert.That(data == null, Is.EqualTo(excludeId));
            }

            Logger.Log.Info("DoConvert_Tests_Exclude_Type2 Test End");
        }

        [Test]
        public void DoConvert_Tests_Locale_Type1()
        {
            Logger.Log.Info("DoConvert_Tests_Locale_Type1 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Locale.Type1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Locale.Type1.Locale1TableSet).Namespace,
                TableSetName = typeof(Tests.Locale.Type1.Locale1TableSet).Name,
                Output = "./Tests/DataSet/Locale.Type1/[Result]/"
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            // Assert
            var isLoaded = Tests.Locale.Type1.Locale1TableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var cultureInfo = new CultureInfo("en-US");
            Tests.Locale.Type1.Locale1TableSet.Instance.SetLocale(cultureInfo.LCID);

            var textEn = Tests.Locale.Type1.Locale1TableSet.Instance.GetString("pawn_mark_four");
            Assert.That(textEn, Is.EqualTo("Cassandra"));

            cultureInfo = new CultureInfo("ko-KR");
            Tests.Locale.Type1.Locale1TableSet.Instance.SetLocale(cultureInfo.LCID);

            var textKo = Tests.Locale.Type1.Locale1TableSet.Instance.GetString("pawn_mark_four");
            Assert.That(textKo, Is.EqualTo("\uCE74\uC0B0\uB4DC\uB77C"));

            Logger.Log.Info("DoConvert_Tests_Locale_Type1 Test End");
        }

        [Test]
        public void DoConvert_Tests_Locale_Type2()
        {
            Logger.Log.Info("DoConvert_Tests_Locale_Type2 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Locale.Type2/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Locale.Type2.Locale2TableSet).Namespace,
                TableSetName = typeof(Tests.Locale.Type2.Locale2TableSet).Name,
                Output = "./Tests/DataSet/Locale.Type2/[Result]/"
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.True);

            // Assert
            var isLoaded = Tests.Locale.Type2.Locale2TableSet.Instance.Load(loadFilePath);
            Assert.That(isLoaded, Is.True);

            var cultureInfo = new CultureInfo("en-US");
            Tests.Locale.Type2.Locale2TableSet.Instance.SetLocale(cultureInfo.LCID);

            var textEn = Tests.Locale.Type2.Locale2TableSet.Instance.GetString("pawn_mark_four");
            Assert.That(textEn, Is.EqualTo("Cassandra"));

            cultureInfo = new CultureInfo("ko-KR");
            Tests.Locale.Type2.Locale2TableSet.Instance.SetLocale(cultureInfo.LCID);

            var textKo = Tests.Locale.Type2.Locale2TableSet.Instance.GetString("pawn_mark_four");
            Assert.That(textKo, Is.EqualTo("\uCE74\uC0B0\uB4DC\uB77C"));

            Logger.Log.Info("DoConvert_Tests_Locale_Type2 Test End");
        }

        [Test]
        public void DoConvert_Tests_Errors1()
        {
            Logger.Log.Info("DoConvert_Tests_Errors1 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Errors.Convert1/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Errors.Convert1.ErrorsConvert1TableSet).Namespace,
                TableSetName = typeof(Tests.Errors.Convert1.ErrorsConvert1TableSet).Name,
                Output = "./Tests/DataSet/Errors.Convert1/[Result]/"
            };

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.False);


            Logger.Log.Info("DoConvert_Tests_Errors1 Test End");
        }
        
        [Test]
        public void DoConvert_Tests_Errors2()
        {
            Logger.Log.Info("DoConvert_Tests_Errors2 Test Start");

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Errors.Convert2/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.Errors.Convert2.ErrorsConvert2TableSet).Namespace,
                TableSetName = typeof(Tests.Errors.Convert2.ErrorsConvert2TableSet).Name,
                Output = "./Tests/DataSet/Errors.Convert2/[Result]/"
            };

            // Act
            var result = Converter.Default.DoConvert(files, ARGs);
            Assert.That(result, Is.False);


            Logger.Log.Info("DoConvert_Tests_Errors2 Test End");
        }
    }
}
