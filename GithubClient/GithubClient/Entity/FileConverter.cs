using GithubClient.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;

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
        bool isLightTheme = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible;
        if (file.Type.Equals("dir"))
        {
            file.Icon = isLightTheme ? "/Assets/Icons/directory-light.png" : "/Assets/Icons/directory-dark.png";
        }
        else
        {
            file.Icon = isLightTheme ? "/Assets/Icons/file-light.png" : "/Assets/Icons/file-dark.png";
        }
        return file;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

}