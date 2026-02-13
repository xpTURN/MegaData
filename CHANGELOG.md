# Changelog

All notable changes to MegaData are documented in this file.

---

## [1.0.1] - 2026-02-13

### Changed

- **Typos & naming**: Invaild→Invalid, WriteConstructorHeader, AssginValue→AssignValue, Ondemand→OnDemand, GetTartgetFile→GetTargetFile, assamblyName→assemblyName, CENVERT→CONVERT, FileSturcture→FileStructure, fileName→FileName (Breaking), listFieldDesc→ListFieldDesc.
- **Comments**: Added or updated XML and class comments (Common, Crc32Helper, FileUtils, InvokeUtils, HashUtils, TypeUtils, etc.).
- **Logging**: Log no-op when null/after Dispose; unified console format; log on GetTypeFromJson/SubsetDataTable failure; CENVERT→CONVERT; clarified SHA256/CreateDataById and related messages.
- **Behavior & API**: Stronger null/path/resource handling. GeneratorContext/Logger Outdent IndentLevel guard. ExcelReader SheetData copy, GetCell bounds check. FileUtils signature and deduplication (Breaking). AssemblyUtils continue scanning other assemblies on ReflectionTypeLoadException. ConverterUtils use continue on duplicate. Excel extensions and xlt pattern. JsonUtils serialization settings and readonly. ConvertTypeUtils/Crc32Helper enum constraint and span. TableSet.Save stream using and instance creation failure handling. xpLimitedInputStream/xpMapCodec/xpDateTimeUtils/xpFieldHelper(Uri) null/exception/comment improvements.

### Fixed

- AssemblyUtils.GetTypesByBaseName: Continue scanning remaining assemblies when one throws.
- FileUtils: Corrected wrong index used in path checks.
- ConverterUtils: On duplicate Alias use continue instead of break.
- TableSet.Load/Save: GetMetaNestedData null, stream dispose, and instance creation failure handling.
