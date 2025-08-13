## Data Definition

### Simple Example
After creating a new file in your spreadsheet program, create a Define sheet and define types as shown below. You can define multiple types in one file or split them across several files. Since there is no import feature, you cannot use Message/Enum types defined elsewhere; all types must be defined and used internally.

* TableSet: Refers to a collection of all Tables defined across multiple sheets.
* Table: The unit for organizing data within a single sheet.
* Data: The unit of data records contained in a Table (e.g., a row in Excel. If NestedData exists, it may span multiple columns).
* NestedData: Sub-data records contained within Data.

|    |  A      |  B               |  C          |  D                           |  E            |  F                                |  G                                |
| -- | ------- | ---------------- | ----------- | ---------------------------- | ------------- | --------------------------------- | --------------------------------- |
|  1 |         |                  |             |                              |               |                                   |                                   |
|  2 |  Type   |  Name            |  Obsolete   |  FType                       |  ExtraOptions |  Desc                             |  #Comment                         |
|  3 |  Table  |  PersonDataTable |             |                              |               |                                   |                                   |
|  4 |  Num    |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |  #Comment                         |
|  5 |  1      |  Map             |             |  Map<SFixed32,PersonData>    |               |                                   |                                   |
|  6 |  Type   |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |  #Comment                         |
|  7 |  Data   |  PersonData      |             |                              |               |                                   |                                   |
|  8 |  Num    |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |  #Comment                         |
|  9 |  1      |  Id              |             |  SFixed32                    |               |  Data Index Id                    |                                   |
| 10 |  2      |  IdAlias         |             |  String                      |               |  Data Alias Name                  |                                   |
| 11 |  3      |  Name            |             |  String                      |               |                                   |                                   |
| 12 |  4      |  Role            |             |  RoleType                    |               |                                   |                                   |
| 13 |  5      |  Email           |             |  String                      |               |                                   |                                   |
| 14 |  6      |  Phone           |             |  String                      |               |                                   |                                   |
| 15 |  Type   |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |                                   |
| 16 |  Enum   |  RoleType        |             |                              |               |                                   |                                   |
| 17 |  Num    |  Name            |  Obsolete   |  FType                       |  ExtraOptions |                                   |                                   |
| 18 |  0      |  None            |             |                              |               |                                   |                                   |
| 19 |  1      |  Staff           |             |                              |               |                                   |                                   |
| 20 |  2      |  Manager         |             |                              |               |                                   |                                   |
| 21 |  3      |  Executive       |             |                              |               |                                   |                                   |

* [Sample Define](../src/Samples/DataSet/Sample1/[Define]/)
* [Sample Output](../src/Samples/xpTURN.TableSet.Samples/Sample1/)

#### Message Type Definition (Table, Data Type Definition)

* The first column is always ignored. You can use it for comments or leave it blank.
* Add the Type/Name/Obsolete/FType/ExtraOptions/Desc columns to indicate the start of a new type.
    - Type column: Enter either Table or Data.
    - Name column: Enter the type name.
    - Obsolete column: Specify whether the type or field is deprecated.

        + Warning: Outputs a warning in the reference code (C# `[Obsolete]` attribute).

        + Error: Causes a compile error in the reference code (C# `[Obsolete(error:true)]` attribute).

        + Delete: Does not generate code for the type or field (must not be referenced in TableSet).

    - ExtraOptions column: Additional settings (JSON format).

        + Key: Data handling key field name (specify Id field or Alias field, default is "Id").

        + Hide: When generating the TableSet class, does not generate Getter functions for Table or Data (default is "false").

        + OnDemand: Enables lazy loading when accessing Data records managed by the Table.

        + WeakRef: Manages lazily loaded Data as WeakReference; if the reference is removed, it will be collected by GC.

            + Example
                ```json
                {"Key":"PawnId", "OnDemand": true}
                ```
                ```json
                {"OnDemand": true, "WeakRef": true}
                ```

    - Desc column: Enter a description for the table; this will be generated as a C# comment.

* Add the Num/Name/Obsolete/FType/ExtraOptions columns to indicate the start of field definitions.

    - Num column: Enter the field number (must follow proto Number conventions for input and management).

    - Name column: Enter the field name.

    - Obsolete column: Set deprecation attributes (Warning/Error/Delete).

    - FType column: Enter the basic proto variable type, a user-defined Message/Enum type, or a collection type such as List or Map.

        + Basic types:
            + Bool
            + Int32, SInt32, SFixed32
            + UInt32, Fixed32
            + Int64, SInt64, SFixed64
            + UInt64, Fixed64
            + Float
            + Double
            + String
            + Bytes (input as Base64 text in the sheet)

        + Custom types:
            + DateTime (serialized internally as UInt64: Ticks | Kind << 62)
            + TimeSpan (serialized internally as Int64)
            + Guid (serialized internally as String)
            + Uri (serialized internally as String)
        
        + Message / Enum types: Defined by the user in the table definition.

        + Collection types:
            + List
            + Map

                > List\<String\>

                > Map\<Int32,String\>

    - ExtraOptions column: Additional settings (JSON format).

        + Get: Generates a Getter function (set in RefId; creates a function that returns the Data referenced by the key).

            If you set the ExtraOptions for the NameRefId field as follows:
            ```json
            {"Get": "String"}
            ```
            The following code is automatically generated:
            ```cs
            public String Name => Instance.GetString(NameRefId);
            ```
            If you set the ExtraOptions for the BoxRefId field as follows:
            ```json
            {"Get": "BoxData"}
            ```
            The following code is automatically generated:
            ```cs
            public BoxData Box => Instance.GetBoxData(BoxRefId);
            ```

    - Desc column: Enter a description for the table; this will be generated as a C# comment.

* Required conventions when using Table

    - Table definition

        + It is recommended that the name ends with "Table" (e.g., BoxDataTable).

        + The Data field must be defined as a Map collection type (Map<Int32,BoxData> or Map<String,BoxData>).

    - Data definition

        + It is recommended that the type name ends with "Data" (e.g., BoxData).

        + The Id field must be provided (NestedData is not required).
            + Id can only use Int32 (Int32, SInt32, SFixed32) or Enum types.

        + If an IdAlias field is provided, you can assign a string alias to the Data record and use it as a Key value (optional).

        + If a field ending with RefId is provided, you can access Data type record indexes defined elsewhere using the Id value.

        + If a RefIdAlias field is provided, you can access data using the Alias Key value (must be used together with RefId).

        + You can define child Nested Data using List<NestedData> or Map<Int32,NestedData> (no limit on the number of owned items, but only one can be used per sheet).

        + There is no limit on the number of single NestedData types owned.

## Etc

* Guidelines for entering Num values:
    - According to proto conventions, you can enter values between 1 and 536,870,911.

    - Note: Values between 19,000 and 19,999 are reserved by Google. Values between 18,000 and 18,999 are reserved by xpTURN.MegaData.

    - Using values between 1 and 15 will serialize as 1 byte; using values between 16 and 2047 will serialize as 2 bytes. (Optimal to use 1–15, recommended to use values below 2047)

    - Be cautious when reusing a previously used number for a different purpose.

        > Note: Refer to Google’s [Updating A Message Type](https://protobuf.dev/programming-guides/proto3/#updating) documentation.

* Fields starting with # are ignored. You can use these for comments or as intermediate data when composing data records.

* Enum type definitions use only the Type, Name, Num, and Obsolete columns.
