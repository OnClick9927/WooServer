namespace WS.Core.Tool;

public static class JsonRecordTool
{
    private const string dir = "Records";
    public static void Save(string name, object obj)
    {
        var _path = Path.Combine(Context.CurrentDirectory, dir);
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        var path = $"{Path.Combine(Context.CurrentDirectory, dir, name)}.json";

        var value = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(path, value);
    }
    public static T? Read<T>(string name)
    {
        var path = $"{Path.Combine(Context.CurrentDirectory, dir, name)}.json";
        if (!File.Exists(path)) return default(T);
        var value = File.ReadAllText(path);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
    }
}
