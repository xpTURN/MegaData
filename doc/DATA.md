## Data Entry

### Organizing Data with Spreadsheets

|    |  A      |  B               |  C          |  D               |  E               |  F          |  G                 |  H               |
| -- | ------- | ---------------- | ----------- | ---------------- | ---------------- | ----------- | ------------------ | ---------------- |
|  1 |         |                  |             |                  |                  |             |                    |                  |
|  2 |         |  PersonDataTable |  Id         |  IdAlias         |  Name            |  Role       |  Email             |  Phone           |
|  3 |         |  PersonData      |  1000001    |  spot_one_001    |  Emily Parker    |  Staff      |  xxx111@zmall.com  |  (415) 555-0134  |
|  4 |         |  PersonData      |  1000002    |  spot_one_002    |  James Mitchell  |  Staff      |  xxx222@zmall.com  |  (415) 555-0135  |
|  5 |         |  PersonData      |  1000003    |  spot_one_003    |  Olivia Brooks   |  Staff      |  xxx333@zmall.com  |  (415) 555-0136  |
|  6 |         |  PersonData      |  1000004    |  spot_one_004    |  Michael Hayes   |  Manager    |  xxx333@zmall.com  |  (415) 555-0137  |
|  7 |         |  PersonData      |  1000005    |  spot_one_005    |  Sophia Bennett  |  Executive  |  xxx333@zmall.com  |  (415) 555-0138  |

* [DataSet Sample](../src/Samples/DataSet/Sample1/)

After creating a new file in your spreadsheet program, create a Table sheet and enter your data.

* The first column is reserved for comments in the sheet (field descriptions or human-readable field names).

* The first row can contain special fields for data exclusion options.

    - Enter in the format Add_yyyyMMdd:
        + Data is activated if the TargetDate is the same as or later than the specified date.

    - Enter in the format Del_yyyyMMdd:
        + Data is deactivated if the TargetDate is the same as or later than the specified date.

    - Enter del:
        + The record is always deactivated.

    - If the field value is empty, the record is always activated.

    - TargetDate can be specified as a command-line argument for xpTURN.Converter.

    - These options can only be set for main Data (not for NestedData levels).

* Enter the table type name in cell B2.

* Enter the field names for the table type in cells B3:B%. (Order is not important, except for NestedData input rules.)

    - Fields starting with # are ignored. These can be used for comments or intermediate data during record composition.

* Enter the data record type owned by the table type in cells B3:%3.

* From cell C3 onward, enter values corresponding to each field variable.

* Special field value specifications
    - The Id or Alias field must be provided and is used as the access key for data records.
    - If the Id field is omitted and only the IdAlias field is present in the sheet, the Id value will be automatically generated using the Crc32 algorithm. (Use this only in unavoidable cases and with caution.)
        + The IdAlias value will be cleared after collection and initialization.
    - If RefId and RefIdAlias fields are defined and only RefIdAlias is present in the sheet (RefId value will be automatically found and entered)
        + The RefIdAlias value will be used to find the RefId value and then cleared after initialization.
    - The Id field value must be unique within the same Table.
    - The IdAlias field value must be unique across the global TableSet.

* List basic type input method (such as List\<Int32\> or List\<String\>):
    - You can enter the List field name multiple times to add multiple values.
    - If the field value is empty, it will not be added.

|    |  A      |  B            |  C          |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | ------------- | ----------- | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |               |             |                  |                  |                  |                    |                  |
|  2 |         |  BoxDataTable |  Id         |  IdAlias         |  ListItemID      |  ListItemID      |  ListItemID        |  ListItemID      |
|  3 |         |  BoxData      |  1000001    |  box_001         |  9000001         |  9000002         |  9000003           |  9000004         |
|  4 |         |  BoxData      |  1000002    |  box_002         |  9000011         |  9000012         |  9000013           |                  |
|  5 |         |  BoxData      |  1000003    |  box_003         |  9000021         |  9000022         |                    |                  |

* Map basic type input method (such as Map\<String, String\> or Map\<Int32, String\>):
    - Enter the field name as MapFieldName<key>. If a field value is provided, the data will be added using Add(key, value).

|    |  A      |  B                   |  C                 |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | -------------------- | ------------------ | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |                      |                    |                  |                  |                  |                    |                  |
|  2 |         |  TranslatedDataTable |  IdAlias           |  Map\<en-US\>    |  Map\<ko-KR\>    |  Map\<ja-JP\>    |  Map\<zh-CN\>      |  Map\<zh-TW\>    |
|  3 |         |  TranslatedData      |  ids_pawn_name_01  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  4 |         |  TranslatedData      |  ids_pawn_name_02  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  5 |         |  TranslatedData      |  ids_pawn_name_03  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |

#### Entering NestedData Types

##### Example Data Sheet Structure
|    |  A      |  B                  |  C          |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | ------------------- | ----------- | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |                     |             |                  |                  |                  |                    |                  |
|  2 |         |  InventoryDataTable |  Id         |  IdAlias         |  {MapSlots}      |  Id              |  Type              |  ItemRefId       |
|  3 |         |  InventoryData      |  1000001    |  inven_001       |  SlotData        |  8000001         |  One               |  9000001         |
|  4 |         |                     |             |                  |  SlotData        |  8000002         |  Two               |  9000002         |
|  5 |         |                     |             |                  |  SlotData        |  8000003         |  Three             |  9000003         |
|  6 |         |  InventoryData      |  1000002    |  inven_002       |  SlotData        |  8000011         |  One               |  9000011         |
|  7 |         |                     |             |                  |  SlotData        |  8000012         |  Two               |  9000012         |
|  8 |         |                     |             |                  |  SlotData        |  8000013         |  Three             |  9000013         |

* [NesteData Samples](../src/Tests/DataSet/Depth/)

* NestedData field types can be either single or enumerable.
    - [Single and Enumerable NestedData Example](../src/Tests/DataSet/Depth/DepthDataTable.xlsx)
* When using Map\<Key, NestedData\> types, the Key type for Id fields can be Int32, Int64, or Enum; for Alias fields, String type is allowed.
* To indicate the start of NestedData, enter {FieldName}. The following field names must match those defined in the NestedData type.
* Below {FieldName}, you must enter the type name of the collection element for that field.
    - Note: All basic type field names of the main Data must be to the left of {FieldName}.
* Only one enumerable NestedData type can be entered at the same depth (List\<NestedData\>, Map\<key, NestedData\> types).
* NestedData can be structured in multiple levels, but for readability, management, and space efficiency, it is recommended to limit to 1–2 levels.
    - Instead, consider using RefId references.
    - For multi-level structures, field names can be entered as full names like {Depth1Map/Depth2Map/Depth3Map} or as abbreviated forms like {//Depth3Map}.
* If a data exclusion option is entered in cell A3, all NestedData owned by that record will also be excluded (all data entered in columns 3–5).

### Composing with Json Files

When using an in-house editing tool, you can save data in Json format for integration into the TableSet. Since you do not need to maintain the strict structure of sheet documents, you have greater flexibility in data composition.
* The rule restricting only one enumerable NestedData type per single depth does not apply.
* However, you must provide either an Id or Alias.

* [Json Samples](../src/Samples/DataSet/Sample1/BoxDataTable.json)
* [Json Serialize](../src/Tests/xpTURN.Converter.Tests/ConverterTests.Json.cs)

## Etc

* You can distribute Data records of the same Table type across multiple files.
    - However, record values must not be duplicated (especially Id and Alias values).
    - For Json files, you may save a single Data record per Table file, etc.

* The following folders are ignored by the Converter tool:
    - '[Define]': Table definition sheet file storage
    - '[Result]': Processed data file storage

* The following files are ignored by the Converter and ProtoGen tools:
    - Files starting with '$': Temporary files created by Excel applications
    - Files starting with '#': Files set to be ignored (commented files)
    - 'Subset.json': Reserved filename for Subset definitions