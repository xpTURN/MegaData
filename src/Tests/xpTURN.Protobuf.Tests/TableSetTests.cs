
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using xpTURN.Common;

namespace xpTURN.Protobuf.TestCases
{
    public class TableSetTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            xpTURN.MegaData.JsonWrapper.FromJsonMethod = xpTURN.Tool.Common.JsonUtils.FromJson;
            xpTURN.MegaData.JsonWrapper.ToJsonMethod = xpTURN.Tool.Common.JsonUtils.ToJson;

            //
            Logger.Log.SetLogger(new xpTURN.Tool.Common.Log(typeof(TableSetTests)), true);

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
        public void DoTableSet_Load_OnDemand()
        {
            Logger.Log.Info("DoTableSet_Load_OnDemand Test Start");

            void LoadTableSet()
            {
                var isLoaded = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.Load("./Tests/DataSet/OnDemand.Type1/[Result]/OnDemand1TableSet.bytes");
                Assert.That(isLoaded, Is.True);
            }

            void LoadData()
            {
                var data = Tests.OnDemand.Type1.OnDemand1TableSet.Instance.GetSampleData("ids_mark04");
                Assert.That(data, Is.Not.Null);
            }

            Logger.Log.Info("");
            Logger.Log.Info("Loading TableSet...");
            LoadTableSet();

            Logger.Log.Info("");
            Logger.Log.Info("Loading data... (1st time)");
            LoadData();

            Logger.Log.Info("");
            Logger.Log.Info("GC... (1st time)");
            GC.Collect();

            Logger.Log.Info("");
            Logger.Log.Info("Loading data... (2nd time)");

            Logger.Log.CaptureStart();
            LoadData();
            var logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 10004 in table")), Is.False);
#endif

            Logger.Log.Info("");
            Logger.Log.Info("GC... (2nd time)");
            GC.Collect();

            Logger.Log.Info("");
            Logger.Log.Info("Loading data... (3nd time)");

            Logger.Log.CaptureStart();
            LoadData();
            logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 10004 in table")), Is.False);
#endif

            Logger.Log.Info("DoTableSet_Load_OnDemand Test End");
        }

        [Test]
        public void DoTableSet_Load_WeakRef()
        {
            Logger.Log.Info("DoTableSet_Load_WeakRef Test Start");

            void LoadTableSet()
            {
                var isLoaded = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.Load("./Tests/DataSet/WeakRef.Type1/[Result]/WeakRef1TableSet.bytes");
                Assert.That(isLoaded, Is.True);
            }

            void LoadData()
            {
                var data = Tests.WeakRef.Type1.WeakRef1TableSet.Instance.GetSampleData(10004);
                Assert.That(data, Is.Not.Null);
            }

            Logger.Log.Info("");
            Logger.Log.Info("Loading TableSet...");
            LoadTableSet();

            Logger.Log.Info("");
            Logger.Log.Info("Loading data... (1st time)");
            LoadData();

            Logger.Log.Info("");
            Logger.Log.Info("GC... (1st time)");
            GC.Collect();

            Logger.Log.Info("");
            Logger.Log.Info("Loading data... (2nd time)");

            Logger.Log.CaptureStart();
            LoadData();
            var logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 10004 in table")), "Expected log message not found.");
#endif

            Logger.Log.Info("");
            Logger.Log.Info("GC... (2nd time)");
            GC.Collect();

            Logger.Log.Info("");
            Logger.Log.Info("Loading data... (3nd time)");

            Logger.Log.CaptureStart();
            LoadData();
            logEvents = Logger.Log.CaptureEnd();
#if DEBUG
            Assert.That(logEvents.Any(e => e.RenderedMessage.Contains("OnDemand: Loaded data for ID 10004 in table")), "Expected log message not found.");
#endif

            Logger.Log.Info("DoTableSet_Load_WeakRef Test End");
        }
    }
}