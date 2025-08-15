## xpTURN.MegaData 라이브러리
xpTURN.MegaData 라이브러리는 .NET 기반 프로젝트에서 다양한 데이터 구조를 간편하게 정의·관리하고, 대량의 데이터를 편리하게 활용할 수 있도록 제공합니다. 데이터 구조의 정의와 관리는 기본적으로 Excel을 통해 이루어집니다.

데이터 시리얼라이즈를 위해서 Google에서 제공하는 "Protocol Buffers"를 사용합니다. 단, 기존 'Protocol Buffers' 라이브러리를 그대로 사용하지 않고 xpTURN에서 커스터마이즈하여 제공합니다. 자세한 내용은 문서 하단을 참고하시기 바랍니다.

또한 .proto 파일 파싱 및 코드 생성을 위해서 protobuf-net 라이브러리가 활용됩니다. 단, protobuf-net.Reflection 라이브러리를 일부 변형하여 제공됩니다.

## Supported Runtimes
- .NET 8.0+
- .NET Standard 2.1 (for Unity3D IL2CPP Scripting backend)
- (C# Language Version 9.0)

## 기본 사용법
프로젝트 통합 방법은 [USAGE](./doc/USAGE.ko-KR.md) 문서를 참고하시면 됩니다.

### 데이터 구조 정의
"Protocol Buffers"의 메시지 정의는 .proto 스크립트 방식으로 제공되지만, xpTURN.MegaData에서는 시트 파일의 Define 시트에 작성하는 방식을 기본으로 합니다. 이를 통해 데이터 구조를 문서화하면서 동시에 데이터 정의 스크립트 역할을 겸합니다. 간단한 예시는 아래와 같습니다.

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

* 상세한 스펙은 다음 문서에서 확인 : [데이터 정의](./doc/DEFINE.ko-KR.md)

### 코드 생성

아래와 같이 전용 생성기를 통해서 시트 파일로 정의한 데이터 구조를 코드로 생성합니다. 생성된 소스코드는 사용자 라이브러리 프로젝트에 포함되어 xpTURN.Converter 툴에 제공되어야 합니다. (예시: [Sample1 프로젝트](./src/Samples/xpTURN.TableSet.Samples))

```sh
dotnet ./xpTURN.ProtoGen.dll --input="../../../Samples/DataSet/Sample1/[Define]" --output="../../../Samples/xpTURN.TableSet.Samples/Sample1" --output-type="cs;proto" --namespace="Samples" --tableset="Sample1TableSet" --for-datatable
```

출력 결과 : [예시](./src/Samples/xpTURN.TableSet.Samples/Sample1)

### 데이터 입력

|    |  A      |  B               |  C          |  D               |  E               |  F          |  G                 |  H               |
| -- | ------- | ---------------- | ----------- | ---------------- | ---------------- | ----------- | ------------------ | ---------------- |
|  1 |         |                  |             |                  |                  |             |                    |                  |
|  2 |         |  PersonDataTable |  Id         |  IdAlias         |  Name            |  Role       |  Email             |  Phone           |
|  3 |         |  PersonData      |  1000001    |  spot_one_001    |  Emily Parker    |  Staff      |  xxx111@zmall.com  |  (415) 555-0134  |
|  4 |         |  PersonData      |  1000002    |  spot_one_002    |  James Mitchell  |  Staff      |  xxx222@zmall.com  |  (415) 555-0135  |
|  5 |         |  PersonData      |  1000003    |  spot_one_003    |  Olivia Brooks   |  Staff      |  xxx333@zmall.com  |  (415) 555-0136  |
|  6 |         |  PersonData      |  1000004    |  spot_one_004    |  Michael Hayes   |  Manager    |  xxx333@zmall.com  |  (415) 555-0137  |
|  7 |         |  PersonData      |  1000005    |  spot_one_005    |  Sophia Bennett  |  Executive  |  xxx333@zmall.com  |  (415) 555-0138  |

* 상세한 스펙은 다음 문서에서 확인 : [데이터 입력](./doc/DATA.ko-KR.md)

### 데이터 가공
시트 파일에 입력된 데이터 구조나 값을 그대로 사용하는 것이 아니라, 가공이 필요한 경우에는 TableSetPostProcess를 상속받아 포스트 프로세서를 직접 구현할 수 있습니다.
예시는 [Locale.Type2](./src/Tests/xpTURN.TableSet.ForTests/Locale.Type2/LocaleTablePostProcess.cs)를 참고하시기 바랍니다.

### 데이터 바이너리화

데이터 컨버트 예시 : 
```sh
dotnet ./xpTURN.Converter.dll --input="../../../Samples/DataSet/Sample1" --output="../../../Samples/DataSet/Sample1/[Result]" --namespace="Samples" --tableset="Sample1TableSet"
```

### 런타임에서 활용
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

참고로, OnDemand 또는 WeakRef 옵션을 사용한 테이블의 경우 Table.Map 변수 접근이 제한되며, GetXXXData와 같은 함수를 통해 레코드 단위로만 접근할 수 있습니다.

## 참고

### 주의사항
xpTURN.MegaData의 시리얼라이즈는 "Protocol Buffers"를 활용하므로 관련 주의점을 숙지하셔야 합니다. 특히 데이터 정의를 수정할 경우 상위/하위 버전 간에 호환성 문제가 발생할 수 있습니다.

* 참고 문서: (Google에서 제공하는 [Updating A Message Type](https://protobuf.dev/programming-guides/proto3/#updating) 문서 참고)

### xpTURN.Protobuf 라이브러리
[Google.Protobuf CSharp](https://github.com/protocolbuffers/protobuf/tree/main/csharp/src/Google.Protobuf) 버전의 변형판으로 코드량을 최소화한 변형판입니다. 주요 특징은 Descriptor 관련 의존성이 제거되어 있고 FieldCodec 관련 수정사항과 커스텀 타입 지원 등을 위해서 일부 수정이 이루어졌습니다. 

### xpTURN.ProtoGen
시트 혹은 .proto에 정의된 메시지 정의를 바탕으로 c# 코드를 생성하는 툴입니다.

생성된 코드는 Google 'Protocol Buffers'의 protoc가 생성하는 결과물과 유사하지만, 코드 생성량을 최소화하고 데이터 관리에 필요한 코드가 추가되어 있습니다. 또한 .proto 문법에서 설정할 수 없는 커스텀 설정도 추가되어 있습니다.

#### protoc에서 생성한 코드 결과물과 차이점
* Descriptor, Json, UnknownFields 관련 코드 생성하지 않음, WellKnownTypes 미지원
* 프로퍼티 대신 필드 사용, Attribute, Const 변수, Parser 코드 생성 최소화
* xpFieldCodec, xpRepeatedCodec, xpMapCodec 사용
* RepeatedField<> 대신 List<> 사용, MapField<> 대신 Dictionary<> 사용
* xpTURN.MegaData를 위한 전용 코드 생성
* xpTURN 전용 커스텀 타입 지원: DateTime, TimeSpan, Uri, Guid (내부적으로 UInt64, Int64, String, String으로 처리)

#### .proto 생성
xpTURN.ProtoGen 툴은 시트 문서를 기반으로 .cs 및 .proto 파일을 생성하는 기능을 갖추고 있습니다.

#### .proto 정의를 바탕으로 c# 코드 생성
xpTURN.ProtoGen 툴에서는 .proto 파일로 .cs 소스코드 생성을 지원합니다. xpTURN.Protobuf의 간소화된 버전의 c# 코드를 네트워크 패킷 등에 활용하고자 하는 경우에 사용할 수 있습니다.

참고. .proto 파서는 protobuf-net.Reflection 라이브러리를 활용하고 있습니다. c# 코드 생성 시 커스터마이징을 위해서 일부 내용이 수정되어 제공됩니다.

### 최적화
* 런타임 영역에서는 Reflection 관련 코드 사용과 GC 발생을 최소화하도록 설계되어 있습니다.
* 다만, Save, TablePostProcess 영역은 디자인 모드 영역으로 설정되어 있으므로 최적화 대상이 아닙니다.

## 지원
문의 사항이 있으신 경우 [Github 이슈 등록](https://github.com/xpTURN/xpTURN/issues) 또는 [email](mailto:xpTURN@gmail.com)로 연락해주시기 바랍니다.
