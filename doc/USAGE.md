# Project Integration
You can refer to [SampleProj](../examples/SampleProj) for an example of using MegaData in a Unity project.

### Getting the Source
Download the project from the [source repository](https://github.com/xpTURN/MegaData) and perform the basic build.

### Initial Project Setup
Create a TableSet project for your own project. You can refer to [xpTURN.TableSet.Samples](../src/Samples/xpTURN.TableSet.Samples) to create one yourself, or copy and rename the sample project.

### TableSet Configuration
Define the data structures you will use in your project by referring to [Data Definition](./DATA.md).

### TableSet Source Generation
Generate code based on your definitions. The generated TableSet project's .dll file should be placed in the same folder as xpTURN.ProtoGen.dll.

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
Structure your data according to the defined Table structure. See [How to Structure Data](DATA.md).

### Data Binarization
Convert your data for runtime use with the converter tool.

Example data conversion:
```sh
dotnet ./xpTURN.Converter.dll --input="../../../Samples/DataSet/Sample1" --output="../../../Samples/DataSet/Sample1/[Result]" --namespace="Samples" --tableset="Sample1TableSet"
```

### Data Loading and Usage
Write code as below to load your data:
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
Note: Adjust Subset files (Sample1TableSet.Locale.bytes) and SetLocale settings as needed for your project.
Note: SetLogger can also be configured as needed.

Example of accessing data:
```cs
// Get the box data for a specific box
var boxData = Sample1TableSet.Instance.GetBoxData("box_0004");
Debug.Log($"BoxData: {boxData.Name}");
```

### Continuous Management
Data definitions and inputs change regularly during project development. Automating these tasks with a CI tool is recommended.

#### Table Processing
If you need to process data, inherit from TableSetPostProcess. See [example](../src/Samples/xpTURN.TableSet.Samples/Sample1/LocaleTablePostProcess.cs).

For logical validation of input data, also inherit from TableSetPostProcess and write your validation code.

#### Mass Data Production / Data Integration with Internal Tools
If you generate data with internal tools, you can save it as a Json file and integrate it into TableSet as shown below.

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
If you need to distribute some tables separately, configure as below and place it in the root of your data folder. The data will be saved separately during conversion.
You can set multiple subsets, but the same table cannot be included in multiple subsets.

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
* The "Locale" part is where you enter the subset name.
* The "Tables" list contains the tables to be saved separately.
