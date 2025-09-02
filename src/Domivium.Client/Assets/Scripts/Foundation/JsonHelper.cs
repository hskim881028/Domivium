using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable CheckNamespace
public static class JsonHelper
{
    public static string ExtractKey(object target, string key)
    {
        try
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(target));
            return jObject[key]?.ToString() ?? string.Empty;
        }
        catch
        {
            return "<invalid json>";
        }
    }

    public static string ExtractNestedKey(object target, string parentKey, string childKey)
    {
        try
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(target));
            return jObject[parentKey]?[childKey]?.ToString() ?? string.Empty;
        }
        catch
        {
            return "<invalid json>";
        }
    }
}