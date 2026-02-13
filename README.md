## xpTURN.MegaData Library

xpTURN.MegaData is a .NET library for defining and managing data structures and working with large datasets. You define structures and manage data mainly in Excel.

Serialization is based on Google's Protocol Buffers. xpTURN uses a customized implementation rather than the standard library; see the **Reference** section below for details.

For .proto file parsing and code generation, the protobuf-net library is used, with some modifications to protobuf-net.Reflection.

## Supported Runtimes

- .NET 8.0+
- .NET Standard 2.1 (for Unity3D IL2CPP Scripting backend)
- (C# Language Version 9.0)

## Basic Usage

For setup and integration, see [USAGE](./doc/USAGE.md).

### Defining Data Structures

Unlike typical Protocol Buffers (which use `.proto` files), xpTURN.MegaData uses an Excel **Define** sheet. The sheet acts as both documentation and the definition source. Example:

|    |  A      |  B               |  C          |  D                           |  E            |  F                                |
| -- | ------- | ---------------- | ----------- | ---------------------------- | ------------- | --------------------------------- |
|  1 |         |                  |             |                              |               |                                   |
|  2 |  Type   |  Name            |  Obsolete   |  FType                       |  ExtraOptions |  Desc                             |
|  3 |  Table  |  PersonDataTable |             |                              |               |                                   |
|  4 |  Num    |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |
|  5 |  1      |  Map             |             |  Map<SFixed32,PersonData>    |               |                                   |
|  6 |  Type   |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |
|  7 |  Table  |  PersonData      |             |                              |               |                                   |
|  8 |  Num    |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |
|  9 |  1      |  Id              |             |  SFixed32                    |               |  Data Index Id                    |
| 10 |  2      |  IdAlias         |             |  String                      |               |  Data Alias Name                  |
| 11 |  3      |  Name            |             |  String                      |               |                                   |
| 12 |  4      |  Role            |             |  RoleType                    |               |                                   |
| 13 |  5      |  Email           |             |  String                      |               |                                   |
| 14 |  6      |  Phone           |             |  String                      |               |                                   |
| 15 |  Type   |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |
| 16 |  Enum   |  RoleType        |             |                              |               |                                   |
| 17 |  Num    |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |
| 18 |  0      |  None            |             |                              |               |                                   |
| 19 |  1      |  Staff           |             |                              |               |                                   |
| 20 |  2      |  Manager         |             |                              |               |                                   |
| 21 |  3      |  Executive       |             |                              |               |                                   |

* For detailed specifications, see: [Data Definition](./doc/DEFINE.md)

### Code Generation

Generate C# and .proto from your Define sheet with xpTURN.ProtoGen. Add the generated code to your project and build it so that the resulting assembly is available to xpTURN.Converter. Example: [Sample1](./src/Samples/xpTURN.TableSet.Samples).

```sh
dotnet ./xpTURN.ProtoGen.dll --input="../../../Samples/DataSet/Sample1/[Define]" --output="../../../Samples/xpTURN.TableSet.Samples/Sample1" --output-type="cs;proto" --namespace="Samples" --tableset="Sample1TableSet" --for-datatable
```

Generated output: [Sample](./src/Samples/xpTURN.TableSet.Samples/Sample1)

### Data Input

|    |  A      |  B               |  C          |  D               |  E               |  F          |  G                 |  H               |
| -- | ------- | ---------------- | ----------- | ---------------- | ---------------- | ----------- | ------------------ | ---------------- |
|  1 |         |                  |             |                  |                  |             |                    |                  |
|  2 |         |  PersonDataTable |  Id         |  IdAlias         |  Name            |  Role       |  Email             |  Phone           |
|  3 |         |  PersonData      |  1000001    |  spot_one_001    |  Emily Parker    |  Staff      |  xxx111@zmall.com  |  (415) 555-0134  |
|  4 |         |  PersonData      |  1000002    |  spot_one_002    |  James Mitchell  |  Staff      |  xxx222@zmall.com  |  (415) 555-0135  |
|  5 |         |  PersonData      |  1000003    |  spot_one_003    |  Olivia Brooks   |  Staff      |  xxx333@zmall.com  |  (415) 555-0136  |
|  6 |         |  PersonData      |  1000004    |  spot_one_004    |  Michael Hayes   |  Manager    |  xxx333@zmall.com  |  (415) 555-0137  |
|  7 |         |  PersonData      |  1000005    |  spot_one_005    |  Sophia Bennett  |  Executive  |  xxx333@zmall.com  |  (415) 555-0138  |

* For detailed specifications, see: [Data Input](./doc/DATA.md)

### Data Processing

If you need to process data structures or values entered in the sheet file, you can implement a post processor by inheriting TableSetPostProcess. See [Locale.Type2](./src/Tests/xpTURN.TableSet.ForTests/Locale.Type2/LocaleTablePostProcess.cs) for an example.

### Data Serialization

Convert Excel/JSON data to binary for runtime. Example:
```sh
dotnet ./xpTURN.Converter.dll --input="../../../Samples/DataSet/Sample1" --output="../../../Samples/DataSet/Sample1/[Result]" --namespace="Samples" --tableset="Sample1TableSet"
```

### Runtime Usage

```cs
var personData1 = Sample1TableSet.Instance.GetPersonData(1000001);
```
```cs
var personData2 = Sample1TableSet.Instance.GetPersonData("spot_one_001");
```
```cs
var boxDataTable = Sample1TableSet.Instance.GetBoxDataTable();
foreach(var pair in boxDataTable.Map)
{
    var box = pair.Value;
    Console.WriteLine($"Box : {box.Name}");
}
```

**Note:** For tables with OnDemand or WeakRef, do not use `Table.Map` directly; use accessors such as `GetXXXData` instead.

## Reference

### Notes

Because serialization follows Protocol Buffers, changing message/field definitions can break compatibility between versions.

* Reference: (See Google's [Updating A Message Type](https://protobuf.dev/programming-guides/proto3/#updating) documentation)

### xpTURN.Protobuf Library

This is a modified version of [Google.Protobuf CSharp](https://github.com/protocolbuffers/protobuf/tree/main/csharp/src/Google.Protobuf) with minimized code. Key features include removal of Descriptor dependencies, modifications to FieldCodec, and support for custom types.

### xpTURN.ProtoGen

A tool that generates C# code from message definitions in sheets or .proto files.

The generated code is similar to the output of Google's 'protoc', but with minimized code and additional features for data management. It also supports custom settings not available in .proto syntax.

#### Differences from protoc-generated code

* No code generation for Descriptor, Json, UnknownFields; WellKnownTypes not supported
* gRPC support removed
* Uses fields instead of properties; minimizes Attribute, Const variables, and Parser code
* Uses xpFieldCodec, xpRepeatedCodec, xpMapCodec
* Uses List<>, Dictionary<> instead of RepeatedField<>, MapField<>
* Generates dedicated code for xpTURN.MegaData
* Supports xpTURN custom types: DateTime, TimeSpan, Uri, Guid (internally handled as UInt64, Int64, String, String)

#### .proto Generation

xpTURN.ProtoGen can generate .cs and .proto files from sheet documents.

#### C# Code Generation from .proto

xpTURN.ProtoGen supports generating .cs source code from .proto files. You can use the simplified C# code from xpTURN.Protobuf for network packets and similar use cases.

Note: The .proto parser uses protobuf-net.Reflection, with some modifications for customization during C# code generation.

### Optimization

* Runtime code minimizes Reflection and GC pressure.
* Save and TablePostProcess run at design time and are not performance-tuned.

## Support

If you have any questions, please [open a GitHub issue](https://github.com/xpTURN/xpTURN/issues) or contact us via [email](mailto:xpTURN@gmail.com).
