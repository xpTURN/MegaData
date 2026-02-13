## 데이터 정의

Excel **Define** 시트에서 TableSet, Table, Data 타입, Enum을 정의합니다. 한 파일에 여러 타입을 두거나 여러 파일로 나눌 수 있습니다. import는 없으므로 사용하는 모든 Message/Enum 타입을 Define 시트에서 정의해야 합니다.

* **TableSet**: 한 데이터셋의 루트 컨테이너. 모든 Table을 묶습니다.
* **Table**: 한 Data 타입을 담는 컨테이너(예: `PersonDataTable`이 `PersonData`를 가짐).
* **Data**: 한 레코드 형태(한 행, 또는 행+중첩 행). 필드와 중첩 타입을 정의합니다.
* **NestedData**: Data의 필드로 쓰이는 메시지 타입(단일 또는 List/Map).

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

#### Table·Data 타입 행

* **A열**은 무시됩니다. 주석이나 비워 두세요.
* **Type / Name / Obsolete / FType / ExtraOptions / Desc** 행이 새 타입의 시작을 나타냅니다.
    - **Type**: `Table` 또는 `Data`.
    - **Name**: 타입명(예: `PersonDataTable`, `PersonData`).
    - **Obsolete**: 사용 중단 수준
        - **Warning**: C# `[Obsolete]` (경고).
        - **Error**: C# `[Obsolete(error: true)]` (컴파일 오류).
        - **Delete**: 코드 미생성. 타입/필드 참조가 없어야 함.
    - **ExtraOptions**(JSON):
        - **Key**: 조회에 쓰는 키 필드(`"Id"` 또는 별칭 필드명, 기본 `"Id"`).
        - **Hide**: true면 이 Table/Data용 Getter 미생성(기본 `false`).
        - **OnDemand**: Data 접근 시 지연 로딩.
        - **WeakRef**: 지연 로드된 Data를 WeakReference로 보관해 미참조 시 GC 대상으로 둠.
        - 예: `{"Key":"PawnId", "OnDemand": true}` 또는 `{"OnDemand": true, "WeakRef": true}`.
    - **Desc**: 설명. C# 주석으로 출력됩니다.

* **Num / Name / Obsolete / FType / ExtraOptions**(및 선택적 Desc) 행이 해당 타입의 필드 정의 시작을 나타냅니다.

    - **Num**: 필드 번호(Protocol Buffers 번호 규칙 준수).
    - **Name**: 필드명.
    - **Obsolete**: 위와 동일(Warning/Error/Delete).
    - **FType**: 필드 타입. 기본 proto 타입, 사용자 정의 Message/Enum, 또는 컬렉션(List/Map).

        - **기본 타입**: Bool, Int32, SInt32, SFixed32, UInt32, Fixed32, Int64, SInt64, SFixed64, UInt64, Fixed64, Float, Double, String, Bytes(시트에서는 Base64).
        - **커스텀 타입**: DateTime, TimeSpan, Guid, Uri(내부적으로 UInt64/Int64/String 등으로 저장).
        - **Message/Enum**: Define 시트에서 정의한 타입.
        - **컬렉션**: `List<요소타입>`, `Map<키타입,값타입>`(예: `List<String>`, `Map<Int32,String>`).

    - **ExtraOptions**(RefId 필드용): **Get**을 쓰면 참조를 풀어 주는 Getter가 생성됩니다.
        - `{"Get": "String"}` → `public String Name => Instance.GetString(NameRefId);`
        - `{"Get": "BoxData"}` → `public BoxData Box => Instance.GetBoxData(BoxRefId);`
    - **Desc**: 설명. C# 주석으로 출력됩니다.

* **규약**
    - **Table**: 이름은 `Table`로 끝나는 것을 권장(예: `BoxDataTable`). 필드 하나는 반드시 `Map<키,Data>` 타입(예: `Map<SFixed32,PersonData>`)이어야 합니다.
    - **Data**: 이름은 `Data`로 끝나는 것을 권장(예: `BoxData`). **Id** 필드(Int32/SInt32/SFixed32 또는 Enum) 필수. **IdAlias**(String)는 선택이며 별칭으로 조회 가능. **RefId**/**RefIdAlias**로 다른 Data를 Id/별칭으로 참조. Table당 NestedData 컬렉션(List 또는 Map)은 하나만 둘 수 있고, 단일(비컬렉션) NestedData 타입은 여러 개 가능합니다.

## 기타

* **필드 번호(Num)**:
    - 허용 범위: 1 ~ 536,870,911(Protocol Buffers 규칙).
    - 예약: 19,000~19,999(Google), 18,000~18,999(xpTURN.MegaData).
    - 1~15는 인코딩 시 1바이트, 16~2047은 2바이트 사용. 1~15 또는 최소 2047 미만 사용 권장.
    - 한 번 쓴 번호를 다른 용도로 재사용하지 마세요. [Updating A Message Type](https://protobuf.dev/programming-guides/proto3/#updating) 참고.

* 이름이 **#**로 시작하는 행/열은 무시됩니다(주석·보조 데이터).

* **Enum** 정의에는 Type, Name, Num, Obsolete 열만 사용합니다.
