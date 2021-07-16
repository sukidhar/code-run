using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using Realms;
public class WebManager
{
    public bool isLoading = false;
    private static string baseUrl = "http://localhost:5000";
    private WebManager() {}
    private static WebManager instance;
    public static WebManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WebManager();
            }
            return instance;
        }
    }

    public IEnumerator RequestRegistration(string email,string password, string nickname, System.Action<GameUser,String> callback)
    {
        isLoading = true;
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("nickname", nickname);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/auth/register",form))
        {

            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isHttpError || request.isNetworkError)
            {
                callback(null, request.error);
            }
            else
            {
                GameUser gameUser = JsonConvert.DeserializeObject<GameUser>(request.downloadHandler.text);
                callback(gameUser, "");
            }
        }
    }

    public IEnumerator RequestLogin(string email,string password, System.Action<GameUser,string> callback)
    {
        isLoading = true;
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/auth/login",form))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isHttpError || request.isNetworkError)
            {
                callback(null, request.error);
            }
            else
            {
                GameUser gameUser = JsonConvert.DeserializeObject<GameUser>(request.downloadHandler.text);
                callback(gameUser, "");
            }
        }
    }

    public IEnumerator RequestLevelCoroutine(string level,System.Action<Level,string> callback)
    {
        isLoading = true;
        
        using(UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/" + level))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isNetworkError || request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                Level levelObj = JsonConvert.DeserializeObject<Level>(request.downloadHandler.text);
                callback(levelObj, "");
            }
        }
    }

    

    private string GetApiKey()
    {
        Realm realm = Realm.GetInstance();
        var users = realm.All<GameUser>();
        return users.Count() > 0 ? users.First().token : "";
    }

    public IEnumerator RequestAllChapters(System.Action<List<Chapter>, string> callback)
    {
        isLoading = true;
        var token = GetApiKey();
        if (token.Length <= 0)
        {
            isLoading = false;
            callback(null, "No API KEY Found");
            yield return "";
        }
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/chapter/all/" + token))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isNetworkError || request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                ChapterResult result = JsonConvert.DeserializeObject<ChapterResult>(request.downloadHandler.text);
                callback(result.chapters, "");
            }
        }
    }

    public IEnumerator RequestChapterIntialisation(string chapterId, System.Action<List<Gate>, string> callback)
    {
        isLoading = true;
        var token = GetApiKey();
        if (token.Length <= 0)
        {
            isLoading = false;
            callback(null, "No API KEY Found");
            yield return "";
        }
        using(UnityWebRequest request = UnityWebRequest.Get(baseUrl+"/"+token+"/chapter/"+chapterId+"/gates/getAll"))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isNetworkError || request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                GateResult result = JsonConvert.DeserializeObject<GateResult>(request.downloadHandler.text);
                callback(result.gates, "");
            }
        }
        yield return new WaitForFixedUpdate();
    }



    public IEnumerator RequestCodeCompilation(string language,string gateId,string code,System.Action<bool,string> callback)
    {
        isLoading = true;
        var token = GetApiKey();
        if (token.Length <= 0)
        {
            isLoading = false;
            callback(false, "No API KEY Found");
            yield return "";
        }
        string url = baseUrl + "/unlock/gate/"+ gateId + "/" + token;
        WWWForm form = new WWWForm();
        form.AddField("code", code);
        form.AddField("language", language);
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                callback(false, request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                var result = request.downloadHandler.text.ToLower() == "true";
                callback(result, "");
            }
        }
    }
}
