using UnityEngine;
using System.Globalization;
using Samples;

public class Warmup : MonoBehaviour
{
    void Start()
    {
        // Set the logger to use Unity's Debug class
        xpTURN.Common.Logger.Log.SetLogger(new xpLogger());

        bool result;

        // Load the Sample1TableSet data
        result = Sample1TableSet.Instance.Load($"{Application.streamingAssetsPath}/Sample1TableSet.bytes");
        if (!result)
            Debug.LogError("Failed to load Sample1TableSet");

        // Load the locale data for the Sample1TableSet
        result = Sample1TableSet.Instance.LoadAdditive($"{Application.streamingAssetsPath}/Sample1TableSet.Locale.bytes");
        if (!result)
            Debug.LogError("Failed to load Sample1TableSet.Locale");

        // Set the locale for the Sample1TableSet
        var cultureInfo = new CultureInfo("en-US");
        result = Sample1TableSet.Instance.SetLocale(cultureInfo.LCID);
        if (!result)
            Debug.LogError("Failed to set locale for Sample1TableSet");

        // Get the box data for a specific box
            var boxData = Sample1TableSet.Instance.GetBoxData("box_0004");
        Debug.Log($"BoxData: {boxData.Name}");

        // Iterate through the items in the box and log their names
        foreach (var box in boxData.List)
        {
            var itemData = Sample1TableSet.Instance.GetItemData(box.ItemRefId);
            Debug.Log($"Item Name: {itemData.Name}");
        }

        // Get the dice data for a specific dice box
        var diceData = Sample1TableSet.Instance.GetDiceData("dice_box_0001");
        foreach (var itemData in diceData.ListItem)
        {
            Debug.Log($"Item Name: {itemData.Name}");
        }

        // Save the box data table
        SaveData.Instance.Save();
    }
}
