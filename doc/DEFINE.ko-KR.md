## 데이터 정의

### 간단한 예시
시트 프로그램에서 새 파일을 만든 뒤, Define 시트를 만든 후 아래와 같이 타입을 정의하면 됩니다. 여러 타입을 한 파일에 정의할 수도 있고, 여러 파일에 나누어 정의할 수도 있습니다. import 기능이 없기 때문에 다른 곳에서 정의된 Message/Enum 타입을 사용할 수 없으며 내부에서 모두 정의해서 사용해야 합니다.

* TableSet : 여러 시트에 정의된 모든 Table 묶음을 의미
* Table : 하나의 시트에 데이터를 구성하는 단위
* Data : Table이 포함하는 데이터 레코드의 단위 (예: Excel의 한 행에 해당하는 데이터. 단, NestedData가 있는 경우 여러 열에 걸쳐 구성될 수 있음)
* NestedData : Data가 포함하는 하위 데이터 레코드

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

#### Message 타입 정의 (Table, Data 유형 정의)

* 1열은 항상 무시됩니다. 코멘트 입력 등을 하시거나 비워두면 됩니다.
* Type/Name/Obsolete/FType/ExtraOptions/Desc 해당 열을 추가하여 새로운 타입의 시작을 나타냅니다.
    - Type 열 : Table 혹은 Data를 입력
    - Name 열 : 타입명 입력
    - Obsolete 열 : 해당 타입 또는 필드의 사용 중지 여부를 지정

        + Warning : 타입 혹은 필드 참조 코드에 경고 출력 (c# \[Obsolete\] 어트리뷰트)

        + Error : 타입 혹은 필드 참조 코드에 컴파일 오류를 발생 (c# \[Obsolete(error:true)\] 어트리뷰트)

        + Delete : 타입 혹은 필드의 코드를 생성하지 않음 (TableSet 내에서 관련 참조가 존재하지 않아야 합니다)

    - ExtraOptions 열 : 부가 설정 (Json 포멧)

        + Key : Data 핸들링 Key 필드명 (Id 필드 혹은 Alias 필드 지정, 기본값 "Id")

        + Hide : TableSet class 생성 시 Table 혹은 Data의 Getter 함수를 생성하지 않음 (기본값 "false")

        + OnDemand : 해당 Table에서 관리하는 Data 레코드를 접근할 때 지연 로딩

        + WeakRef : 지연 로드된 Data를 WeakReference로 관리함, 참조가 제거되면 GC에 수집됨

            + 예시
                ```json
                {"Key":"PawnId", "OnDemand": true}
                ```
                ```json
                {"OnDemand": true, "WeakRef": true}
                ```

    - Desc 열 : 테이블에 관한 설명을 입력하면, C# 주석 생성됨

* Num/Name/Obsolete/FType/ExtraOptions 열을 추가하여 필드 정의의 시작을 나타냅니다.

    - Num 열 : 필드 번호를 입력 (proto의 Number 규약에 맞춰 입력 및 관리가 필요합니다)

    - Name 열 : 필드명을 입력

    - Obsolete 열 : 무효화 속성 설정 (Warning/Error/Delete)

    - FType 열 : proto의 기본 변수 유형을 입력하거나 사용자 정의 Message/Enum 타입을 입력, 또한, 컬렉션 타입 중 List, Map 타입을 사용할 수도 있음

        + 기본 타입 : 
            + Bool
            + Int32, SInt32, SFixed32
            + UInt32, Fixed32
            + Int64, SInt64, SFixed64
            + UInt64, Fixed64
            + Float
            + Double
            + String
            + Bytes (시트에서는 Base64 텍스트로 입력)

        + 커스텀 타입 :
            + DateTime (내부적으로 UInt64 타입으로 직렬화 : Ticks | Kind << 62)
            + TimeSpan (내부적으로 Int64 타입으로 직렬화)
            + Guid (내부적으로 String 타입으로 직렬화)
            + Uri (내부적으로 String 타입으로 직렬화)
        
        + Message / Enum 타입 : 사용자가 테이블 정의서로 작성하여 제공

        + Collections 타입 :
            + List
            + Map

                > List\<String\>

                > Map\<Int32,String\>

    - ExtraOptions 열 : 부가 설정 (Json 포멧)

        + Get : Getter 함수 생성 (RefId에 지정, 해당 키값이 가리키는 Data를 리턴하는 함수 생성)

            NameRefId 필드의 ExtraOptions 항목을 아래와 같이 설정하면
            ```json
            {"Get": "String"}
            ```
            다음 코드가 자동 생성됩니다.
            ```cs
            public String Name => Instance.GetString(NameRefId);
            ```
            BoxRefId 필드의 ExtraOptions 항목을 아래와 같이 설정하면
            ```json
            {"Get": "BoxData"}
            ```
            다음 코드가 자동 생성됩니다.
            ```cs
            public BoxData Box => Instance.GetBoxData(BoxRefId);
            ```

    - Desc 열 : 테이블에 관한 설명을 입력해두면, c# 주석 생성됨

* Table을 사용할 때 필수 규약

    - Table 정의

        + 이름은 Table로 끝나는 것을 권장 (ex. BoxDataTable)

        + Data 필드는 Map 컬랙션 타입으로 구성하여야 합니다. (Map\<Int32,BoxData\> 혹은 Map\<String,BoxData\>)

    - Data 정의

        + 타입 이름은 Data로 끝나는 것을 권장 (ex. BoxData)

        + Id 필드를 필수로 제공하여야 함 (하위 NestedData는 필수 아님)
            + Id는 Int32(Int32, SInt32, SFixed32) 또는 Enum 타입만 사용할 수 있음

        + IdAlias 필드를 제공하는 경우 String으로 된 별명을 Data 레코드에 지정하고 해당 값을 Key 값으로 사용할 수 있음 (선택적 정의)

        + RefId로 끝나는 필드를 제공하는 경우 다른 곳에 정의된 Data 타입 레코드 인덱스 Id 값으로 데이터 레코드 접근 가능

        + RefIdAlias 필드를 제공하는 경우 Alias Key 값으로 데이터 접근 가능 (RefId와 함께 사용 필요)

        + List\<NestedData\> 혹은 Map\<Int32,NestedData\> 로 하위 Nested Data 구성 가능 (소유 개수 제한이 없으나, 시트에서 사용은 1개만 가능)

        + 단일 NestedData 유형은 소유 개수에 제한이 없음

## 참고

* Num 값 입력에 관한 안내
    - proto 규약에 따라 1 ~ 536,870,911 사이의 값을 입력 할 수 있습니다.

    - 단, 19,000 ~ 19,999 사이의 값은 Google에 의해서 예약되어 있습니다. 18,000 ~ 18,999 사이의 값은 xpTURN.MegaData에 의해서 예약되어 있습니다.

    - 1 ~ 15 사이의 값을 사용 시 1 bytes로 시리얼라이즈, 6 ~ 2047 사이의 값을 사용 시 2 bytes로 시리얼라이즈됩니다. (1 ~ 15까지 사용 최적, 2047 값 이하로 사용 권장)

    - 한번 사용한 번호를 다른 용도로 재사용하는 경우 주의하여야 합니다.

        > 주의사항 : Google에서 제공하는 [Updating A Message Type](https://protobuf.dev/programming-guides/proto3/#updating) 문서 참고

* #로 시작하는 필드의 경우 해당 내용이 무시됩니다. 데이터 레코드 구성 시 코멘트를 입력하거나 중간 데이터로 활용할 수 있습니다.

* Enum 타입 정의는 Type, Name, Num, Obsolete 열만 사용 합니다.
