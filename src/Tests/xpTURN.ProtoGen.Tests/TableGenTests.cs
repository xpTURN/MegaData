using System;
using System.IO;
using System.Collections.Generic;

using NUnit.Framework;

using xpTURN.Common;

using static xpTURN.Common.FileUtils;

namespace xpTURN.TableGen.TestCases
{
    [TestFixture]
    internal class TableGenTests
    {

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            xpTURN.MegaData.JsonWrapper.FromJsonMethod = xpTURN.Tool.Common.JsonUtils.FromJson;
            xpTURN.MegaData.JsonWrapper.ToJsonMethod = xpTURN.Tool.Common.JsonUtils.ToJson;

            //
            Logger.Log.SetLogger(new xpTURN.Tool.Common.Log(typeof(TableGenTests)), true);

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
            xpTURN.TableGen.TableGen.Clear();
        }

        [Test]
        public void DoTableGen_MegaData_FileStructure()
        {
            Logger.Log.Info("DoTableGen_MegaData_FileStructure Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Libs/xpTURN.MegaData/[Define]/" },
                { "Output", "./Libs/xpTURN.MegaData/FileStructure/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "xpTURN.MegaData" },
                { "TableSetName", "MegaData" },
                { "OutputComment", "true" },
                { "ForDataTable", "false" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DoTableGen_AllTypes()
        {
            Logger.Log.Info("DoTableGen_AllTypes Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/AllTypes/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/AllTypes/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.AllTypes" },
                { "TableSetName", "AllTypesTableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_AllTypes Test End");
        }

        [Test]
        public void DoTableGen_Depth()
        {
            Logger.Log.Info("DoTableGen_Depth Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Depth/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Depth/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Depth" },
                { "TableSetName", "DepthTableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Depth Test End");
        }

        [Test]
        public void DoTableGen_Alias()
        {
            Logger.Log.Info("DoTableGen_Alias Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Alias/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Alias/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Alias" },
                { "TableSetName", "AliasTableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Alias Test End");
        }

        [Test]
        public void DoTableGen_RefId()
        {
            Logger.Log.Info("DoTableGen_RefId Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/RefId/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/RefId/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.RefId" },
                { "TableSetName", "RefIdTableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_RefId Test End");
        }

        [Test]
        public void DoTableGen_CustomField()
        {
            Logger.Log.Info("DoTableGen_CustomField Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/CustomField/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/CustomField/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.CustomField" },
                { "TableSetName", "CustomFieldTableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_CustomField Test End");
        }

        [Test]
        public void DoTableGen_Multi1()
        {
            Logger.Log.Info("DoTableGen_Multi1 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Multi1/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Multi1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Multi1" },
                { "TableSetName", "Multi1TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Multi1 Test End");
        }

        [Test]
        public void DoTableGen_Repeated_V0()
        {
            Logger.Log.Info("DoTableGen_Repeated_V0 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Repeated.V0/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Repeated.V0/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Repeated.V0" },
                { "TableSetName", "RepeatedTableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Repeated_V0 Test End");
        }

        [Test]
        public void DoTableGen_Repeated_V1()
        {
            Logger.Log.Info("DoTableGen_Repeated_V1 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Repeated.V1/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Repeated.V1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Repeated.V1" },
                { "TableSetName", "RepeatedTableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Repeated_V1 Test End");
        }

        [Test]
        public void DoTableGen_Sample1TableSet()
        {
            Logger.Log.Info("DoTableGen_Sample1TableSet Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Samples/DataSet/Sample1/[Define]/" },
                { "Output", "./Samples/xpTURN.TableSet.Samples/Sample1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Samples" },
                { "TableSetName", "Sample1TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Sample1TableSet Test End");
        }

        [Test]
        public void DoTableGen_OnDemand_Type1()
        {
            Logger.Log.Info("DoTableGen_OnDemand_Type1 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/OnDemand.Type1/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/OnDemand.Type1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.OnDemand.Type1" },
                { "TableSetName", "OnDemand1TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_OnDemand_Type1 Test End");
        }

        [Test]
        public void DoTableGen_WeakRef_Type1()
        {
            Logger.Log.Info("DoTableGen_WeakRef_Type1 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/WeakRef.Type1/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/WeakRef.Type1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.WeakRef.Type1" },
                { "TableSetName", "WeakRef1TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_WeakRef_Type1 Test End");
        }

        [Test]
        public void DoTableGen_Locale_Type1()
        {
            Logger.Log.Info("DoTableGen_Locale_Type1 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Locale.Type1/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Locale.Type1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Locale.Type1" },
                { "TableSetName", "Locale1TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Locale_Type1 Test End");
        }

        [Test]
        public void DoTableGen_Locale_Type2()
        {
            Logger.Log.Info("DoTableGen_Locale_Type2 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Locale.Type2/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Locale.Type2/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Locale.Type2" },
                { "TableSetName", "Locale2TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_Locale_Type2 Test End");
        }

        [Test]
        public void DoTableGen_ErrorsConvert1()
        {
            Logger.Log.Info("DoTableGen_ErrorsConvert1 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Errors.Convert1/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Errors.Convert1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Errors.Convert1" },
                { "TableSetName", "ErrorsConvert1TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_ErrorsConvert1 Test End");
        }

        [Test]
        public void DoTableGen_ErrorsConvert2()
        {
            Logger.Log.Info("DoTableGen_ErrorsConvert2 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Errors.Convert2/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Errors.Convert2/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Errors.Convert2" },
                { "TableSetName", "ErrorsConvert2TableSet" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.True);

            Logger.Log.Info("DoTableGen_ErrorsConvert2 Test End");
        }

        [Test]
        public void DoTableGen_Errors1()
        {
            Logger.Log.Info("DoTableGen_Errors1 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Errors.TableGen1/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Errors.TableGen1/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Errors.TableGen1" },
                { "TableSetName", "ErrorsTableGen1" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.False);

            Logger.Log.Info("DoTableGen_Errors1 Test End");
        }

        [Test]
        public void DoTableGen_Errors2()
        {
            Logger.Log.Info("DoTableGen_Errors2 Test Start");

            // Arrange
            var options = new Dictionary<string, string>
            {
                { "Input", "./Tests/DataSet/Errors.TableGen2/[Define]/" },
                { "Output", "./Tests/xpTURN.TableSet.ForTests/Errors.TableGen2/" },
                { "OutputType", "cs;proto" },
                { "Namespace", "Tests.Errors.TableGen2" },
                { "TableSetName", "ErrorsTableGen2" },
                { "OutputComment", "true" },
                { "ForDataTable", "true" },
                { "OutputSplit", "false" }
            };
            List<string> ignoreFiles = new()
            {
                "^~$*", // Exclude Excel backup files
                @"^#.*", // Exclude files
            };
            List<string> ignoreFolders = new()
            {
                @"\[Result\]" // Exclude Result files
            };
            var searchPatterns = new List<string> { "*.xls*" };

            List<string> inputFiles = GetTartgetFile(new List<string> { options.GetCustomOption("Input") }, searchPatterns, SearchOption.AllDirectories, ignoreFiles, ignoreFolders);

            // Act
            bool result = xpTURN.TableGen.TableGen.DoGenerate(inputFiles, options);

            // Assert
            Assert.That(result, Is.False);

            Logger.Log.Info("DoTableGen_Errors2 Test End");
        }
    }
}
