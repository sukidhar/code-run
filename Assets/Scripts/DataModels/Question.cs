using System;
using Newtonsoft.Json;
public class Question
{
    [JsonProperty("tag")]
    public string tag { get; set; }

    [JsonProperty("query")]
    public string query { get; set; }

    [JsonProperty("key")]
    public string key { get; set; }

    public bool ValidateAnswer(string output)
    {
        //todo process the output
        return key.Equals(output);
    }
}
