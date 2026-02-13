## 프로젝트 통합

Unity 예제는 [SampleProj](../examples/SampleProj)를 참고하세요.

### 소스 가져오기
[저장소](https://github.com/xpTURN/MegaData)에서 클론 또는 다운로드 후 솔루션을 빌드합니다.

### 프로젝트 초기 세팅
TableSet 프로젝트를 만들거나, [xpTURN.TableSet.Samples](../src/Samples/xpTURN.TableSet.Samples)를 복사해 이름만 바꿔 사용할 수 있습니다.

### 데이터 구조 정의
Excel에서 사용할 TableSet·Table·Data 타입을 [데이터 정의](./DEFINE.ko-KR.md)에 따라 정의합니다. 

### TableSet 소스 생성
Define 시트를 기준으로 C# 및 .proto를 생성합니다. TableSet 프로젝트를 빌드한 뒤, 출력 .dll을 xpTURN.Converter(및 ProtoGen)와 같은 디렉터리에 두어 컨버터가 타입을 로드할 수 있게 합니다.

예시:
```sh
dotnet ./xpTURN.ProtoGen.dll --input="../../../Samples/DataSet/Sample1/[Define]" --output="../../../Samples/xpTURN.TableSet.Samples/Sample1" --output-type="cs;proto" --namespace="Samples" --tableset="Sample1TableSet" --for-datatable
```

### Unity 프로젝트 통합
프로젝트에 포함시켜야 하는 런타임 .dll 목록은 아래와 같습니다.
```sh
System.Runtime.CompilerServices.Unsafe.dll
xpTURN.Common.dll
xpTURN.MegaData.dll
xpTURN.Protobuf.dll
MyProduct.TableSet.dll
```
참고: MyProduct.TableSet.dll은 사용자가 만든 TableSet이 포함된 .dll의 예시입니다.
참고, System.Runtime.CompilerServices.Unsafe.dll은 .NET Standard 2.1 바이너리를 사용할 때 필요하며, .NET 8.0 이상에서는 필요하지 않습니다.

### 데이터 입력
정의한 Table에 맞게 Excel 또는 JSON으로 데이터를 입력·내보냅니다. [데이터 입력](DATA.ko-KR.md) 참고.

### 데이터 변환
컨버터를 실행해 런타임용 바이너리를 만듭니다.

예시: 
```sh
dotnet ./xpTURN.Converter.dll --input="../../../Samples/DataSet/Sample1" --output="../../../Samples/DataSet/Sample1/[Result]" --namespace="Samples" --tableset="Sample1TableSet"
```

### 데이터 로드 및 사용
바이너리 데이터를 코드에서 로드하는 예는 다음과 같습니다.
```cs
// Set the logger to use Unity's Debug class
xpTURN.Common.Logger.Log.SetLogger(new xpLogger());

// Load the Sample1TableSet data
Sample1TableSet.Instance.Load($"{Application.streamingAssetsPath}/Sample1TableSet.bytes");
Sample1TableSet.Instance.LoadAdditive($"{Application.streamingAssetsPath}/Sample1TableSet.Locale.bytes");

// Set the locale for the Sample1TableSet
var cultureInfo = new CultureInfo("en-US");
Sample1TableSet.Instance.SetLocale(cultureInfo.LCID);
```
서브셋 파일(예: `Sample1TableSet.Locale.bytes`)과 `SetLocale`은 프로젝트에 맞게 조정하세요. `SetLogger`도 필요 시 설정할 수 있습니다.

데이터 접근 예시는 아래와 같습니다.
```cs
// Get the box data for a specific box
var boxData = Sample1TableSet.Instance.GetBoxData("box_0004");
Debug.Log($"BoxData: {boxData.Name}");
```

### 지속적인 관리
프로젝트 진행 중 데이터 정의는 주기적으로 변경되며, 데이터 입력도 수시로 이루어집니다. 이러한 작업은 CI 툴을 통해 자동화하는 것이 좋습니다.

#### 사후 처리
로드 후 로직(예: 로케일 해석)을 실행하려면 **TableSetPostProcess**를 상속하세요. 예: [LocaleTablePostProcess](../src/Tests/xpTURN.TableSet.ForTests/Locale.Type2/LocaleTablePostProcess.cs).

검증이 필요하면 **TableSetCheckPostProcess**를 상속해 `CheckData`를 구현하세요.

#### 내부 툴에서 대량 데이터
자체 툴에서 데이터를 만들 경우 JSON으로 내보낸 뒤 컨버터로 로드할 수 있습니다. 예시:
```cs
var boxDataTable = new BoxDataTable();

var boxData = new BoxData();
boxData.Id = 2100001;
boxData.IdAlias = "box_1001";
boxData.NameRefIdAlias = "box_name_1001";

var boxSlot = new BoxSlot();
boxSlot.Slot = 1;
boxSlot.ItemRefIdAlias = "item_1001";
boxData.List.Add(boxSlot);

boxDataTable.GetMap().Add(boxData.Id, boxData);

JsonUtils.ToJsonFile(boxDataTable, $"{Application.dataPath}/../DataSet/BoxDataTable.json");
```
참고, [JsonUtils](../examples/SampleProj/Assets/Scripts/JsonUtils.cs) 코드는 링크를 참고하세요.

#### Subset 사용
일부 테이블만 별도 파일(예: 로케일 데이터)로 배포하려면 데이터 폴더 루트에 **Subset.json**을 두세요. 컨버터가 해당 테이블들을 별도 바이너리로 저장합니다. 서브셋은 여러 개 정의할 수 있고, 각 테이블은 최대 하나의 서브셋에만 포함될 수 있습니다.

```json
{
  "$type": "xpTURN.MegaData.SubsetDataTable, xpTURN.MegaData",
  "Map": {
    "Locale": {
      "Tables": [
        "LocaleDataTable",
        "TextDataTable",
        "TranslatedDataTable"
      ]
    }
  }
}
```
`"Locale"` 자리에는 서브셋 이름을, `"Tables"`에는 해당 서브셋 파일에 저장할 테이블 이름 목록을 넣습니다.