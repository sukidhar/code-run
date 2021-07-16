using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Realms;
using TMPro;
using System.ComponentModel;

public class GameUser : RealmObject
{

    [PrimaryKey]
    public string id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string token { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string currentLanguage { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string currentChapterID { get; set; }
    [DefaultValue("0")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string level { get; set; }


    
    public GameUser() { }   
    public GameUser(string nickname,string email,string authenticationCode,string currentLanguage,string currentChapterId) {
        this.token = authenticationCode;
        this.username = nickname;
        this.email = email;
        this.currentChapterID = currentChapterId;
        this.currentLanguage = currentLanguage;
    }

    public void SetChapterID(string chapterid)
    {
        var realm = Realm.GetInstance();
        realm.Write(() =>
        {
            this.currentChapterID = chapterid;
        });
        realm.Dispose();
    }
}

