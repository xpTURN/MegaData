## 프로젝트 통합
유니티 프로젝트에서 MegaData 사용 예시는 [SampleProj](../examples/SampleProj)에서 참고하실 수 있습니다

### 소스 가져오기
[소스 저장소](https://github.com/xpTURN/MegaData)에서 프로젝트를 다운 받고 기본 빌드를 진행합니다.

### 프로젝트 초기 세팅
자신의 프로젝트에서 사용할 TableSet 프로젝트를 생성합니다. [xpTURN.TableSet.Samples](../src/Samples/xpTURN.TableSet.Samples)를 참고하여 직접 생성하거나, 혹은 샘플 프로젝트를 복사한 뒤 이름을 변경하여 사용할 수 있습니다.

### TableSet 구성
[데이터 정의](./DATA.md)를 참조하여 자신의 프로젝트에서 사용할 데이터 구조를 정의합니다. 

### TableSet 소스 생성
정의한 내용을 바탕으로 코드를 생성합니다. 생성된 TableSet 프로젝트의 .dll 파일은 xpTURN.ProtoGen.dll과 동일한 폴더에 위치해야 합니다.

코드 생성 예시 :
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
정의한 Table 구조에 맞춰 데이터를 구성합니다. [데이터 구성 방법](DATA.md)

### 데이터 바이너리화
런타임에서 사용하기 위해 컨버터 툴을 이용해 데이터를 변환합니다.

데이터 컨버트 예시 : 
```sh
dotnet ./xpTURN.Converter.dll --input="../../../Samples/DataSet/Sample1" --output="../../../Samples/DataSet/Sample1/[Result]" --namespace="Samples" --tableset="Sample1TableSet"
```

### 데이터 로드 및 사용
아래와 같이 코드를 작성해서 데이터를 로드합니다.
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
참고, Subset 파일(Sample1TableSet.Locale.bytes) 및 SetLocale 설정 등은 프로젝트 상황에 맞게 조정해야 합니다.
참고, SetLogger 또한 필요한 경우 설정할 수 있습니다.

데이터 접근 예시는 아래와 같습니다.
```cs
// Get the box data for a specific box
var boxData = Sample1TableSet.Instance.GetBoxData("box_0004");
Debug.Log($"BoxData: {boxData.Name}");
```

### 지속적인 관리
프로젝트 진행 중 데이터 정의는 주기적으로 변경되며, 데이터 입력도 수시로 이루어집니다. 이러한 작업은 CI 툴을 통해 자동화하는 것이 좋습니다.

#### Table 가공
데이터 가공이 필요한 경우 TableSetPostProcess를 상속받아 구성하시면 됩니다. [예시](../src/Samples/xpTURN.TableSet.Samples/Sample1/LocaleTablePostProcess.cs)

입력된 데이터의 논리적 검증이 필요한 경우에도 TableSetPostProcess를 상속 받아 검증 코드를 작성하시면 됩니다.

#### 프로젝트 내부 툴로 데이터 양산 / 데이터 통합
내부 툴로 데이터를 생성하는 경우, 아래 예시를 참고하여 Json 파일로 저장한 뒤 TableSet에 통합할 수 있습니다.

저장 방법 예시 :
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
일부 테이블을 분리 배포해야 하는 경우, 아래와 같이 설정하여 데이터 폴더 루트에 넣으면 변환 시 분리 저장됩니다.
여러 개의 Subset을 설정할 수 있지만, 동일한 테이블을 여러 Subset에 중복 포함할 수는 없습니다.

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
* "Locale"로 입력된 부분이 Subset 명칭이 들어가는 곳입니다.
* "Tables"에 들어 있는 Table 목록이 분리 저장될 Table의 목록 부분입니다.