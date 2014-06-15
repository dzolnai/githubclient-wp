using GithubClient.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class FileConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(File));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        File file = new File();
        file.Url = (string)jObject["url"];
        file.Name = (string)jObject["name"];
        file.Type = (string)jObject["type"];
        // icon depends on type
        if (file.Type.Equals("dir"))
        {
            file.Icon = "/Assets/Icons/directory.png";
        }
        else
        {
            file.Icon = "/Assets/Icons/file.png";
        }
        return file;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

}