using GithubClient.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class FileContentsConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(FileContents));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        FileContents contents = new FileContents();
        contents.Content = (string)jObject["content"];
        contents.Name = (string)jObject["name"];
        contents.Encoding = (string)jObject["encoding"];
        return contents;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

}