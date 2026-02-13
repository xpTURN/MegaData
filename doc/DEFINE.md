## Data Definition

Define your TableSet, Tables, Data types, and Enums in an Excel **Define** sheet. You can put multiple types in one file or split them across files. There is no import: every Message/Enum type must be defined in your Define sheets.

* **TableSet**: The root container; it holds all Tables for one logical dataset.
* **Table**: A container for one Data type (e.g. `PersonDataTable` holds `PersonData`).
* **Data**: One record shape (e.g. one row, or a row plus nested rows). Defines fields and nested types.
* **NestedData**: A message type used as a field of Data (single or in a List/Map).

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

#### Table and Data Type Rows

* **Column A** is ignored (use for comments or leave blank).
* A row with **Type / Name / Obsolete / FType / ExtraOptions / Desc** starts a new type.
    - **Type**: `Table` or `Data`.
    - **Name**: Type name (e.g. `PersonDataTable`, `PersonData`).
    - **Obsolete**: Deprecation level:
        - **Warning**: C# `[Obsolete]` (compiler warning).
        - **Error**: C# `[Obsolete(error: true)]` (compile error).
        - **Delete**: No code generated; type/field must not be referenced.
    - **ExtraOptions** (JSON):
        - **Key**: Field used as the key for lookups (`"Id"` or alias field name; default `"Id"`).
        - **Hide**: If true, no Getter is generated for this Table/Data (default `false`).
        - **OnDemand**: Lazy-load Data when accessed.
        - **WeakRef**: Hold lazily loaded Data as WeakReference so it can be collected when unused.
        - Example: `{"Key":"PawnId", "OnDemand": true}` or `{"OnDemand": true, "WeakRef": true}`.
    - **Desc**: Description; emitted as a C# comment.

* A row with **Num / Name / Obsolete / FType / ExtraOptions** (and optional Desc) starts field definitions for that type.

    - **Num**: Field number (must follow Protocol Buffers numbering rules).
    - **Name**: Field name.
    - **Obsolete**: Same as above (Warning/Error/Delete).
    - **FType**: Field type: a basic proto type, a user-defined Message/Enum, or a collection (List/Map).

        - **Basic types**: Bool, Int32, SInt32, SFixed32, UInt32, Fixed32, Int64, SInt64, SFixed64, UInt64, Fixed64, Float, Double, String, Bytes (Base64 in sheet).
        - **Custom types**: DateTime, TimeSpan, Guid, Uri (stored as UInt64/Int64/String internally).
        - **Message/Enum**: Types defined in your Define sheet.
        - **Collections**: `List<ElementType>`, `Map<KeyType,ValueType>` (e.g. `List<String>`, `Map<Int32,String>`).

    - **ExtraOptions** (for RefId fields): **Get** generates a getter that resolves the reference.
        - `{"Get": "String"}` → `public String Name => Instance.GetString(NameRefId);`
        - `{"Get": "BoxData"}` → `public BoxData Box => Instance.GetBoxData(BoxRefId);`
    - **Desc**: Description; emitted as a C# comment.

* **Conventions**
    - **Table**: Name should end with `Table` (e.g. `BoxDataTable`). It must have one field of type `Map<Key,Data>` (e.g. `Map<SFixed32,PersonData>`).
    - **Data**: Name should end with `Data` (e.g. `BoxData`). Must have an **Id** field (Int32/SInt32/SFixed32 or Enum). **IdAlias** (String) is optional and allows lookup by alias. **RefId** / **RefIdAlias** reference other Data by Id or Alias. You can have one collection of NestedData (List or Map) per Table; multiple single (non-collection) NestedData types are allowed.

## Other Notes

* **Field numbers (Num)**:
    - Allowed range: 1 to 536,870,911 (Protocol Buffers rules).
    - Reserved: 19,000–19,999 (Google), 18,000–18,999 (xpTURN.MegaData).
    - 1–15 use 1 byte when encoded; 16–2047 use 2 bytes. Prefer 1–15 or at least &lt; 2047.
    - Do not reuse a number for a different purpose; see [Updating A Message Type](https://protobuf.dev/programming-guides/proto3/#updating).

* Rows or columns whose name starts with **#** are ignored (comments or helper data).

* **Enum** definitions use only the Type, Name, Num, and Obsolete columns.
