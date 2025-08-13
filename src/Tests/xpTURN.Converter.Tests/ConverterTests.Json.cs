using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using NUnit.Framework;
using xpTURN.Common;
using xpTURN.MegaData;
using xpTURN.Tool.Common;

namespace xpTURN.Converter.TestCases
{
    [TestFixture]
    public class ConverterTests_Json
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
        public void DoConvert_Tests_Json()
        {
            Logger.Log.Info("DoConvert_Tests_Json Test Start");

            // Create an instance of the TranslatedDataTable
            {
                var translatedDataTable = new Tests.RefId.TranslatedDataTable();

                var translatedData = new Tests.RefId.TranslatedData();
                translatedData.IdAlias = "box_name_1001";
                translatedData.Id = Crc32Helper.ComputeInt32(translatedData.IdAlias);
                translatedData.Map.Add("en-US", "Box 1001");

                translatedDataTable.Map.Add(translatedData.Id, translatedData);

                translatedData = new Tests.RefId.TranslatedData();
                translatedData.IdAlias = "box_name_1002";
                translatedData.Id = Crc32Helper.ComputeInt32(translatedData.IdAlias);
                translatedData.Map.Add("en-US", "Box 1002");

                translatedDataTable.Map.Add(translatedData.Id, translatedData);

                translatedData = new Tests.RefId.TranslatedData();
                translatedData.IdAlias = "item_name_1001";
                translatedData.Id = Crc32Helper.ComputeInt32(translatedData.IdAlias);
                translatedData.Map.Add("en-US", "Item 1001");

                translatedDataTable.Map.Add(translatedData.Id, translatedData);

                translatedData = new Tests.RefId.TranslatedData();
                translatedData.IdAlias = "item_name_1002";
                translatedData.Id = Crc32Helper.ComputeInt32(translatedData.IdAlias);
                translatedData.Map.Add("en-US", "Item 1002");

                translatedDataTable.Map.Add(translatedData.Id, translatedData);

                JsonUtils.ToJsonFile(translatedDataTable, "./Tests/DataSet/Json/TranslatedDataTable.json");
            }

            // Create an instance of the ItemDataTable
            {
                var itemDataTable = new Tests.RefId.ItemDataTable();

                var itemData = new Tests.RefId.ItemData();
                itemData.Id = 10001;
                itemData.IdAlias = "item_1001";
                itemData.NameRefIdAlias = "item_name_1001";
                itemData.SlotSize = 10;
                itemData.Type = Tests.RefId.ItemType.TypePosion;
                itemData.Weight = 100;

                itemDataTable.Map.Add(itemData.Id, itemData);

                itemData = new Tests.RefId.ItemData();
                itemData.Id = 10002;
                itemData.IdAlias = "item_1002";
                itemData.NameRefIdAlias = "item_name_1002";
                itemData.SlotSize = 20;
                itemData.Type = Tests.RefId.ItemType.TypePosion;
                itemData.Weight = 200;

                itemDataTable.Map.Add(itemData.Id, itemData);

                JsonUtils.ToJsonFile(itemDataTable, "./Tests/DataSet/Json/ItemDataTable.json");
            }

            // Create an instance of the BoxDataTable
            {
                var boxDataTable = new Tests.RefId.BoxDataTable();

                var boxData = new Tests.RefId.BoxData();
                boxData.Id = 2100001;
                boxData.IdAlias = "box_1001";
                boxData.NameRefIdAlias = "box_name_1001";

                var boxSlot = new Tests.RefId.BoxSlot();
                boxSlot.Slot = 1;
                boxSlot.ItemRefIdAlias = "item_1001";
                boxData.List.Add(boxSlot);

                boxDataTable.Map.Add(boxData.Id, boxData);

                boxData = new Tests.RefId.BoxData();
                boxData.Id = 2100002;
                boxData.IdAlias = "box_1002";
                boxData.NameRefIdAlias = "box_name_1002";

                boxSlot = new Tests.RefId.BoxSlot();
                boxSlot.Slot = 99;
                boxSlot.ItemRefIdAlias = "item_1002";
                boxData.List.Add(boxSlot);

                boxDataTable.Map.Add(boxData.Id, boxData);

                JsonUtils.ToJsonFile(boxDataTable, "./Tests/DataSet/Json/BoxDataTable.json");
            }

            // Arrange
            var files = FileUtils.GetTartgetFile(
                new List<string> { "./Tests/DataSet/Json/" },
                new List<string> { "*.xls*", "*.json" },
                SearchOption.AllDirectories,
                new List<string> { "^~$*", @"^#.*", @"^Subset.json" },
                new List<string> { @"\[Result\]", @"\[Define\]" }
            );
            var ARGs = new Arguments
            {
                Namespace = typeof(Tests.RefId.RefIdTableSet).Namespace,
                TableSetName = typeof(Tests.RefId.RefIdTableSet).Name,
                Output = "./Tests/DataSet/Json/[Result]/",
            };
            var loadFilePath = $"{ARGs.Output}{ARGs.TableSetName}.bytes";

            // Act
            {
                var result = Converter.Default.DoConvert(files, ARGs);
                Assert.That(result, Is.True);

                var isLoaded = Tests.RefId.RefIdTableSet.Instance.Load(loadFilePath);
                Assert.That(isLoaded, Is.True);

                var cultureInfo = new CultureInfo("en-US");
                Tests.RefId.RefIdTableSet.Instance.SetLocale(cultureInfo.LCID, false);

                var boxDataTable = Tests.RefId.RefIdTableSet.Instance.GetBoxDataTable();
                Assert.That(boxDataTable, Is.Not.Null);

                var boxData = Tests.RefId.RefIdTableSet.Instance.GetBoxData("box_1001");
                Assert.That(boxData, Is.Not.Null);

                Assert.That(boxData.Id, Is.EqualTo(2100001));
                Assert.That(boxData.List[0].Slot, Is.EqualTo(1));

            }

            Logger.Log.Info("DoConvert_Tests_Json Test End");
        }
    }
}
