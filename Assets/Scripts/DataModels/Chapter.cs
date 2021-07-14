﻿using System;
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
    public string language { get; set; }


    public Chapter() { }
    public Chapter(string id, string title, string description,int order,string langauge) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.order = order;
        this.status = status;
        this.language = language;
    }
}

public class Gate : RealmObject
{
    public string id { get; set; }
    public string question { get; set; }
    public string key { get; set; }
    public string tag { get; set; }

    public Gate() { }
    public Gate(string id, string question, string key)
    {
        this.id = id;
        this.question = question;
        this.key = key;
        this.tag = tag;
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