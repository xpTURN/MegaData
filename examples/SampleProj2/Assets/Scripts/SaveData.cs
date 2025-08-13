#if UNITY_EDITOR
using UnityEngine;
using Samples;

public class SaveData
{
    public static SaveData Instance { get; private set; } = new SaveData();

    private SaveData()
    {
    }

    public void Save()
    {
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
    }
}
#endif // UNITY_EDITOR