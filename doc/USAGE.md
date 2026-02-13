# Project Integration

See [SampleProj](../examples/SampleProj) for a Unity example.

### Getting the Source
Clone or download from the [repository](https://github.com/xpTURN/MegaData) and build the solution.

### Initial Project Setup
Create a TableSet project (or copy and rename [xpTURN.TableSet.Samples](../src/Samples/xpTURN.TableSet.Samples)).

### Data Structure Definition
Define your TableSet, Tables, and Data types in Excel as described in [Data Definition](./DEFINE.md).

### TableSet Source Generation
Generate C# and .proto from your Define sheets. Build the TableSet project and place its output .dll in the same directory as xpTURN.Converter (and ProtoGen) so the converter can load your types.

Example code generation:
```sh
dotnet ./xpTURN.ProtoGen.dll --input="../../../Samples/DataSet/Sample1/[Define]" --output="../../../Samples/xpTURN.TableSet.Samples/Sample1" --output-type="cs;proto" --namespace="Samples" --tableset="Sample1TableSet" --for-datatable
```

### Unity Project Integration
Include the following runtime .dll files in your project:
```sh
System.Runtime.CompilerServices.Unsafe.dll
xpTURN.Common.dll
xpTURN.MegaData.dll
xpTURN.Protobuf.dll
MyProduct.TableSet.dll
```
Note: MyProduct.TableSet.dll is an example .dll containing your own TableSet.
Note: System.Runtime.CompilerServices.Unsafe.dll is required for .NET Standard 2.1 binaries, but not needed for .NET 8.0 or higher.

### Data Input
Enter or export data in Excel or JSON following your Table definitions. See [Data Input](DATA.md).

### Data Conversion
Run the converter to produce binary files for runtime.

Example data conversion:
```sh
dotnet ./xpTURN.Converter.dll --input="../../../Samples/DataSet/Sample1" --output="../../../Samples/DataSet/Sample1/[Result]" --namespace="Samples" --tableset="Sample1TableSet"
```

### Data Loading and Usage
Load the binary data in code, for example:
```cs
// Set the logger to use Unity's Debug class
xpTURN.Common.Logger.Log.SetLogger(new xpLogger());

// Load the Sample1TableSet data
Sample1TableSet.Instance.Load($"{Application.streamingAssetsPath}/Sample1TableSet.bytes");
Sample1TableSet.Instance.LoadAdditive($"{Application.streamingAssetsPath}/Sample1TableSet.Locale.bytes");

// Set the locale for the Sample1TableSet
var cultureInfo = new CultureInfo("en-US");
Sample1TableSet.Instance.SetLocale(cultureInfo.LCID);
```
Adjust subset files (e.g. `Sample1TableSet.Locale.bytes`) and `SetLocale` to match your project. You can also configure `SetLogger` as needed.

Example of accessing data:
```cs
// Get the box data for a specific box
var boxData = Sample1TableSet.Instance.GetBoxData("box_0004");
Debug.Log($"BoxData: {boxData.Name}");
```

### Continuous Management
Data definitions and inputs change regularly during project development. Automating these tasks with a CI tool is recommended.

#### Post-Processing
To run logic after loading (e.g. locale resolution), inherit from **TableSetPostProcess**. Example: [LocaleTablePostProcess](../src/Tests/xpTURN.TableSet.ForTests/Locale.Type2/LocaleTablePostProcess.cs).

For validation, inherit from **TableSetCheckPostProcess** and implement `CheckData`.

#### Bulk Data from Internal Tools
To feed data from your own tools, export to JSON and load it via the converter. Example:

Example of saving:
```cs
var boxDataTable = new BoxDataTable();

var boxData = new BoxData();
boxData.Id = 2100001;
boxData.IdAlias = "box_1001";
boxData.NameRefIdAlias = "box_name_1001";

var boxSlot = new BoxSlot();
boxSlot.Slot = 1;
boxSlot.ItemRefIdAlias = "item_1001";
boxData.List.Add(boxSlot);

boxDataTable.GetMap().Add(boxData.Id, boxData);

JsonUtils.ToJsonFile(boxDataTable, $"{Application.dataPath}/../DataSet/BoxDataTable.json");
```
See [JsonUtils](../examples/SampleProj/Assets/Scripts/JsonUtils.cs) for reference.

#### Using Subsets
To ship some tables in separate files (e.g. locale data), add a **Subset.json** file in the root of your data folder. The converter will write those tables to separate binaries. You can define multiple subsets; each table may belong to at most one subset.

```json
{
  "$type": "xpTURN.MegaData.SubsetDataTable, xpTURN.MegaData",
  "Map": {
    "Locale": {
      "Tables": [
        "LocaleDataTable",
        "TextDataTable",
        "TranslatedDataTable"
      ]
    }
  }
}
```
Replace `"Locale"` with your subset name; list the table names under `"Tables"` that should be saved in that subset file.
