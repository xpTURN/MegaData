## 데이터 입력

### 스프레드 시트로 데이터 구성

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

시트 프로그램에서 새 파일을 만든 후, Table 시트를 생성하고 데이터를 입력하면 됩니다.

* 첫 번째 열은 시트에서 주석 입력 용도로 예약되어 있습니다. (필드 설명 혹은 사람이 읽기 쉬운 필드명)

* 첫 번째 행에는 데이터 제외 옵션을 입력할 수 있는 특수 필드가 있습니다.

    - Add_yyyyMMdd 형식으로 입력 :
        + TargetDate 기준으로 데이터 활성화 (TagetDate와 같거나 과거인 경우 데이터 활성화)

    - Del_yyyyMMdd 형식으로 입력 :    
        + TargetDate 기준으로 데이터 비활성화 (TagetDate와 같거나 과거인 경우 데이터 비활성화)

    - del 입력 :
        + 해당 레코드 항상 비활성화

    - 해당 필드 값이 비어 있는 경우 해당 레코드는 항상 활성화

    - TargetDate는 xpTURN.Converter 커멘드라인 인자로 지정 가능

    - 메인 Data에만 설정 가능 (NestedData 레벨에 설정 불가)

* B2 셀에 Table 타입명을 입력하여야 합니다.

* B3:B% 셀에는 Table 타입의 필드명을 입력합니다. (순서는 중요하지 않음, 단 NestedData 입력 규약은 별도)

    - #로 시작하는 필드의 경우 해당 내용이 무시됩니다. 데이터 레코드 구성 시 코멘트를 입력하거나 중간 데이터로 활용 가능합니다.

* B3:%3 셀에는 Table 타입이 소유하는 Data 레코드 타입을 입력합니다.

* C3 셀 부터는 해당 필드 변수에 맞는 값을 입력하면 됩니다.

* 필드 값 관련 특수 사양
    - Id 또는 Alias 필드는 반드시 제공해야 하며, 데이터 레코드의 접근 키로 사용됩니다.
    - 시트에서 Id 필드를 생략하고 IdAlias 필드만 있을 경우, Id 값은 Crc32 알고리즘을 통해 자동으로 발급됩니다. (불가피한 경우에만 제한적으로 사용하세요)
        + IdAlias 값은 수집 후 초기화되어 값이 지워집니다.
    - RefId, RefIdAlias 필드가 정의되어 있고 RefId를 시트에서 생략하고 RefIdAlias만 필드가 있는 경우 (RefId 값은 자동으로 찾아서 입력됩니다)
        + RefIdAlias 값은 RefId 값을 찾는 용도로 사용 후 초기화되어 값이 지워집니다.
    - Id 필드 값은 같은 Table에서 유니크하여야 합니다.
    - IdAlias 필드 값은 전역 TableSet에서 유니크하여야 합니다.

* List 기본 타입 유형 입력 방식 (List\<Int32\>> 혹은 List\<String\> 등) : 
    - List 필드명을 여러번 입력하면 다수의 값을 Add 할 수 있습니다.
    - 필드 값이 비어 있는 경우에는 추가되지 않습니다.

|    |  A      |  B            |  C          |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | ------------- | ----------- | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |               |             |                  |                  |                  |                    |                  |
|  2 |         |  BoxDataTable |  Id         |  IdAlias         |  ListItemID      |  ListItemID      |  ListItemID        |  ListItemID      |
|  3 |         |  BoxData      |  1000001    |  box_001         |  9000001         |  9000002         |  9000003           |  9000004         |
|  4 |         |  BoxData      |  1000002    |  box_002         |  9000011         |  9000012         |  9000013           |                  |
|  5 |         |  BoxData      |  1000003    |  box_003         |  9000021         |  9000022         |                    |                  |


* Map 기본 타입 입력 방식 (Map\<String, String\> 또는 Map\<Int32,String\> 등) :
    - 필드명에 MapFieldName<key> 와 같이 입력하고 필드 값이 입력되어 있으면 Add(key,value)로 데이터가 추가됩니다.

|    |  A      |  B                   |  C                 |  D               |  E               |  F               |  G                 |  H               |
| -- | ------- | -------------------- | ------------------ | ---------------- | ---------------- | ---------------- | ------------------ | ---------------- |
|  1 |         |                      |                    |                  |                  |                  |                    |                  |
|  2 |         |  TranslatedDataTable |  IdAlias           |  Map\<en-US\>    |  Map\<ko-KR\>    |  Map\<ja-JP\>    |  Map\<zh-CN\>      |  Map\<zh-TW\>    |
|  3 |         |  TranslatedData      |  ids_pawn_name_01  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  4 |         |  TranslatedData      |  ids_pawn_name_02  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |
|  5 |         |  TranslatedData      |  ids_pawn_name_03  |  Abcde           |  가나다            |  カタカナ          |  简体字             |  簡體字            |

#### NestedData 유형 입력

##### 데이터 시트 구성 예시
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

* NestedData 필드 유형은 단일, 열거형 모두 가능합니다.
    - [단일, 열거형 NestedData 예시](../src/Tests/DataSet/Depth/DepthDataTable.xlsx)
* Map\<Key,NestedData\> 유형을 사용하는 경우 Key 타입은 Id 필드의 경우 Int32류, Int64류, Enum이 사용 가능하고 Alias 필드의 경우 String 유형이 사용 가능합니다.
* {FieldName} 같이 입력 시 NestedData 시작을 알립니다. 그 뒤 필드명은 NestedData 타입에 정의된 필드명을 입력하여야 합니다.
* {FieldName} 하단에는 해당 필드의 컬렉선 구성 요소 타입명을 입력하여야 합니다.
    - 주의, 메인 Data의 기본 타입 필드명은 {FieldName}의 좌측에 모두 있어야 합니다.
* 동일 뎁스에는 열거형 NestedData 유형은 한 종류만 입력 가능하게 제한됩니다. (List\<NestedData\>, Map\<key,NestedData\> 유형)
* NestedData는 여러 단계로 구성할 수 있지만, 시트 문서의 가독성, 관리, 공간 효율 등을 고려하여 1~2단계로 제한하는 것을 권장합니다.
    - 대신, RefId 참조 구성 활용을 추천합니다.
    - 다단계로 구성할 경우, 필드명은 {Depth1Map/Depth2Map/Depth3Map}과 같은 전체 이름이나 {//Depth3Map}과 같은 축약형으로 입력할 수 있습니다.

* 데이터 제외 옵션을 A3 셀에 입력하는 경우 해당 레코드가 소유하는 NestedData까지 모두 제외됩니다. (3 ~ 5 열에 입력된 데이터 전부)

### Json 파일로 구성

인하우스 편집 툴로 데이터를 구성하는 경우 Json 포멧으로 데이터를 저장하여 TableSet에 통합 할 수 있도록 지원합니다. 시트 문서의 정형성을 지킬 필요가 없기 때문에 Data 구성의 자유도가 높습니다. 
* 단일 depth에 열거형 NestedData 1종만 사용 가능한 규칙 적용되지 않음.
* 단, Id 혹은 Alias 제공 필요.

* [Json Samples](../src/Samples/DataSet/Sample1/BoxDataTable.json)
* [Json Serialize](../examples/SampleProj/Assets/Scripts/SaveData.cs)

## 참고

* 같은 Table 유형의 Data 레코드를 여러 파일에 분산 저장 할 수도 있습니다.
    - 다만, 레코드 값이 중복되어서는 안 됩니다. (특히 Id 와 Alias 값들)
    - Json 파일로 저장하는 경우 Table 파일 하나에 단일 Data 만 저장 하는 경우 등.

* 다음 폴더는 Converter 툴에서 무시됩니다.
    - '[Define]' : 테이블 정의 시트 파일 저장소
    - '[Result]' : 가공된 데이터 파일 저장소

* 다음 파일들은 Converter, ProtoGen 툴에서 무시됩니다.
    - 파일명이 '$' 로 시작 : Excel 어플리케이션의 임시 파일
    - 파일명이 '\#' 로 시작 : 무시 설정된 파일 (주석화된 파일)
    - 'Subset.json' : Subset 정의 용도로 예약된 파일명