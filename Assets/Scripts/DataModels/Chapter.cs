using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Realms;
using System.ComponentModel;
using System.Collections.Generic;

public class Chapter : RealmObject
{
    public string id { get; set; }
    public string title { get; set; }
    public string description { get; set; }

    public Chapter() { }
    public Chapter(string id, string title, string description) {
        this.id = id;
        this.title = title;
        this.description = description;
    }
}

public class ChapterResult
{
    [JsonProperty("data")]
    public List<Chapter> chapters { get; set; }
}