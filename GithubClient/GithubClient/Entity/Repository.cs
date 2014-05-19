using Newtonsoft.Json;

[JsonConverter(typeof(RepositoryConverter))]
public class Repository
{
    public string url { get; set; }
    public string name { get; set; }
    public string owner { get; set; }
}