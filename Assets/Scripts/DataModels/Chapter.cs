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
    public int order { get; set; }
    public string status { get; set; }


    public Chapter() { }
    public Chapter(string id, string title, string description,int order) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.order = order;
        this.status = status;
    }
}

public class ChapterResult
{
    [JsonProperty("data")]
    public List<Chapter> chapters { get; set; }
}

public class ChapterComparator : IComparer<Chapter>
{
    public int Compare(Chapter x, Chapter y)
    {
        return x.order.CompareTo(y.order);
    }
}