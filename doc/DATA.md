## Data Input

This section describes how to enter data in Excel sheets and JSON files.

### Entering Data in Spreadsheets

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

Create a **Table** sheet in your workbook and enter data as follows.

* **Column A** is reserved for comments (e.g. field descriptions). It is not used as data.

* **Data exclusion options** can be set in a dedicated column (e.g. column A). Only one such column applies; options apply to the whole row (main Data only, not NestedData).

    - **Add_yyyyMMdd**: Include this row only when the converter’s TargetDate is on or after the given date.
    - **Del_yyyyMMdd**: Exclude this row when the converter’s TargetDate is on or after the given date.
    - **del** (or **hide**): Always exclude this row.
    - Empty: Always include the row.
    - TargetDate is passed as a command-line argument to xpTURN.Converter.

* **Cell B2**: Table type name (e.g. `PersonDataTable`).

* **Row 3**: In column B, enter the Data type name (e.g. `PersonData`). From column C onward, enter field names (Id, IdAlias, Name, etc.). Order does not matter except for NestedData (see below).
    - Names starting with `#` are ignored (comments or helper columns).

* **From row 4 downward**: In column B repeat the Data type name for each record. From column C onward, enter values for each field.

* **Key fields**
    - Provide either **Id** or **IdAlias** (or both). They are used as access keys.
    - If you provide only IdAlias, Id is auto-generated (e.g. via Crc32). Use sparingly; IdAlias is not stored as-is after conversion.
    - If **RefId** and **RefIdAlias** exist and you only fill RefIdAlias, RefId is resolved from the alias and RefIdAlias is cleared after conversion.
    - **Id** must be unique within the same Table. **IdAlias** must be unique across the entire TableSet.

* **List fields** (e.g. `List<Int32>`, `List<String>`): Repeat the same column header for each slot. Each non-empty cell adds one value to the list.

|    |  A      |  B            |  C          |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | ------------- | ----------- | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |               |             |                  |                  |                  |                    |                  |
|  2 |         |  BoxDataTable |  Id         |  IdAlias         |  ListItemID      |  ListItemID      |  ListItemID        |  ListItemID      |
|  3 |         |  BoxData      |  1000001    |  box_001         |  9000001         |  9000002         |  9000003           |  9000004         |
|  4 |         |  BoxData      |  1000002    |  box_002         |  9000011         |  9000012         |  9000013           |                  |
|  5 |         |  BoxData      |  1000003    |  box_003         |  9000021         |  9000022         |                    |                  |

* **Map fields** (e.g. `Map<String,String>`, `Map<Int32,String>`): Use column headers of the form `MapFieldName<key>`. Each cell value is stored as the entry for that key.

|    |  A      |  B                   |  C                 |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | -------------------- | ------------------ | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |                      |                    |                  |                  |                  |                    |                  |
|  2 |         |  TranslatedDataTable |  IdAlias           |  Map\<en-US\>    |  Map\<ko-KR\>    |  Map\<ja-JP\>    |  Map\<zh-CN\>      |  Map\<zh-TW\>    |
|  3 |         |  TranslatedData      |  ids_pawn_name_01  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  4 |         |  TranslatedData      |  ids_pawn_name_02  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  5 |         |  TranslatedData      |  ids_pawn_name_03  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |

#### Entering NestedData

##### Example
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

* [NestedData samples](../src/Tests/DataSet/Depth/)

* NestedData can be a single message or a collection (List/Map).
    - [Example: single and collection NestedData](../src/Tests/DataSet/Depth/DepthDataTable.xlsx)
* For `Map<Key, NestedData>`, Key may be Int32, Int64, or Enum for Id-style keys, or String for Alias.
* Start a NestedData block with **{FieldName}**. Subsequent columns must use field names defined for that NestedData type. Under {FieldName}, repeat the element type name for each row that belongs to that nested block.
    - All simple (non-nested) fields of the main Data must appear to the left of {FieldName}.
* At each depth, only one collection NestedData field (List or Map) is allowed.
* Prefer 1–2 levels of nesting; for deeper structures consider RefId instead. You can use full paths like `{Depth1Map/Depth2Map/Depth3Map}` or short forms like `{//Depth3Map}`.
* If a row is excluded by the data-exclusion option, all NestedData for that row is excluded as well.

### Using JSON Files

You can supply data as JSON (e.g. from an in-house tool) instead of Excel. JSON does not require the same column layout as sheets, so you have more flexibility.
* The “one collection NestedData per depth” rule does not apply.
* You must still provide either Id or IdAlias for each record.

* [Json Samples](../src/Samples/DataSet/Sample1/BoxDataTable.json)
* [Json Serialize](../src/Tests/xpTURN.Converter.Tests/ConverterTests.Json.cs)

## Other Notes

* **Multiple files**: You can split data for the same Table across several files. Id and IdAlias must be unique across all of them. JSON files may contain one record per file if desired.

* **Folders ignored by Converter**: `[Define]` (definition sheets), `[Result]` (converter output).

* **Files ignored by Converter and ProtoGen**: Names starting with `$` (Excel temp files), names starting with `#` (treated as comments), and the reserved name `Subset.json` (used for subset configuration).