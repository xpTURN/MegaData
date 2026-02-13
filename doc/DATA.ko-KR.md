## 데이터 입력

Excel 시트와 JSON 파일에 데이터를 넣는 방법을 설명합니다.

### 시트에 데이터 입력하기

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

워크북에 **Table** 시트를 만들고 아래처럼 데이터를 입력합니다.

* **A열**은 주석용으로 예약되어 있습니다. 데이터로 사용하지 않습니다.

* **데이터 제외 옵션**은 전용 열(예: A열)에 넣을 수 있습니다. 한 열만 적용되며, 해당 행 전체(메인 Data만, NestedData 레벨에는 불가)에 적용됩니다.

    - **Add_yyyyMMdd**: 컨버터의 TargetDate가 이 날짜와 같거나 그 이후일 때만 해당 행을 포함합니다.
    - **Del_yyyyMMdd**: 컨버터의 TargetDate가 이 날짜와 같거나 그 이후일 때 해당 행을 제외합니다.
    - **del**(또는 **hide**): 해당 행은 항상 제외합니다.
    - 비어 있음: 해당 행은 항상 포함합니다.
    - TargetDate는 xpTURN.Converter의 커맨드라인 인자로 전달합니다.

* **B2 셀**: Table 타입명(예: `PersonDataTable`)을 입력합니다.

* **3행**: B열에는 Data 타입명(예: `PersonData`)을, C열부터는 필드명(Id, IdAlias, Name 등)을 입력합니다. 순서는 NestedData 규칙을 제외하면 상관없습니다.
    - `#`로 시작하는 이름은 무시됩니다(주석·보조 열).

* **4행 이하**: B열에는 각 행의 Data 타입명을 반복하고, C열부터는 각 필드 값을 입력합니다.

* **키 필드**
    - **Id** 또는 **IdAlias**(또는 둘 다)를 지정합니다. 접근 키로 사용됩니다.
    - IdAlias만 넣으면 Id는 자동 생성됩니다(예: Crc32). 가급적 제한적으로 사용하고, 변환 후 IdAlias 값은 유지되지 않습니다.
    - **RefId**와 **RefIdAlias**가 있고 RefIdAlias만 채우면 RefId는 별칭으로 해석된 뒤, 변환 후 RefIdAlias는 지워집니다.
    - **Id**는 같은 Table 내에서, **IdAlias**는 전체 TableSet에서 유일해야 합니다.

* **List 필드**(예: `List<Int32>`, `List<String>`): 같은 열 머리글을 여러 번 두고, 비어 있지 않은 셀마다 값이 리스트에 하나씩 추가됩니다.

|    |  A      |  B            |  C          |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | ------------- | ----------- | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |               |             |                  |                  |                  |                    |                  |
|  2 |         |  BoxDataTable |  Id         |  IdAlias         |  ListItemID      |  ListItemID      |  ListItemID        |  ListItemID      |
|  3 |         |  BoxData      |  1000001    |  box_001         |  9000001         |  9000002         |  9000003           |  9000004         |
|  4 |         |  BoxData      |  1000002    |  box_002         |  9000011         |  9000012         |  9000013           |                  |
|  5 |         |  BoxData      |  1000003    |  box_003         |  9000021         |  9000022         |                    |                  |


* **Map 필드**(예: `Map<String,String>`, `Map<Int32,String>`): 열 머리글을 `MapFieldName<key>` 형태로 쓰고, 셀 값이 해당 키의 값으로 저장됩니다.

|    |  A      |  B                   |  C                 |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | -------------------- | ------------------ | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |                      |                    |                  |                  |                  |                    |                  |
|  2 |         |  TranslatedDataTable |  IdAlias           |  Map\<en-US\>    |  Map\<ko-KR\>    |  Map\<ja-JP\>    |  Map\<zh-CN\>      |  Map\<zh-TW\>    |
|  3 |         |  TranslatedData      |  ids_pawn_name_01  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  4 |         |  TranslatedData      |  ids_pawn_name_02  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  5 |         |  TranslatedData      |  ids_pawn_name_03  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |

#### NestedData 입력

##### 예시
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

* [NestedData 샘플](../src/Tests/DataSet/Depth/)

* NestedData는 단일 메시지이거나 컬렉션(List/Map)일 수 있습니다.
    - [단일·컬렉션 NestedData 예시](../src/Tests/DataSet/Depth/DepthDataTable.xlsx)
* `Map<Key,NestedData>`에서 Key는 Id 계열이면 Int32/Int64/Enum, Alias 계열이면 String을 사용할 수 있습니다.
* **{FieldName}**으로 NestedData 블록을 시작합니다. 이후 열은 해당 NestedData 타입에 정의된 필드명을 사용하고, {FieldName} 아래 행에는 요소 타입명을 반복해서 넣습니다.
    - 메인 Data의 단순(비중첩) 필드는 모두 {FieldName} 왼쪽에 있어야 합니다.
* 같은 depth에는 컬렉션 NestedData(List 또는 Map) 필드는 하나만 둘 수 있습니다.
* 중첩은 1~2단계로 두는 것을 권장합니다. 더 깊게 쓰려면 RefId를 고려하세요. 전체 경로는 `{Depth1Map/Depth2Map/Depth3Map}`처럼, 축약은 `{//Depth3Map}`처럼 쓸 수 있습니다.
* 데이터 제외 옵션으로 행이 제외되면, 해당 행의 NestedData 전체도 제외됩니다.

### JSON 파일 사용

Excel 대신 JSON(예: 인하우스 툴 출력)으로 데이터를 넣을 수 있습니다. 시트의 열 구조를 따를 필요가 없어 구성이 자유롭습니다.
* "depth당 컬렉션 NestedData 하나" 제한은 적용되지 않습니다.
* 각 레코드에는 Id 또는 IdAlias 중 하나는 반드시 있어야 합니다.

* [Json Samples](../src/Samples/DataSet/Sample1/BoxDataTable.json)
* [Json Serialize](../examples/SampleProj/Assets/Scripts/SaveData.cs)

## 기타

* **여러 파일**: 같은 Table의 데이터를 여러 파일로 나눌 수 있습니다. Id와 IdAlias는 전체에서 유일해야 합니다. JSON은 파일당 레코드 하나만 두어도 됩니다.

* **Converter가 무시하는 폴더**: `[Define]`(정의 시트), `[Result]`(컨버터 출력).

* **Converter·ProtoGen이 무시하는 파일**: 이름이 `$`로 시작하는 파일(Excel 임시), `#`로 시작하는 파일(주석 처리), 예약 이름 `Subset.json`(서브셋 설정용).