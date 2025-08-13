#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using xpTURN.Common;
using xpTURN.Protobuf;

public class JsonUtils
{
    /// <summary>
    /// Custom converter to convert ByteString to Base64 string
    /// </summary>
    public class ByteStringJsonConverter : JsonConverter<ByteString>
    {
        public override void WriteJson(JsonWriter writer, ByteString value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToBase64() ?? "");
        }

        public override ByteString ReadJson(JsonReader reader, Type objectType, ByteString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var str = reader.Value as string;
            return string.IsNullOrEmpty(str) ? ByteString.Empty : ByteString.FromBase64(str);
        }
    }

    static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter> { new ByteStringJsonConverter() }
    };

    public static string ToJson(object data)
    {
        if (data == null)
        {
            return string.Empty;
        }

        // Convert object to JObject
        var jsonObj = JObject.FromObject(data, JsonSerializer.Create(serializerSettings));

        // Add $type property if it does not exist
        if (jsonObj["$type"] == null)
        {
            var type = data.GetType();
            string typeString = $"{type.FullName}, {type.Assembly.GetName().Name}";
            jsonObj.AddFirst(new JProperty("$type", typeString));
        }

        return jsonObj.ToString(Formatting.Indented);
    }

    public static bool ToJsonFile(object data, string filePath)
    {
        if (data == null)
        {
            return false;
        }

        // Convert object to JSON string
        var json = ToJson(data);

        // Write JSON string to file
        try
        {
            var bytes = Encoding.UTF8.GetBytes(json);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllBytes(filePath, bytes);
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Failed to write JSON to file {filePath}: {ex.Message}");
            return false;
        }

        return true;
    }

    public static object FromJson(string json, Type type)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject(json, type);
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Failed to deserialize JSON to type {type.FullName}: {ex.Message}");
            return null;
        }
    }

    public static string GetTypeFromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        try
        {
            var jObj = JObject.Parse(json);
            return jObj["$type"]?.ToString();
        }
        catch
        {
            return null;
        }
    }

    public static bool PopulateObject(string json, object target)
    {
        if (string.IsNullOrEmpty(json))
        {
            return false;
        }

        try
        {
            JsonConvert.PopulateObject(json, target);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Failed to deserialize JSON to type {target.GetType().FullName}: {ex.Message}");
            return false;
        }
    }

    public static object FromJsonTypeInfo(string jsonContent, DebugInfo debugInfo)
    {
        // Get the type information from the JSON
        string typeString = GetTypeFromJson(jsonContent);
        if (string.IsNullOrEmpty(typeString))
        {
            Logger.Log.Tool.Error(debugInfo, $"Type information not found in JSON");
            return null;
        }

        string[] typeSplits = typeString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        string fullName = typeSplits.Length > 0 ? typeSplits[0] : string.Empty;
        string assemblyName = typeSplits.Length > 1 ? typeSplits[1] : string.Empty;

        var type = AssemblyUtils.GetTypeByName(fullName);
        if (type == null)
        {
            Logger.Log.Tool.Error(debugInfo, $"Type not found: {fullName}");
            return null;
        }

        // Create an instance of the Target
        object target = Activator.CreateInstance(type);

        // Deserialize the JSON content into the Target object
        if (!PopulateObject(jsonContent, target))
        {
            Logger.Log.Tool.Error(debugInfo, $"Failed to populate object of type {type.FullName}");
            return null;
        }

        return target;
    }

    public static object FromJsonFile(string file)
    {
        var debugInfo = new DebugInfo { File = file };

        // Load the JSON file
        if (!File.Exists(file))
        {
            Logger.Log.Tool.Error(debugInfo, $"File not found");
            return null;
        }

        string jsonContent = File.ReadAllText(file);
        if (string.IsNullOrEmpty(jsonContent))
        {
            Logger.Log.Tool.Error(debugInfo, $"Empty JSON file");
            return null;
        }

        return FromJsonTypeInfo(jsonContent, debugInfo);
    }
}
#endif // UNITY_EDITOR