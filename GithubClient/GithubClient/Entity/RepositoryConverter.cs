using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
public class RepositoryConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(Repository));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        Repository repository = new Repository();
        repository.url = (string) jObject["url"];
        repository.name = (string) jObject["name"];
        repository.owner = (string)jObject["owner"]["login"];
        return repository;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

}